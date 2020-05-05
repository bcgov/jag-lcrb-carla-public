import os
from data_integration.pipelines import Pipeline, Task
from data_integration.ui.cli import run_pipeline
import mara_db.auto_migration
import mara_db.config
import mara_db.dbs
import data_integration
import data_integration.config
from mara_app.monkey_patch import patch
from von_pipeline.von_data_pipelines import von_data_root_pipeline


patch(data_integration.config.system_statistics_collection_period)(lambda: 15)

@patch(data_integration.config.root_pipeline)
def root_pipeline():
    return von_data_root_pipeline()

mara_host = os.environ.get('MARA_DB_HOST', 'von-pipeline-db')
mara_database = os.environ.get('MARA_DB_DATABASE', 'mara_db')
mara_port = os.environ.get('MARA_DB_PORT', '5432')
mara_user = os.environ.get('MARA_DB_USER', 'mara_db')
mara_password = os.environ.get('MARA_DB_PASSWORD')

mara_db.config.databases \
    = lambda: {'mara': mara_db.dbs.PostgreSQLDB(user=mara_user, password=mara_password, host=mara_host, database=mara_database, port=mara_port)}

(status_pipeline, success) = data_integration.pipelines.find_node(['von_data_pipeline_status']) 
if success:
	run_pipeline(status_pipeline)
else:
	print("Pipeline not found")
