#
# Copyright 2017-2018 Government of Canada
# Public Services and Procurement Canada - buyandsell.gc.ca
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#

"""
Methods for loading and working with our standard YAML-based configuration files
"""

import base64
import logging
import os
import pathlib
import re
from typing import Callable, Mapping, TextIO

import pkg_resources
import yaml

LOGGER = logging.getLogger(__name__)


def load_resource(path: str) -> TextIO:
    """
    Open a resource file located in a python package or the local filesystem

    Args:
        path (str): The resource path in the form of `dir/file` or `package:dir/file`
    Returns:
        A file-like object representing the resource
    """
    components = path.rsplit(":", 1)
    if len(components) == 1:
        return open(components[0])
    return pkg_resources.resource_stream(components[0], components[1])


def load_settings(config_root=None, env=True) -> dict:
    """
    Loads the application settings from several sources:

        - settings.yml
        - an optional application settings file defined by SETTINGS_PATH
        - custom environment settings defined by ENVIRONMENT (ie. dev, prod)
        - enviroment variable overrides

    Args:
        env: A dict of environment variables, or the value True to inherit the global
            environment
    Returns:
        A combined dictionary of setting values
    """
    if env is True:
        env = os.environ
    elif not env:
        env = {}
    env_name = os.environ.get("ENVIRONMENT", "default")

    settings = {}

    # Load default settings
    if config_root:
        with open(config_root + "/settings.yml", "r") as stream:
            cfg = yaml.safe_load(stream)
    else:
        with load_resource("issuer_controller.config:settings.yml") as resource:
            cfg = yaml.safe_load(resource)

    if "default" not in cfg:
        raise ValueError("Default settings not found in settings.yml")
    settings.update(cfg["default"])
    if env_name != "default" and env_name in cfg:
        settings.update(cfg[env_name])

    # Load application settings
    ext_path = os.environ.get("SETTINGS_PATH")
    if not ext_path:
        config_root = os.environ.get("CONFIG_ROOT", "../config")
        ext_path = os.path.join(config_root, "settings.yml")
    with load_resource(ext_path) as resource:
        ext_cfg = yaml.load(resource, Loader=yaml.FullLoader)
        if "default" in ext_cfg:
            settings.update(ext_cfg["default"])
        if env_name != "default":
            if env_name not in ext_cfg:
                raise ValueError(
                    "Environment not defined by application settings: {}".format(
                        env_name
                    )
                )
            settings.update(ext_cfg[env_name])

    # Inherit environment variables
    for k, v in env.items():
        if v is not None and v != "":
            settings[k] = v

    # Expand variable references
    for k, v in settings.items():
        if isinstance(v, str):
            settings[k] = expand_string_variables(v, settings)

    return settings


def load_config(path: str, env=None):
    """
    Load a YAML config file and replace variables from the environment

    Args:
        path (str): The resource path in the form of `dir/file` or `package:dir/file`
    Returns:
        The configuration tree with variable references replaced, or `False` if the
        file is not found
    """
    try:
        with load_resource(path) as resource:
            cfg = yaml.load(resource, Loader=yaml.FullLoader)
    except FileNotFoundError:
        return False
    cfg = expand_tree_variables(cfg, env or os.environ)
    return cfg


def expand_string_variables(value, env: Mapping, warn: bool = True):
    """
    Expand environment variables of form `$var` and `${var}` in a string

    Args:
        value (str): The input value
        env (Mapping): The dictionary of environment variables
        warn (bool): Whether to warn on references to undefined variables
    Returns:
        The transformed string
    """
    if not isinstance(value, str):
        return value

    def _replace_var(matched):
        default = None
        var = matched.group(1)
        if matched.group(2):
            var = matched.group(2)
            default = matched.group(4)
        found = env.get(var)
        if found is None or found == "":
            found = default
        if found is None and warn:
            LOGGER.warning("Configuration variable not defined: %s", var)
            found = ""
        return found

    return re.sub(r"\$(?:(\w+)|\{([^}]*?)(:-([^}]*))?\})", _replace_var, value)


def map_tree(tree, map_fn: Callable):
    """
    Map one tree to another using a transformation function

    Args:
        tree: A sequence, mapping or other value
        map_fn (Callable): The function to apply to each node, returing the new value
    Returns:
        The transformed tree
    """
    if isinstance(tree, Mapping):
        return {key: map_tree(value, map_fn) for (key, value) in tree.items()}
    if isinstance(tree, (list, tuple)):
        return [map_tree(value, map_fn) for value in tree]
    return map_fn(tree)


def expand_tree_variables(tree, env: Mapping, warn: bool = True):
    """
    Expand environment variables of form `$var` and `${var}` in a configuration tree.
    This is used to allow variable insertion in issuer and route definitions

    Args:
        tree: A sequence, mapping or other value
        env (Mapping): The dictionary of environment variables
        warn (bool): Whether to warn on references to undefined variables
    Returns:
        The transformed tree
    """
    return map_tree(tree, lambda val: expand_string_variables(val, env, warn))


# Configuration logic from  vonx
def encode_logo_image(config: dict, path_root: str) -> str:
    """
    Encode logo image as base64 for transmission
    """
    if config.get("logo_b64"):
        return config["logo_b64"]
    elif config.get("logo_path"):
        path = pathlib.Path(path_root, config["logo_path"])
        if path.is_file():
            content = path.read_bytes()
            if content:
                return base64.b64encode(content).decode("ascii")
        else:
            LOGGER.warning("No file found at logo path: %s", path)
    return None


def extract_translated(config: dict, field: str, defval=None, deflang: str = "en"):
    ret = {deflang: defval}
    if config:
        pfx = field + "_"
        for k, v in config.items():
            if k == field:
                ret[deflang] = v
            elif k.startswith(pfx):
                lang = k[len(pfx) :]
                if lang:
                    ret[lang] = v
    return ret


def assemble_issuer_spec(config: dict) -> dict:
    """
    Create the issuer JSON definition which will be submitted to the OrgBook
    """

    config_root = config.get("config_root")
    deflang = "en"

    details = config.get("details", {})

    abbrevs = extract_translated(details, "abbreviation", "", deflang)
    labels = extract_translated(details, "label", "", deflang)
    urls = extract_translated(details, "url", "", deflang)

    spec = {
        "name": config.get("name"),
        # "name": labels[deflang] or config.get("email"),
        "did": config.get("did"),
        "email": config.get("email"),
        "logo_b64": encode_logo_image(details, config_root),
        "abbreviation": abbrevs[deflang],
        "url": urls[deflang],
        "endpoint": config.get("endpoint"),
    }
    spec["abbreviations"] = {}
    for k, v in abbrevs.items():
        spec["abbreviations"][k] = v
    spec["labels"] = {}
    for k, v in labels.items():
        spec["labels"][k] = v
    spec["urls"] = {}
    for k, v in urls.items():
        spec["urls"][k] = v

    return spec


def assemble_credential_type_spec(config: dict, schema_attrs: dict) -> dict:
    """
    Create the issuer JSON definition which will be submitted to the OrgBook
    """

    CRED_TYPE_PARAMETERS = (
        "cardinality_fields",
        "category_labels",
        "claim_descriptions",
        "claim_labels",
        "credential",
        "details",
        "mapping",
        "topic",
        "visible_fields",
    )

    config_root = config.get("config_root")
    deflang = "en"

    if not config.get("topic"):
        raise RuntimeError(
            "Missing 'topic' for credential type " + config.get("schema_name")
        )

    if not config.get("issuer_url"):
        raise RuntimeError(
            "Missing 'issuer_url' for credential type " + config.get("schema_name")
        )

    labels = extract_translated(config, "label", config.get("schema_name"), deflang)
    urls = extract_translated(config, "url", config.get("issuer_url"), deflang)
    logo_b64 = encode_logo_image(config, config_root)

    claim_labels = {}
    claim_descriptions = {}
    for k, v in schema_attrs.items():
        claim_labels[k] = extract_translated(v, "label", k, deflang)
        claim_descriptions[k] = extract_translated(v, "description", k, deflang)

    ctype = {
        "schema": config.get("schema_name"),
        "version": config.get("schema_version"),
        "credential_def_id": config.get("credential_def_id"),
        "name": labels[deflang],
        "endpoint": urls[deflang],
        "topic": [],
        "logo_b64": logo_b64,
    }
    topics = (
        config["topic"]
        if isinstance(config["topic"], list)
        else [
            config["topic"],
        ]
    )
    for config_topic in topics:
        cred_topic = {}
        has_label = False
        for k, v in config_topic.items():
            if not k.startswith("label"):
                cred_topic[k] = v
            else:
                has_label = True
        if has_label:
            cred_topic["labels"] = extract_translated(
                config_topic, "label", None, deflang
            )
        ctype["topic"].append(cred_topic)
    ctype["labels"] = {}
    for k in labels:
        ctype["labels"][k] = labels[k]
    ctype["endpoints"] = {}
    for k in urls:
        ctype["endpoints"][k] = urls[k]
    for k in CRED_TYPE_PARAMETERS:
        if k != "details" and k in config and k not in ctype:
            ctype[k] = config[k]

    ctype["claim_labels"] = claim_labels
    ctype["claim_descriptions"] = claim_descriptions

    return ctype


#####
sample_app_config = {
    "AGENT_ADMIN_URL": "http://myorg-agent:8034",
    "DID": "XZxwKKqiKaV6yZQob1UZpq",
    "TOB_CONNECTION": "cee21dfa-cd60-479b-b096-9db9552fa948",
    "running": True,
    "schemas": {
        "CRED_DEF_test-permit.org_1.0.0": "XZxwKKqiKaV6yZQob1UZpq:3:CL:14:default",
        "CRED_DEF_my-registration.org_1.0.0": "XZxwKKqiKaV6yZQob1UZpq:3:CL:10:default",
        "CRED_DEF_my-relationship.org_1.0.0": "XZxwKKqiKaV6yZQob1UZpq:3:CL:12:default",
        "SCHEMA_test-permit.org": {
            "attributes": {
                "corp_num": {
                    "data_type": "ui_text",
                    "description_en": "Registration/Incorporation "
                    "Number "
                    "or "
                    "Identifying "
                    "Number",
                    "label_en": "Registration " "ID",
                    "required": True,
                },
                "effective_date": {
                    "data_type": "ui_date",
                    "description_en": "Date " "Credential " "is " "effective",
                    "label_en": "Credential " "Effective " "Date",
                    "required": True,
                },
                "entity_name": {
                    "data_type": "ui_name",
                    "description_en": "The " "legal " "name " "of " "entity",
                    "label_en": "Name",
                    "required": True,
                },
                "permit_id": {
                    "data_type": "helper_uuid",
                    "description_en": "Permit " "Identifying " "Number",
                    "label_en": "Permit " "ID",
                    "required": True,
                },
                "permit_issued_date": {
                    "data_type": "ui_date",
                    "description_en": "Date " "Permit " "is " "issued",
                    "label_en": "Permit " "Issued " "Date",
                    "required": True,
                },
                "permit_status": {
                    "data_type": "ui_select",
                    "description_en": "Status " "of " "the " "permit",
                    "label_en": "Permit " "Status",
                    "required": True,
                },
                "permit_type": {
                    "data_type": "ui_select",
                    "description_en": "Status " "of " "the " "permit",
                    "label_en": "Permit " "Type",
                    "required": True,
                },
            },
            "description": "The " "test-permit " "credential " "issued by " "org",
            "name": "test-permit.org",
            "version": "1.0.0",
        },
        "SCHEMA_test-permit.org_1.0.0": "XZxwKKqiKaV6yZQob1UZpq:2:test-permit.org:1.0.0",
        "SCHEMA_my-registration.org": {
            "attributes": {
                "address_line_1": {
                    "data_type": "ui_text",
                    "description": "address_line_1",
                    "required": True,
                },
                "addressee": {
                    "data_type": "ui_text",
                    "description": "addressee",
                    "required": True,
                },
                "city": {
                    "data_type": "ui_text",
                    "description": "city",
                    "required": True,
                },
                "corp_num": {
                    "data_type": "helper_uuid",
                    "description_en": "Registration/Incorporation "
                    "Number "
                    "or "
                    "Identifying "
                    "Number",
                    "label_en": "Registration " "ID",
                    "required": True,
                },
                "country": {
                    "data_type": "ui_text",
                    "description": "country",
                    "required": True,
                },
                "effective_date": {
                    "data_type": "ui_date",
                    "description_en": "Date " "Credential " "is " "effective",
                    "label_en": "Credential " "Effective " "Date",
                    "required": True,
                },
                "entity_name": {
                    "data_type": "ui_name",
                    "description_en": "The " "legal " "name " "of " "entity",
                    "label_en": "Name",
                    "required": True,
                },
                "entity_name_effective": {
                    "data_type": "ui_date",
                    "description_en": "Date " "current " "name " "became " "effective",
                    "label_en": "Name " "Effective " "Date",
                    "required": True,
                },
                "entity_status": {
                    "data_type": "ui_select",
                    "description_en": "Status "
                    "of "
                    "the "
                    "entity "
                    "(active "
                    "or "
                    "historical)",
                    "label_en": "Registration " "Status",
                    "required": True,
                },
                "entity_status_effective": {
                    "data_type": "ui_date",
                    "description_en": "Date " "status " "became " "effective",
                    "label_en": "Status " "Effective " "Date",
                    "required": True,
                },
                "entity_type": {
                    "data_type": "ui_text",
                    "description_en": "Type "
                    "of "
                    "entity "
                    "incorporated "
                    "or "
                    "registered",
                    "label_en": "Registration " "Type",
                    "required": True,
                },
                "expiry_date": {
                    "data_type": "ui_date",
                    "description_en": "Date " "Credential " "expired",
                    "label_en": "Credential " "Expiry " "Date",
                    "required": False,
                },
                "postal_code": {
                    "data_type": "ui_text",
                    "description": "postal_code",
                    "required": True,
                },
                "province": {
                    "data_type": "ui_text",
                    "description": "province",
                    "required": True,
                },
                "registered_jurisdiction": {
                    "data_type": "ui_text",
                    "description_en": "The "
                    "jurisdiction "
                    "an "
                    "entity "
                    "was "
                    "created "
                    "in",
                    "label_en": "Registered " "Jurisdiction",
                    "required": False,
                },
                "registration_date": {
                    "data_type": "ui_date",
                    "description_en": "Date "
                    "of "
                    "Registration, "
                    "Incorporation, "
                    "Continuation "
                    "or "
                    "Amalgamation",
                    "label_en": "Registration " "Date",
                    "required": False,
                },
            },
            "description": "The " "my-registration " "credential issued " "by org",
            "name": "my-registration.org",
            "version": "1.0.0",
        },
        "SCHEMA_my-registration.org_1.0.0": "XZxwKKqiKaV6yZQob1UZpq:2:my-registration.org:1.0.0",
        "SCHEMA_my-relationship.org": {
            "attributes": {
                "associated_corp_num": {
                    "data_type": "ui_text",
                    "description_en": "Registry "
                    "id(s) "
                    "of "
                    "associated "
                    "organizations/individuals",
                    "label_en": "Associated " "Registration " "ID",
                    "required": True,
                },
                "associated_registration_name": {
                    "data_type": "ui_text",
                    "description_en": "Registered "
                    "name "
                    "of "
                    "associated "
                    "organizations/individuals",
                    "label_en": "Associated " "Registration " "Namwe",
                    "required": False,
                },
                "corp_num": {
                    "data_type": "ui_text",
                    "description_en": "Unique "
                    "identifer "
                    "assigned "
                    "to "
                    "entity "
                    "by "
                    "registrar",
                    "label_en": "Registration " "ID",
                    "required": True,
                },
                "effective_date": {
                    "data_type": "ui_date",
                    "description_en": "Date " "Credential " "is " "effective",
                    "label_en": "Effective " "Date",
                    "required": True,
                },
                "expiry_date": {
                    "data_type": "ui_date",
                    "description_en": "Date " "Credential " "expires",
                    "label_en": "Credential " "Expiry " "Date",
                    "required": False,
                },
                "relationship": {
                    "data_type": "ui_text",
                    "description_en": "Name " "of " "the " "relationship",
                    "label_en": "Relationship",
                    "required": True,
                },
                "relationship_description": {
                    "data_type": "ui_text",
                    "description_en": "Description " "of " "the " "relationship",
                    "label_en": "Relationship " "Description",
                    "required": True,
                },
                "relationship_status": {
                    "data_type": "ui_select",
                    "description_en": "Status " "of " "the " "relationship",
                    "label_en": "Relationship " "Status",
                    "required": True,
                },
                "relationship_status_effective": {
                    "data_type": "ui_date",
                    "description_en": "Date "
                    "the "
                    "relationship "
                    "became/becomes "
                    "effective",
                    "label_en": "Relationship " "Status " "Effective",
                    "required": False,
                },
            },
            "description": "The relationship " "between two " "organizations",
            "name": "my-relationship.org",
            "version": "1.0.0",
        },
        "SCHEMA_my-relationship.org_1.0.0": "XZxwKKqiKaV6yZQob1UZpq:2:my-relationship.org:1.0.0",
    },
}


TestConfig = {
    # other fixed values
    "test_issuer_app_config": sample_app_config,
    "test_issuer_synced": {"cee21dfa-cd60-479b-b096-9db9552fa948": True},
    # This is a full pprint of the ENV dict created in the app.py file, only uncommmented values
    # that cause test to error if missing
    #'ACK_ERROR_PCT': '0',
    "AGENT_ADMIN_URL": "http://myorg-agent:8034",
    "APPLICATION_URL": "http://localhost:5000",
    # 'APPLICATION_URL_VONX': 'http://localhost:5000/org/test-permit',
    # 'AUTO_REGISTER_DID': False,
    "CONFIG_ROOT": "./config",
    # 'DESCRIPTION': 'von-image provides a consistent base image for running VON '
    #                 'python web components. Based on Ubuntu bionic, this image '
    #                 'includes Python 3.6.9,  indy-sdk, and supporting Python '
    #                 'libraries.',
    # 'ENABLE_GUNICORN': '0',
    # 'ENDPOINT_URL': 'http://172.17.0.1:5000',
    # 'ENVIRONMENT': 'default',
    # 'HOME': '/app/controller',
    # 'HOSTNAME': '723f6b74f663',
    # 'HOST_IP': '0.0.0.0',
    # 'HOST_PORT': '5000',
    # 'HTTP_FORCE_CLOSE_CONNECTIONS': 'true',
    # 'INDY_GENESIS_PATH': '/app/controller/genesis',
    # 'INDY_GENESIS_URL': 'http://localhost:9000/genesis',
    # 'INDY_LEDGER_URL': 'http://172.17.0.1:9000',
    # 'LANG': 'C.UTF-8',
    # 'LC_ALL': 'C.UTF-8',
    # 'LD_LIBRARY_PATH': '/app/controller/.local/lib:',
    # 'LEDGER_PROTOCOL_VERSION': '1.6',
    # 'LEDGER_URL': 'http://172.17.0.1:9000',
    # 'LS_COLORS': 'rs=0:di=01;34:ln=01;36:mh=00:pi=40;33:so=01;35:do=01;35:bd=40;33;01:cd=40;33;01:or=40;31;01:mi=00:su=37;41:sg=30;43:ca=30;41:tw=30;42:ow=34;42:st=37;44:ex=01;32:*.tar=01;31:*.tgz=01;31:*.arc=01;31:*.arj=01;31:*.taz=01;31:*.lha=01;31:*.lz4=01;31:*.lzh=01;31:*.lzma=01;31:*.tlz=01;31:*.txz=01;31:*.tzo=01;31:*.t7z=01;31:*.zip=01;31:*.z=01;31:*.Z=01;31:*.dz=01;31:*.gz=01;31:*.lrz=01;31:*.lz=01;31:*.lzo=01;31:*.xz=01;31:*.zst=01;31:*.tzst=01;31:*.bz2=01;31:*.bz=01;31:*.tbz=01;31:*.tbz2=01;31:*.tz=01;31:*.deb=01;31:*.rpm=01;31:*.jar=01;31:*.war=01;31:*.ear=01;31:*.sar=01;31:*.rar=01;31:*.alz=01;31:*.ace=01;31:*.zoo=01;31:*.cpio=01;31:*.7z=01;31:*.rz=01;31:*.cab=01;31:*.wim=01;31:*.swm=01;31:*.dwm=01;31:*.esd=01;31:*.jpg=01;35:*.jpeg=01;35:*.mjpg=01;35:*.mjpeg=01;35:*.gif=01;35:*.bmp=01;35:*.pbm=01;35:*.pgm=01;35:*.ppm=01;35:*.tga=01;35:*.xbm=01;35:*.xpm=01;35:*.tif=01;35:*.tiff=01;35:*.png=01;35:*.svg=01;35:*.svgz=01;35:*.mng=01;35:*.pcx=01;35:*.mov=01;35:*.mpg=01;35:*.mpeg=01;35:*.m2v=01;35:*.mkv=01;35:*.webm=01;35:*.ogm=01;35:*.mp4=01;35:*.m4v=01;35:*.mp4v=01;35:*.vob=01;35:*.qt=01;35:*.nuv=01;35:*.wmv=01;35:*.asf=01;35:*.rm=01;35:*.rmvb=01;35:*.flc=01;35:*.avi=01;35:*.fli=01;35:*.flv=01;35:*.gl=01;35:*.dl=01;35:*.xcf=01;35:*.xwd=01;35:*.yuv=01;35:*.cgm=01;35:*.emf=01;35:*.ogv=01;35:*.ogx=01;35:*.aac=00;36:*.au=00;36:*.flac=00;36:*.m4a=00;36:*.mid=00;36:*.midi=00;36:*.mka=00;36:*.mp3=00;36:*.mpc=00;36:*.ogg=00;36:*.ra=00;36:*.wav=00;36:*.oga=00;36:*.opus=00;36:*.spx=00;36:*.xspf=00;36:',
    # 'PATH': '/app/controller/.pyenv/versions/3.6.9/bin:/app/controller/.pyenv/libexec:/app/controller/.pyenv/plugins/python-build/bin:/app/controller/.local/bin:/app/controller/bin:/app/controller/.pyenv/shims:/app/controller/.pyenv/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin',
    # 'PIP_NO_CACHE_DIR': 'off',
    # 'POSTGRESQL_WALLET_ADMIN_PASSWORD': 'mysecretpassword',
    # 'POSTGRESQL_WALLET_HOST': 'myorg-wallet-db',
    # 'POSTGRESQL_WALLET_PASSWORD': 'DB_PASSWORD',
    # 'POSTGRESQL_WALLET_PORT': '5432',
    # 'POSTGRESQL_WALLET_USER': 'DB_USER',
    # 'PWD': '/app/controller',
    # 'PYENV_DIR': '/app/controller',
    # 'PYENV_HOOK_PATH': '/app/controller/.pyenv/pyenv.d:/usr/local/etc/pyenv.d:/etc/pyenv.d:/usr/lib/pyenv/hooks',
    # 'PYENV_ROOT': '/app/controller/.pyenv',
    # 'PYENV_VERSION': '3.6.9',
    # 'PYTEST_CURRENT_TEST': 'test/issue_credential_resource_test.py::test_liveness_method '
    #                         '(setup)',
    # 'PYTHONIOENCODING': 'UTF-8',
    # 'PYTHONUNBUFFERED': '1',
    # 'PYTHON_ENV': 'development',
    # 'PYTHON_VERSION': '3.6.9',
    # 'RUST_LOG': 'warning',
    # 'SHELL': '/bin/bash',
    # 'SHLVL': '1',
    # 'SUMMARY': 'von-image including Python 3.6.9 and indy-sdk',
    # 'TEMPLATE_PATH': '../templates',
    # 'TERM': 'xterm',
    # 'TOB_ADMIN_API_KEY': 'R2D2HfPM5Zwd69IjclQiuFmcMV6',
    # 'TOB_AGENT_ADMIN_URL': 'http://172.17.0.1:8024',
    # 'TOB_API_URL': 'http://localhost:8081/api/v2',
    # 'TOB_APP_URL': 'http://localhost:8080',
    # 'TOB_CONNECTION_NAME': 'vcr-agent',
    # 'TRACE_EVENTS': 'false',
    # 'TRACE_MSG_PCT': '0',
    # 'TRACE_TARGET': 'log',
    # 'WALLET_ENCRYPTION_KEY': 'key',
    # 'WALLET_SEED_VONX': 'empr_000000000000000000000000000',
    # 'WALLET_TYPE': 'postgres_storage',
    # 'WEBHOOK_PORT': '5000'
}
