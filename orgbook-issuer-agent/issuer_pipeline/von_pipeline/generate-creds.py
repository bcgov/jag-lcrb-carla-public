#!/usr/bin/python
import psycopg2
import datetime
from von_pipeline.config import config
from von_pipeline.eventprocessor import EventProcessor


with EventProcessor() as event_processor:
    event_processor.process_event_queue()

