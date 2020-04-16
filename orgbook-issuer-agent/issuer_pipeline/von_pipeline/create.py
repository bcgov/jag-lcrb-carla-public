#!/usr/bin/python
 
import psycopg2
from pymongo import MongoClient
from von_pipeline.config import config
from von_pipeline.eventprocessor import EventProcessor

 
with EventProcessor() as event_processor:
    event_processor.create_tables()
    print("Created event processor tables")

