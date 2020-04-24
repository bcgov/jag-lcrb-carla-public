#!/usr/bin/python
import psycopg2
from pymongo import MongoClient
import datetime
from von_pipeline.config import config
from tests.gen_test_data import *


print("TODO load some test data")
mongo_sample_data(3, 2)

mdb_config = config(section='eao_data')
print(mdb_config['host'], mdb_config['port'], mdb_config['database'])
client = MongoClient('mongodb://%s:%s@%s:%s' % (mdb_config['user'], mdb_config['password'], mdb_config['host'], mdb_config['port']))
db = client[mdb_config['database']]

collections = db.collection_names(include_system_collections=False)
print(collections)
