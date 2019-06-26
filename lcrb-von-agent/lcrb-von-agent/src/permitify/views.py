
import logging.config
from typing import Coroutine

from aiohttp import web, ClientRequest
import aiohttp_jinja2

from vonx.common import config, manager
from vonx.web.view_helpers import (
    IndyRequestError,
    get_handle_id,
    get_manager,
    get_request_json,
    indy_client,
    perform_issue_credential,
    perform_store_credential,
    service_request,
)
from vonx.web.routes import RouteDefinitions
from .view_helpers import (
    call_orgbook_api,
    orgbook_creds_for_org,
    orgbook_topic_to_creds,
    filter_by_dependent_proof_requests,
)

LOGGER = logging.getLogger(__name__)


def get_agent_routes(app) -> list:
    """
    Get the standard list of routes for the von-x agent
    """
    handler = AgentHandler(app['manager'])

    routes = []
    routes.extend([
        web.get('/chooser', handler.render_chooser),
        web.post('/chooser2', handler.process_chooser),
        web.post('/chooser3', handler.process_chooser),
        web.get('/search_credential/{org_name}', handler.search_credential),
        web.get('/search_credential/{org_name}/{service_name}', handler.search_credential),
        web.get('/filter_credential/{org_name}/{service_name}', handler.filter_credential),
    ])

    # add routes for all configured forms
    routes.extend(
        web.view(form['path'] + '/{org_name}', form_handler(form, handler), name=form['name'] + '-ivy')
                    for form in handler.forms)

    return routes


class AgentHandler:
    def __init__(self, cfg_mgr: manager.ConfigServiceManager):
        self.forms = RouteDefinitions.load(cfg_mgr).forms
        self.proofs = cfg_mgr.services_config("proof_requests")
        pass

    async def agent_form_handler(self, form: dict, request: web.Request) -> web.Response:
        org_name = request.match_info.get("org_name")

        result_creds = await orgbook_creds_for_org(org_name)

        cred_ids = []
        if "proof_request" in form:
            if not form["proof_request"]["id"] in self.proofs:
                raise RuntimeError(
                    'Proof request not found for service: {} {}'.format(service_name, form["proof_request"]["id"])
                )
            proof = self.proofs[form["proof_request"]["id"]]
            result_creds = filter_by_dependent_proof_requests(form, proof, result_creds, True)
            print(result_creds)
            for key in result_creds:
                creds = result_creds[key]
                for cred in creds:
                    cred_ids.append(cred['wallet_id'])
        else:
            print(result_creds)
            for cred in result_creds:
                cred_ids.append(cred['wallet_id'])

        print("Redirecting :-D", len(cred_ids), cred_ids)
        location = request.app.router[form['id']].url_for()
        if 0 < len(cred_ids):
            query = 'credential_ids='
            for i in range(len(cred_ids)):
                if i > 0:
                    query = query + ','
                query = query + cred_ids[i]
            location = location.with_query(query)
        print("Redirecting --> ", location)
        raise web.HTTPFound(location=location)

    async def filter_credential(self, request: web.Request) -> web.Response:
        return await self.search_credential(request, True)

    async def search_credential(self, request: web.Request, latest=False) -> web.Response:
        org_name = request.match_info.get("org_name")
        service_name = request.match_info.get("service_name")

        result_creds = await orgbook_creds_for_org(org_name)

        if service_name is not None and 0 < len(service_name):
            found = False
            for form in self.forms:
                if form["name"] == service_name:
                    found = True
                    if "proof_request" in form:
                        if not form["proof_request"]["id"] in self.proofs:
                            raise RuntimeError(
                                'Proof request not found for service: {} {}'.format(service_name, form["proof_request"]["id"])
                            )
                        proof = self.proofs[form["proof_request"]["id"]]
                        result_creds = filter_by_dependent_proof_requests(form, proof, result_creds, latest)
            if not found:
                raise RuntimeError(
                    'Service not found: {}'.format(service_name)
                )

        response = web.json_response(result_creds)
        return response

    async def render_chooser(self, request: web.Request) -> web.Response:
        """
        Respond with HTTP code 200 if services are ready to accept new credentials, 451 otherwise
        """

        tpl_name = "my_chooser.html"
        tpl_vars = {}

        result = await get_manager(request).get_service_status('manager')
        ok = result and result.get("services", {}).get("indy", {}).get("synced")

        tpl_vars["result_text"] = 'ok get chooser' if ok else ''
        tpl_vars["ok_text"] = '200' if ok else '451'

        print("tpl_vars", tpl_vars)

        response = aiohttp_jinja2.render_template(tpl_name, request, tpl_vars)
        print(response)
        return response


    async def process_chooser(self, request: web.Request) -> web.Response:
        """
        Respond with HTTP code 200 if services are ready to accept new credentials, 451 otherwise
        """

        tpl_name = "my_chooser.html"
        tpl_vars = {}

        inputs = await request.post()

        corp_num = inputs.get("corp_num")
        if corp_num is not None and corp_num != '':
            tpl_vars["corp_num"] = corp_num
            tpl_name = "my_chooser2.html"
        cred_schema = inputs.get("credential_type")
        if cred_schema is not None and cred_schema != '':
            tpl_vars["cred_schema"] = cred_schema
        credential_id = inputs.get("credential_id")
        if credential_id is not None and credential_id != '':
            tpl_vars["credential_id"] = credential_id
            tpl_name = "my_chooser3.html"

        print("corp_num", corp_num)
        print("cred_schema", cred_schema)
        print("credential_id", credential_id)

        if corp_num is not None and corp_num == "goto":
            print("Redirecting :-D")
            location = request.app.router['myorg-issue'].url_for()
            raise web.HTTPFound(location=location)
            #return web.Response(status=307, headers={'location': "/myorg/myorg-credential",},) 
        else:
            print("Go to page ", tpl_name)
            response = aiohttp_jinja2.render_template(tpl_name, request, tpl_vars)
            print(response)
            return response


def form_handler(form: dict, handler: AgentHandler) -> Coroutine:
    """
    Return a request handler for processing form routes
    """
    async def _process(request: ClientRequest):
        if request.method == 'GET' or request.method == 'HEAD':
            return await handler.agent_form_handler(form, request)
        elif request.method == 'POST':
            return await handler.agent_form_handler(form, request)
        return web.Response(status=405)
    return _process
