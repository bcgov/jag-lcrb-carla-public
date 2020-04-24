import datetime

import data_integration.config
import mara_db.config
import mara_db.dbs
from mara_app.monkey_patch import patch
import os

import app.config


@patch(mara_db.config.databases)
def databases():
    mara_host = os.environ.get('MARA_DB_HOST', 'von-pipeline-db')
    mara_database = os.environ.get('MARA_DB_DATABASE', 'mara_db')
    mara_port = os.environ.get('MARA_DB_PORT', '5432')
    mara_user = os.environ.get('MARA_DB_USER', 'mara_db')
    mara_password = os.environ.get('MARA_DB_PASSWORD')

    return {
        'mara': mara_db.dbs.PostgreSQLDB(user=mara_user, password=mara_password, host=mara_host, database=mara_database, port=mara_port)
    }


# How many cores to use for running the ETL, defaults to the number of CPUs of the machine
# On production, make sure the ETL does not slow down other services too much
patch(data_integration.config.max_number_of_parallel_tasks)(lambda: 4)

# The first day for which to download and process data (default 2017-01-01).
# Locally, a few days of data is enough to test a pipeline.
# On production, size of days that can be processed depends on machine size.
# One year of data amounts to roughly 50GB database size
patch(app.config.first_date)(lambda: datetime.date.today() - datetime.timedelta(days=5))

# Whether it is possible to run the ETL from the web UI
# Disable on production
patch(data_integration.config.allow_run_from_web_ui)(lambda: True)
