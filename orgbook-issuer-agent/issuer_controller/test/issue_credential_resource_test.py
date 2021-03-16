import pytest,threading,json, random

from time import sleep

from unittest.mock import MagicMock, patch, PropertyMock
from src import issuer,config

test_send_credential = [
    {
        "schema": "my-registration.org",
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
            "addressee": "A Person",
            "address_line_1": "123 Some Street",
            "city": "Victoria",
            "country": "Canada",
            "postal_code": "V1V1V1",
            "province": "BC",
            "effective_date": "2019-01-01",
            "expiry_date": ""
        }
    },
    {
        "schema": "test-permit.org",
        "version": "1.0.0",
        "attributes": {
            "permit_id": "MYPERMIT12345",
            "entity_name": "Ima Permit",
            "corp_num": "ABC12345",
            "permit_issued_date": "2018-01-01", 
            "permit_type": "ABC", 
            "permit_status": "OK", 
            "effective_date": "2019-01-01"
        }
    }
]


def test_liveness_method(app):
    val = issuer.issuer_liveness_check()
    assert val


def test_liveness_route(test_client):
    get_resp = test_client.get(f'/liveness')
    assert get_resp.status_code == 200

def test_health_method(app):
    val = issuer.tob_connection_synced()
    assert val

def test_health_route(test_client):
    get_resp = test_client.get(f'/health')
    assert get_resp.status_code == 200


##-------------Issue-Credential--------------
class MockSendCredentialThread(threading.Thread):
    def __init__(self,*args):
        threading.Thread.__init__(self)
        return

    def run(self):
        sleep(random.randint(1,1000)/1000)
        self.cred_response = {"success": True, "result":"MOCK_RESPONSE"}
        return    

def test_issue_credential_spawns_thread(app):
    with patch('src.issuer.SendCredentialThread',new=MockSendCredentialThread) as mock:
        res = issuer.handle_send_credential(test_send_credential)
        assert res.status_code == 200
        responses = json.loads(res.response[0])
        assert 'MOCK' in responses[0]["result"] 
        assert all(r['success'] == True for r in responses)
        assert len(responses) == 2


def test_SendCredentialThread_posts_to_agent(app):
    cred_def = "CRED_DEF_my-registration.org_1.0.0"
    cred_offer =  {"test":"tests","test2":"test2"}
    agent_url = config.TestConfig.get("AGENT_ADMIN_URL") + "/issue-credential/send"
    headers = {"Content-Type": "application/json"}

    with patch('src.issuer.requests.post') as mock:
        thread = issuer.SendCredentialThread(
            cred_def,
            cred_offer,
            agent_url,
            headers,
        )
        thread.start()
        thread.join()
        mock.assert_called_with(agent_url, json.dumps(cred_offer), headers=headers)
