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
        config_root = os.environ.get("CONFIG_ROOT", os.curdir)
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
    for k, v in abbrevs.items():
        spec["abbreviation_{}".format(k)] = v
    for k, v in labels.items():
        spec["label_{}".format(k)] = v
    for k, v in urls.items():
        spec["url_{}".format(k)] = v

    return spec


def assemble_credential_type_spec(config: dict) -> dict:
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
        raise RuntimeError("Missing 'topic' for credential type")

    if not config.get("issuer_url"):
        raise RuntimeError("Missing 'issuer_url' for credential type")

    labels = extract_translated(config, "label", config.get("schema_name"), deflang)
    urls = extract_translated(config, "url", config.get("issuer_url"), deflang)
    logo_b64 = encode_logo_image(config, config_root)

    ctype = {
        "schema": config.get("schema_name"),
        "version": config.get("schema_version"),
        "credential_def_id": config.get("credential_def_id"),
        "name": labels[deflang],
        "endpoint": urls[deflang],
        "topic": (config["topic"] if isinstance(config["topic"], list) else [config["topic"],]),
        "logo_b64": logo_b64,
    }
    for k in labels:
        ctype["label_{}".format(k)] = labels[k]
    for k in urls:
        ctype["endpoint_{}".format(k)] = urls[k]
    for k in CRED_TYPE_PARAMETERS:
        if k != "details" and k in config and k not in ctype:
            ctype[k] = config[k]

    return ctype
