import pytest, os, pprint

from src import config, issuer, app as entry_file
from unittest.mock import patch
from requests.models import Response

@pytest.fixture(scope="session")
def app(request):
    test_app = entry_file.app
    test_app.startup_thread.join()
    #stop startup thread and override with test config
    issuer.app_config = config.TestConfig['test_issuer_app_config']
    issuer.synced = config.TestConfig['test_issuer_synced']


    return test_app

@pytest.fixture(scope='session')
def test_client(app):
    client = app.test_client()
    ctx = app.app_context()
    ctx.push()
    yield client
    ctx.pop()