
import logging.config

from aiohttp import ClientSession
from vonx.common import config

LOGGER = logging.getLogger(__name__)

ENV = config.load_settings()


async def call_orgbook_api(uri):
    try:
        http_client = ClientSession()

        url = ENV.get('TOB_API_URL') + uri
        response = await http_client.get(url)
        if response.status != 200:
            raise RuntimeError(
                'OrgBook API call failed: {}'.format(await response.text())
            )
        result_json = await response.json()
        return result_json
    except (Exception) as error:
        raise
    finally:
        await http_client.close()

async def orgbook_creds_for_org(org_name):
    topic_uri = '/topic/ident/registration/' + org_name 
    topic_result_json = await call_orgbook_api(topic_uri)

    if "id" not in topic_result_json:
        raise RuntimeError(
            'No organization found for: {}'.format(org_name)
        )
    topic_id = topic_result_json["id"]

    topic_search_uri = '/topic/' + str(topic_id) + '/credential/active'
    result_json = await call_orgbook_api(topic_search_uri)

    if 0 == len(result_json):
        raise RuntimeError(
            'No credentials found for: {}'.format(org_name)
        )
    result_creds = orgbook_topic_to_creds(result_json)
    
    return result_creds

def orgbook_topic_to_creds(topic_json):
    result_creds = []
    for result in topic_json:
        cred = {}
        cred["issuer_name"] = result["credential_type"]["issuer"]["name"]
        cred["issuer_did"] = result["credential_type"]["issuer"]["did"]
        cred["schema_name"] = result["credential_type"]["schema"]["name"]
        cred["schema_version"] = result["credential_type"]["schema"]["version"]
        cred["description"] = result["credential_type"]["description"]
        cred["effective_date"] = result["effective_date"]
        cred["wallet_id"] = result["wallet_id"]
        cred["id"] = result["id"]
        cred["topic_id"] = result["topic"]["id"]
        cred["source_id"] = result["topic"]["source_id"]
        result_creds.append(cred)
    return result_creds

def filter_by_dependent_proof_requests(form, proof, creds, latest=False):
    print("form", form)
    print("proof", proof)

    return_creds = {}
    for cred in creds:
        for schema in proof["schemas"]:
            if schema['key']['did'] == cred['issuer_did'] and schema['key']['name'] == cred['schema_name'] and schema['key']['version'] == cred['schema_version']:
                key = schema['key']['did'] + '::' + schema['key']['name'] + '::' + schema['key']['version']
                if latest:
                    if not key in return_creds:
                        return_creds[key] = []
                        return_creds[key].append(cred)
                    else:
                        if cred['effective_date'] > return_creds[key][0]['effective_date']:
                            return_creds[key][0] = cred
                else:
                    if not key in return_creds:
                        return_creds[key] = []
                    return_creds[key].append(cred)

    return return_creds
