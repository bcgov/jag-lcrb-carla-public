#!/usr/bin/python
from configparser import ConfigParser
import os 
import urllib.parse

 
def config(filename='database.ini', section='postgresql'):
    db = {}
    if section == 'event_processor':
        db['host'] = os.environ.get('EVENT_PROC_DB_HOST', 'localhost')
        db['port'] = os.environ.get('EVENT_PROC_DB_PORT', '5444')
        db['database'] = os.environ.get('EVENT_PROC_DB_DATABASE', 'pipeline_db')
        db['user'] = os.environ.get('EVENT_PROC_DB_USER', 'pipeline_db')
        db['password'] = os.environ.get('EVENT_PROC_DB_PASSWORD', '')
    else:
        raise Exception('Section {0} not a valid database'.format(section))
 
    return db

