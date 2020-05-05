
from pymongo import MongoClient
import datetime

from von_pipeline.config import config
from .gen_test_data import *

from von_pipeline.eventprocessor import EventProcessor


def test_eao_sample_data():
    with EventProcessor() as eao_pipeline:
        objects = eao_pipeline.find_unprocessed_objects()
        obj_tree = eao_pipeline.organize_unprocessed_objects(objects)

        # TODO verify results?

def test_eao_generate_creds():
    with EventProcessor() as eao_pipeline:
        objects = eao_pipeline.find_unprocessed_objects()
        obj_tree = eao_pipeline.organize_unprocessed_objects(objects)

        creds = eao_pipeline.generate_all_credentials(obj_tree, save_to_db=False)

        # TODO verify results?
        print(creds)

