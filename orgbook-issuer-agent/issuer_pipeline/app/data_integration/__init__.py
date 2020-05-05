"""Configures the data integration pipelines of the project"""

import datetime
import functools

import data_integration.config
from data_integration.pipelines import Pipeline, Task
from mara_app.monkey_patch import patch

import app.config

from von_pipeline.von_pipelines import von_root_pipeline

patch(data_integration.config.data_dir)(lambda: app.config.data_dir())
patch(data_integration.config.first_date)(lambda: app.config.first_date())
patch(data_integration.config.default_db_alias)(lambda: 'dwh')
patch(data_integration.config.system_statistics_collection_period)(lambda: 15)


@patch(data_integration.config.root_pipeline)
@functools.lru_cache(maxsize=None)
def root_pipeline():
    return von_root_pipeline()
