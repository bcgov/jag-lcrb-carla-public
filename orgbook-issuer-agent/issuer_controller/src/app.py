#!/usr/bin/env python
from flask import Flask, jsonify, abort, request, make_response

import requests
import json
import os
import time
import yaml
import config
import issuer

# Load application settings (environment)
config_root = os.environ.get('CONFIG_ROOT', '../config')
ENV = config.load_settings(config_root=config_root)

class Controller(Flask):
    def __init__(self):
        print("Initializing " + __name__ + " ...")
        super().__init__(__name__)
        issuer.startup_init(ENV)

app = Controller()
wsgi_app = app.wsgi_app

@app.route('/health', methods=['GET'])
def health_check():
    return make_response(jsonify({'success': True}), 200)

@app.route('/status/reset', methods=['GET'])
def clear_status():
    issuer.clear_stats()
    return make_response(jsonify({'success': True}), 200)

@app.route('/status', methods=['GET'])
def get_status():
    return make_response(jsonify(issuer.get_stats()), 200)

@app.errorhandler(404)
def not_found(error):
    return make_response(jsonify({'error': 'Not found'}), 404)

@app.route('/issue-credential', methods=['POST'])
def submit_credential():
    """
    Exposed method to proxy credential issuance requests.
    """
    if not issuer.tob_connection_synced():
        abort(503, "Connection not yet synced")

    start_time = time.perf_counter()
    method = 'submit_credential.batch'

    if not request.json:
        end_time = time.perf_counter()
        issuer.log_timing_method(method, start_time, end_time, False)
        abort(400)

    cred_input = request.json

    response = issuer.handle_send_credential(cred_input)

    end_time = time.perf_counter()
    issuer.log_timing_method(method, start_time, end_time, True)

    return response


@app.route('/api/agentcb/topic/<topic>/', methods=['POST'])
def agent_callback(topic):
    """
    Main callback for aries agent.  Dispatches calls based on the supplied topic.
    """
    start_time = time.perf_counter()
    method = 'agent_callback.' + topic

    if not request.json:
        end_time = time.perf_counter()
        issuer.log_timing_method(method, start_time, end_time, False)
        abort(400)

    message = request.json

    # dispatch based on the topic type
    if topic == issuer.TOPIC_CONNECTIONS:
        if "state" in message:
            method = method + '.' + message["state"]
            response = issuer.handle_connections(message["state"], message)
        else:
            response = jsonify({})

    elif topic == issuer.TOPIC_CONNECTIONS_ACTIVITY:
        response = jsonify({})

    elif topic == issuer.TOPIC_CREDENTIALS:
        if "state" in message:
            method = method + '.' + message["state"]
            response = issuer.handle_credentials(message["state"], message)
        else:
            response = jsonify({})

    elif topic == issuer.TOPIC_PRESENTATIONS:
        if "state" in message:
            method = method + '.' + message["state"]
            response = issuer.handle_presentations(message["state"], message)
        else:
            response = jsonify({})

    elif topic == issuer.TOPIC_GET_ACTIVE_MENU:
        response = issuer.handle_get_active_menu(message)

    elif topic == issuer.TOPIC_PERFORM_MENU_ACTION:
        response = issuer.handle_perform_menu_action(message)

    elif topic == issuer.TOPIC_ISSUER_REGISTRATION:
        response = issuer.handle_register_issuer(message)
    
    elif topic == issuer.TOPIC_PROBLEM_REPORT:
        response = issuer.handle_problem_report(message)

    else:
        print("Callback: topic=", topic, ", message=", message)
        end_time = time.perf_counter()
        issuer.log_timing_method(method, start_time, end_time, False)
        abort(400, {'message': 'Invalid topic: ' + topic})

    end_time = time.perf_counter()
    issuer.log_timing_method(method, start_time, end_time, True)

    return response
