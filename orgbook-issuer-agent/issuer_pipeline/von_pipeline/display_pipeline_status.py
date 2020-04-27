#!/usr/bin/python
import psycopg2
import datetime
import json
import decimal
from von_pipeline.config import config
from von_pipeline.eventprocessor import EventProcessor


with EventProcessor() as event_processor:
    event_processor.display_event_processing_status()



