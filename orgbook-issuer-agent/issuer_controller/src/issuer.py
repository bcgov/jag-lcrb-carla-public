import json
import os
import threading
import time

import requests
from flask import jsonify

import config

AGENT_ADMIN_API_KEY = os.environ.get("AGENT_ADMIN_API_KEY")
ADMIN_REQUEST_HEADERS = {}
if AGENT_ADMIN_API_KEY is not None:
    ADMIN_REQUEST_HEADERS = {"x-api-key": AGENT_ADMIN_API_KEY}

TOB_ADMIN_API_KEY = os.environ.get("TOB_ADMIN_API_KEY")
TOB_REQUEST_HEADERS = {}
if TOB_ADMIN_API_KEY is not None:
    TOB_REQUEST_HEADERS = {"x-api-key": TOB_ADMIN_API_KEY}

# list of cred defs per schema name/version
app_config = {}
app_config["schemas"] = {}
synced = {}

MAX_RETRIES = 3


def agent_post_with_retry(url, payload, headers=None):
    retries = 0
    while True:
        try:
            # test code to test exception handling
            # if retries < MAX_RETRIES:
            #    raise Exception("Fake exception!!!")
            response = requests.post(url, payload, headers=headers)
            response.raise_for_status()
            return response
        except Exception as e:
            print("Error posting", url, e)
            retries = retries + 1
            if retries > MAX_RETRIES:
                raise e
            time.sleep(5)


def agent_schemas_cred_defs(agent_admin_url):
    ret_schemas = {}

    # get loaded cred defs and schemas
    response = requests.get(
        agent_admin_url + "/schemas/created", headers=ADMIN_REQUEST_HEADERS
    )
    response.raise_for_status()
    schemas = response.json()["schema_ids"]
    for schema_id in schemas:
        response = requests.get(
            agent_admin_url + "/schemas/" + schema_id, headers=ADMIN_REQUEST_HEADERS
        )
        response.raise_for_status()
        schema = response.json()["schema"]
        schema_key = schema["name"] + "::" + schema["version"]
        ret_schemas[schema_key] = {"schema": schema, "schema_id": str(schema["seqNo"])}

    response = requests.get(
        agent_admin_url + "/credential-definitions/created",
        headers=ADMIN_REQUEST_HEADERS,
    )
    response.raise_for_status()
    cred_defs = response.json()["credential_definition_ids"]
    for cred_def_id in cred_defs:
        response = requests.get(
            agent_admin_url + "/credential-definitions/" + cred_def_id,
            headers=ADMIN_REQUEST_HEADERS,
        )
        response.raise_for_status()
        cred_def = response.json()["credential_definition"]
        for schema_key in ret_schemas:
            if ret_schemas[schema_key]["schema_id"] == cred_def["schemaId"]:
                ret_schemas[schema_key]["cred_def"] = cred_def
                break

    return ret_schemas


class StartupProcessingThread(threading.Thread):
    global app_config

    def __init__(self, ENV):
        threading.Thread.__init__(self)
        self.ENV = ENV

    def run(self):
        # read configuration files
        config_root = self.ENV.get("CONFIG_ROOT", "../config")
        config_schemas = config.load_config(config_root + "/schemas.yml", env=self.ENV)
        config_services = config.load_config(
            config_root + "/services.yml", env=self.ENV
        )

        # print("schemas.yml -->", json.dumps(config_schemas))
        # print("services.yml -->", json.dumps(config_services))

        agent_admin_url = self.ENV.get("AGENT_ADMIN_URL")
        if not agent_admin_url:
            raise RuntimeError(
                "Error AGENT_ADMIN_URL is not specified, can't connect to Agent."
            )
        app_config["AGENT_ADMIN_URL"] = agent_admin_url

        # get public DID from our agent
        response = requests.get(
            agent_admin_url + "/wallet/did/public", headers=ADMIN_REQUEST_HEADERS
        )
        result = response.json()
        did = result["result"]
        print("Fetched DID from agent: ", did)
        app_config["DID"] = did["did"]

        # determine pre-registered schemas and cred defs
        existing_schemas = agent_schemas_cred_defs(agent_admin_url)
        # print("Existing schemas:", json.dumps(existing_schemas))

        # register schemas and credential definitions
        for schema in config_schemas:
            schema_name = schema["name"]
            schema_version = schema["version"]
            schema_key = schema_name + "::" + schema_version
            if schema_key not in existing_schemas:
                schema_attrs = []
                schema_descs = {}
                if isinstance(schema["attributes"], dict):
                    # each element is a dict
                    for attr, desc in schema["attributes"].items():
                        schema_attrs.append(attr)
                        schema_descs[attr] = desc
                else:
                    # assume it's an array
                    for attr in schema["attributes"]:
                        schema_attrs.append(attr)

                # register our schema(s) and credential definition(s)
                schema_request = {
                    "schema_name": schema_name,
                    "schema_version": schema_version,
                    "attributes": schema_attrs,
                }
                response = agent_post_with_retry(
                    agent_admin_url + "/schemas",
                    json.dumps(schema_request),
                    headers=ADMIN_REQUEST_HEADERS,
                )
                response.raise_for_status()
                schema_id = response.json()
            else:
                schema_id = {"schema_id": existing_schemas[schema_key]["schema"]["id"]}
            app_config["schemas"]["SCHEMA_" + schema_name] = schema
            app_config["schemas"][
                "SCHEMA_" + schema_name + "_" + schema_version
            ] = schema_id["schema_id"]
            print("Registered schema: ", schema_id)

            if (
                schema_key not in existing_schemas
                or "cred_def" not in existing_schemas[schema_key]
            ):
                cred_def_request = {"schema_id": schema_id["schema_id"]}
                response = agent_post_with_retry(
                    agent_admin_url + "/credential-definitions",
                    json.dumps(cred_def_request),
                    headers=ADMIN_REQUEST_HEADERS,
                )
                response.raise_for_status()
                credential_definition_id = response.json()
            else:
                credential_definition_id = {
                    "credential_definition_id": existing_schemas[schema_key][
                        "cred_def"
                    ]["id"]
                }
            app_config["schemas"][
                "CRED_DEF_" + schema_name + "_" + schema_version
            ] = credential_definition_id["credential_definition_id"]
            print("Registered credential definition: ", credential_definition_id)

        # what is the TOB connection name?
        tob_connection_params = config_services["verifiers"]["bctob"]

        # check if we have a TOB connection
        response = requests.get(
            agent_admin_url + "/connections?alias=" + tob_connection_params["alias"],
            headers=ADMIN_REQUEST_HEADERS,
        )
        response.raise_for_status()
        connections = response.json()["results"]
        tob_connection = None
        for connection in connections:
            # check for TOB connection
            if connection["alias"] == tob_connection_params["alias"]:
                tob_connection = connection

        if not tob_connection:
            # if no tob connection then establish one
            tob_agent_admin_url = tob_connection_params["connection"]["agent_admin_url"]
            if not tob_agent_admin_url:
                raise RuntimeError(
                    "Error TOB_AGENT_ADMIN_URL is not specified, can't establish a TOB connection."
                )

            response = requests.post(
                tob_agent_admin_url + "/connections/create-invitation",
                headers=TOB_REQUEST_HEADERS,
            )
            response.raise_for_status()
            invitation = response.json()

            response = requests.post(
                agent_admin_url
                + "/connections/receive-invitation?alias="
                + tob_connection_params["alias"],
                json.dumps(invitation["invitation"]),
                headers=ADMIN_REQUEST_HEADERS,
            )
            response.raise_for_status()
            tob_connection = response.json()

            print("Established tob connection: ", tob_connection)
            time.sleep(5)

        app_config["TOB_CONNECTION"] = tob_connection["connection_id"]
        synced[tob_connection["connection_id"]] = False

        for issuer_name, issuer_info in config_services["issuers"].items():
            # register ourselves (issuer, schema(s), cred def(s)) with TOB
            issuer_config = {
                "name": issuer_name,
                "did": app_config["DID"],
                "config_root": config_root,
            }
            issuer_config.update(issuer_info)
            issuer_spec = config.assemble_issuer_spec(issuer_config)

            credential_types = []
            for credential_type in issuer_info["credential_types"]:
                schema_name = credential_type["schema"]
                schema_info = app_config["schemas"]["SCHEMA_" + schema_name]
                ctype_config = {
                    "schema_name": schema_name,
                    "schema_version": schema_version,
                    "issuer_url": issuer_config["url"],
                    "config_root": config_root,
                    "credential_def_id": app_config["schemas"][
                        "CRED_DEF_" + schema_name + "_" + schema_info["version"]
                    ],
                }
                credential_type['attributes'] = schema_info["attributes"]
                ctype_config.update(credential_type)
                ctype = config.assemble_credential_type_spec(ctype_config, schema_info.get("attributes"))
                if ctype is not None:
                    credential_types.append(ctype)

            issuer_request = {
                "connection_id": app_config["TOB_CONNECTION"],
                "issuer_registration": {
                    "credential_types": credential_types,
                    "issuer": issuer_spec,
                },
            }

            print(json.dumps(issuer_request))
            response = requests.post(
                agent_admin_url + "/issuer_registration/send",
                json.dumps(issuer_request),
                headers=ADMIN_REQUEST_HEADERS,
            )
            response.raise_for_status()
            response.json()
            print("Registered issuer: ", issuer_name)

        synced[tob_connection["connection_id"]] = True
        print("Connection {} is synchronized".format(tob_connection))


def tob_connection_synced():
    return (
        ("TOB_CONNECTION" in app_config)
        and (app_config["TOB_CONNECTION"] in synced)
        and (synced[app_config["TOB_CONNECTION"]])
    )


def startup_init(ENV):
    global app_config

    thread = StartupProcessingThread(ENV)
    thread.start()


credential_lock = threading.Lock()
credential_requests = {}
credential_responses = {}
credential_threads = {}


timing_lock = threading.Lock()
record_timings = True
timings = {}


def clear_stats():
    global timings
    timing_lock.acquire()
    try:
        timings = {}
    finally:
        timing_lock.release()


def get_stats():
    timing_lock.acquire()
    try:
        return timings
    finally:
        timing_lock.release()


def log_timing_method(method, start_time, end_time, success, data=None):
    if not record_timings:
        return

    timing_lock.acquire()
    try:
        elapsed_time = end_time - start_time
        if not method in timings:
            timings[method] = {
                "total_count": 1,
                "success_count": 1 if success else 0,
                "fail_count": 0 if success else 1,
                "min_time": elapsed_time,
                "max_time": elapsed_time,
                "total_time": elapsed_time,
                "avg_time": elapsed_time,
                "data": {},
            }
        else:
            timings[method]["total_count"] = timings[method]["total_count"] + 1
            if success:
                timings[method]["success_count"] = timings[method]["success_count"] + 1
            else:
                timings[method]["fail_count"] = timings[method]["fail_count"] + 1
            if elapsed_time > timings[method]["max_time"]:
                timings[method]["max_time"] = elapsed_time
            if elapsed_time < timings[method]["min_time"]:
                timings[method]["min_time"] = elapsed_time
            timings[method]["total_time"] = timings[method]["total_time"] + elapsed_time
            timings[method]["avg_time"] = (
                timings[method]["total_time"] / timings[method]["total_count"]
            )
        if data:
            timings[method]["data"][str(timings[method]["total_count"])] = data
    finally:
        timing_lock.release()


def set_credential_thread_id(cred_exch_id, thread_id):
    credential_lock.acquire()
    try:
        # add 2 records so we can x-ref
        print("Set cred_exch_id, thread_id", cred_exch_id, thread_id)
        credential_threads[thread_id] = cred_exch_id
        credential_threads[cred_exch_id] = thread_id
    finally:
        credential_lock.release()


def add_credential_request(cred_exch_id):
    credential_lock.acquire()
    try:
        # short circuit if we already have the response
        if cred_exch_id in credential_responses:
            return None

        result_available = threading.Event()
        credential_requests[cred_exch_id] = result_available
        return result_available
    finally:
        credential_lock.release()


def add_credential_response(cred_exch_id, response):
    credential_lock.acquire()
    try:
        credential_responses[cred_exch_id] = response
        if cred_exch_id in credential_requests:
            result_available = credential_requests[cred_exch_id]
            result_available.set()
            del credential_requests[cred_exch_id]
    finally:
        credential_lock.release()


def add_credential_problem_report(thread_id, response):
    print("get problem report for thread", thread_id)
    if thread_id in credential_threads:
        cred_exch_id = credential_threads[thread_id]
        add_credential_response(cred_exch_id, response)
    else:
        print("thread_id not found", thread_id)
        # hack for now
        if 1 == len(list(credential_requests.keys())):
            cred_exch_id = list(credential_requests.keys())[0]
            add_credential_response(cred_exch_id, response)
        else:
            print("darn, too many outstanding requests :-(")
            print(credential_requests)


def add_credential_timeout_report(cred_exch_id):
    print("add timeout report for cred", cred_exch_id)
    response = {"success": False, "result": cred_exch_id + "::Error thread timeout"}
    add_credential_response(cred_exch_id, response)


def add_credential_exception_report(cred_exch_id, exc):
    print("add exception report for cred", cred_exch_id)
    response = {"success": False, "result": cred_exch_id + "::" + str(exc)}
    add_credential_response(cred_exch_id, response)


def get_credential_response(cred_exch_id):
    credential_lock.acquire()
    try:
        if cred_exch_id in credential_responses:
            response = credential_responses[cred_exch_id]
            del credential_responses[cred_exch_id]
            if cred_exch_id in credential_threads:
                thread_id = credential_threads[cred_exch_id]
                print("cleaning out cred_exch_id, thread_id", cred_exch_id, thread_id)
                del credential_threads[cred_exch_id]
                del credential_threads[thread_id]
            return response
        else:
            return None
    finally:
        credential_lock.release()


TOPIC_CONNECTIONS = "connections"
TOPIC_CONNECTIONS_ACTIVITY = "connections_actvity"
TOPIC_CREDENTIALS = "issue_credential"
TOPIC_PRESENTATIONS = "presentations"
TOPIC_GET_ACTIVE_MENU = "get-active-menu"
TOPIC_PERFORM_MENU_ACTION = "perform-menu-action"
TOPIC_ISSUER_REGISTRATION = "issuer_registration"
TOPIC_PROBLEM_REPORT = "problem-report"

# max 15 second wait for a credential response (prevents blocking forever)
MAX_CRED_RESPONSE_TIMEOUT = 45


def handle_connections(state, message):
    # TODO auto-accept?
    print("handle_connections()", state)
    return jsonify({"message": state})


def handle_credentials(state, message):
    # TODO auto-respond to proof requests
    print("handle_credentials()", state, message["credential_exchange_id"])
    # TODO new "stored" state is being added by Nick
    if "thread_id" in message:
        set_credential_thread_id(
            message["credential_exchange_id"], message["thread_id"]
        )
    if state == "credential_acked":
        response = {"success": True, "result": message["credential_exchange_id"]}
        add_credential_response(message["credential_exchange_id"], response)
    return jsonify({"message": state})


def handle_presentations(state, message):
    # TODO auto-respond to proof requests
    print("handle_presentations()", state)
    return jsonify({"message": state})


def handle_get_active_menu(message):
    # TODO add/update issuer info?
    print("handle_get_active_menu()", message)
    return jsonify({})


def handle_perform_menu_action(message):
    # TODO add/update issuer info?
    print("handle_perform_menu_action()", message)
    return jsonify({})


def handle_register_issuer(message):
    # TODO add/update issuer info?
    print("handle_register_issuer()")
    return jsonify({})


def handle_problem_report(message):
    print("handle_problem_report()", message)

    msg = message["~thread"]["thid"] + "::" + message["explain-ltxt"]
    response = {"success": False, "result": msg}
    add_credential_problem_report(message["~thread"]["thid"], response)

    return jsonify({})


class SendCredentialThread(threading.Thread):
    def __init__(self, credential_definition_id, cred_offer, url, headers):
        threading.Thread.__init__(self)
        self.credential_definition_id = credential_definition_id
        self.cred_offer = cred_offer
        self.url = url
        self.headers = headers

    def run(self):
        start_time = time.perf_counter()
        method = "submit_credential.credential"

        cred_data = None
        try:
            response = requests.post(
                self.url, json.dumps(self.cred_offer), headers=self.headers
            )
            response.raise_for_status()
            cred_data = response.json()
            result_available = add_credential_request(
                cred_data["credential_exchange_id"]
            )
            # print(
            #    "Sent offer",
            #    cred_data["credential_exchange_id"],
            #    cred_data["connection_id"],
            # )

            # wait for confirmation from the agent, which will include the credential exchange id
            if result_available and not result_available.wait(
                MAX_CRED_RESPONSE_TIMEOUT
            ):
                add_credential_timeout_report(cred_data["credential_exchange_id"])
                end_time = time.perf_counter()
                print(
                    "Got credential TIMEOUT:",
                    cred_data["credential_exchange_id"],
                    cred_data["connection_id"],
                )
                log_timing_method(
                    method,
                    start_time,
                    end_time,
                    False,
                    data={
                        "thread_id": cred_data["thread_id"],
                        "credential_exchange_id": cred_data["credential_exchange_id"],
                        "Error": "Timeout",
                        "elapsed_time": (end_time - start_time),
                    },
                )
            else:
                # print(
                #    "Got credential response:",
                #    cred_data["credential_exchange_id"],
                #    cred_data["connection_id"],
                # )
                end_time = time.perf_counter()
                log_timing_method(method, start_time, end_time, True)
                pass

        except Exception as exc:
            print(exc)
            end_time = time.perf_counter()
            # if cred_data is not set we don't have a credential to set status for
            if cred_data:
                add_credential_exception_report(
                    cred_data["credential_exchange_id"], exc
                )
                data = {
                    "thread_id": cred_data["thread_id"],
                    "credential_exchange_id": cred_data["credential_exchange_id"],
                    "Error": str(exc),
                    "elapsed_time": (end_time - start_time),
                }
            else:
                data = {"Error": str(exc), "elapsed_time": (end_time - start_time)}
            log_timing_method(method, start_time, end_time, False, data=data)
            # don't re-raise; we want to log the exception as the credential error response

        self.cred_response = get_credential_response(
            cred_data["credential_exchange_id"]
        )
        processing_time = end_time - start_time
        # print("Got response", self.cred_response, "time=", processing_time)


def handle_send_credential(cred_input):
    """
    # other sample data
    sample_credentials = [
        {
            "schema": "ian-registration.ian-ville",
            "version": "1.0.0",
            "attributes": {
                "corp_num": "ABC12345",
                "registration_date": "2018-01-01", 
                "entity_name": "Ima Permit",
                "entity_name_effective": "2018-01-01", 
                "entity_status": "ACT", 
                "entity_status_effective": "2019-01-01",
                "entity_type": "ABC", 
                "registered_jurisdiction": "BC", 
                "effective_date": "2019-01-01",
                "expiry_date": ""
            }
        },
        {
            "schema": "ian-permit.ian-ville",
            "version": "1.0.0",
            "attributes": {
                "permit_id": str(uuid.uuid4()),
                "entity_name": "Ima Permit",
                "corp_num": "ABC12345",
                "permit_issued_date": "2018-01-01", 
                "permit_type": "ABC", 
                "permit_status": "OK", 
                "effective_date": "2019-01-01"
            }
        }
    ]
    """
    # construct and send the credential
    # print("Received credentials", cred_input)
    global app_config

    agent_admin_url = app_config["AGENT_ADMIN_URL"]

    start_time = time.perf_counter()
    processing_time = 0
    processed_count = 0

    # let's send a credential!
    cred_responses = []
    for credential in cred_input:
        cred_def_key = "CRED_DEF_" + credential["schema"] + "_" + credential["version"]
        credential_definition_id = app_config["schemas"][cred_def_key]
        schema_name = credential["schema"]
        schema_info = app_config["schemas"]["SCHEMA_" + schema_name]
        schema_version = schema_info["version"]
        schema_id = app_config["schemas"][
            "SCHEMA_" + schema_name + "_" + schema_version
        ]
        cred_req = {
            "schema_issuer_did": app_config["DID"],
            "issuer_did": app_config["DID"],
            "schema_name": schema_name,
            "cred_def_id": credential_definition_id,
            "schema_version": schema_version,
            "credential_proposal": {
                "@type": "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview",
                "attributes": [
                    {"name": attr_name, "value": attr_value}
                    for attr_name, attr_value in credential["attributes"].items()
                ],
            },
            "connection_id": app_config["TOB_CONNECTION"],
            # "comment": "string",
            "schema_id": schema_id,
        }

        thread = SendCredentialThread(
            credential_definition_id,
            cred_req,
            agent_admin_url + "/issue-credential/send",
            ADMIN_REQUEST_HEADERS,
        )
        thread.start()
        thread.join()
        cred_responses.append(thread.cred_response)
        processed_count = processed_count + 1

    processing_time = time.perf_counter() - start_time
    print(">>> Processed", processed_count, "credentials in", processing_time)
    print("   ", processing_time / processed_count, "seconds per credential")

    return jsonify(cred_responses)
