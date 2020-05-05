#!/usr/bin/python
 
import datetime
import decimal
import hashlib
import json
import time
import traceback
import types
import uuid
import random
import string
import copy
from enum import Enum

import pytz

import psycopg2
from von_pipeline.config import config

PIPELINE_SYSTEM_TYPE = 'PPL'

CORP_BATCH_SIZE = 3000

# number of test/random credentials to generate and post
GEN_TOPIC_COUNT = 100

MIN_START_DATE = datetime.datetime(datetime.MINYEAR+1, 1, 1)
MAX_END_DATE   = datetime.datetime(datetime.MAXYEAR-1, 12, 31)
DATA_CONVERSION_DATE = datetime.datetime(2004, 3, 26)

# for now, we are in PST time
timezone = pytz.timezone("America/Los_Angeles")

MIN_START_DATE_TZ = timezone.localize(MIN_START_DATE)
MAX_END_DATE_TZ   = timezone.localize(MAX_END_DATE)
DATA_CONVERSION_DATE_TZ = timezone.localize(DATA_CONVERSION_DATE)

# custom encoder to convert wierd data types to strings
class CustomJsonEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, datetime.datetime):
            try:
                tz_aware = timezone.localize(o)
                return tz_aware.astimezone(pytz.utc).isoformat()
            except (Exception) as error:
                if o.year <= datetime.MINYEAR+1:
                    return MIN_START_DATE_TZ.astimezone(pytz.utc).isoformat()
                elif o.year >= datetime.MAXYEAR-1:
                    return MAX_END_DATE_TZ.astimezone(pytz.utc).isoformat()
                return o.isoformat()
        if isinstance(o, (list, dict, str, int, float, bool, type(None))):
            return JSONEncoder.default(self, o)        
        if isinstance(o, decimal.Decimal):
            return (str(o) for o in [o])
        if isinstance(o, set):
            return list(o)
        if isinstance(o, map):
            return list(o)
        if isinstance(o, types.GeneratorType):
            ret = ""
            for s in next(o):
                ret = ret + str(s)
            return ret
        if isinstance(o, ObjectId):
            return str(o)
        return json.JSONEncoder.default(self, o)

class DateTimeEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, datetime.datetime):
            try:
                tz_aware = timezone.localize(o)
                return tz_aware.astimezone(pytz.utc).isoformat()
            except (Exception) as error:
                if o.year <= datetime.MINYEAR+1:
                    return MIN_START_DATE_TZ.astimezone(pytz.utc).isoformat()
                elif o.year >= datetime.MAXYEAR-1:
                    return MAX_END_DATE_TZ.astimezone(pytz.utc).isoformat()
                return o.isoformat()
        return json.JSONEncoder.default(self, o)


# interface to Event Processor database
class EventProcessor:
    def __init__(self):
        try:
            params = config(section='event_processor')
            self.conn = psycopg2.connect(**params)
        except (Exception) as error:
            print(error)
            print(traceback.print_exc())
            self.conn = None
            raise

    def __del__(self):
        if self.conn:
            self.conn.close()
            self.conn = None

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc_value, traceback):
        pass
 
    # create our base processing tables
    def create_tables(self):
        """ create tables in the PostgreSQL database"""
        commands = (
            """
            CREATE TABLE IF NOT EXISTS LAST_EVENT (
                RECORD_ID SERIAL PRIMARY KEY,
                SYSTEM_TYPE_CD VARCHAR(255) NOT NULL, 
                COLLECTION VARCHAR(255) NOT NULL,
                OBJECT_DATE TIMESTAMP NOT NULL,
                ENTRY_DATE TIMESTAMP NOT NULL
            )
            """,
            """
            CREATE INDEX IF NOT EXISTS le_stc ON LAST_EVENT 
            (SYSTEM_TYPE_CD);
            """,
            """
            CREATE TABLE IF NOT EXISTS EVENT_HISTORY_LOG (
                RECORD_ID SERIAL PRIMARY KEY,
                SYSTEM_TYPE_CD VARCHAR(255) NOT NULL, 
                COLLECTION VARCHAR(255) NOT NULL,
                PROJECT_ID VARCHAR(255) NOT NULL,
                PROJECT_NAME VARCHAR(255) NOT NULL,
                OBJECT_ID VARCHAR(255) NOT NULL,
                OBJECT_DATE TIMESTAMP NOT NULL,
                UPLOAD_DATE TIMESTAMP,
                UPLOAD_HASH VARCHAR(255),
                ENTRY_DATE TIMESTAMP NOT NULL,
                PROCESS_DATE TIMESTAMP,
                PROCESS_SUCCESS CHAR,
                PROCESS_MSG VARCHAR(255)
            )
            """,
            """
            -- Hit for counts and queries
            CREATE INDEX IF NOT EXISTS chl_pd_null ON EVENT_HISTORY_LOG 
            (PROCESS_DATE) WHERE PROCESS_DATE IS NULL;
            """,
            """
            -- Hit for query
            CREATE INDEX IF NOT EXISTS chl_ri_pd_null_asc ON EVENT_HISTORY_LOG 
            (RECORD_ID ASC, PROCESS_DATE) WHERE PROCESS_DATE IS NULL;	
            """,
            """
            ALTER TABLE EVENT_HISTORY_LOG
            SET (autovacuum_vacuum_scale_factor = 0.0);
            """,
            """ 
            ALTER TABLE EVENT_HISTORY_LOG
            SET (autovacuum_vacuum_threshold = 5000);
            """,
            """
            ALTER TABLE EVENT_HISTORY_LOG  
            SET (autovacuum_analyze_scale_factor = 0.0);
            """,
            """ 
            ALTER TABLE EVENT_HISTORY_LOG  
            SET (autovacuum_analyze_threshold = 5000);
            """,
            """ 
            REINDEX TABLE EVENT_HISTORY_LOG;
            """,
            """
            CREATE TABLE IF NOT EXISTS CREDENTIAL_LOG (
                RECORD_ID SERIAL PRIMARY KEY,
                SYSTEM_TYPE_CD VARCHAR(255) NOT NULL, 
                CREDENTIAL_TYPE_CD VARCHAR(255) NOT NULL,
                CREDENTIAL_ID VARCHAR(255) NOT NULL,
                SCHEMA_NAME VARCHAR(255) NOT NULL,
                SCHEMA_VERSION VARCHAR(255) NOT NULL,
                CREDENTIAL_JSON JSON NOT NULL,
                CREDENTIAL_HASH VARCHAR(64) NOT NULL, 
                ENTRY_DATE TIMESTAMP NOT NULL,
                PROCESS_DATE TIMESTAMP,
                PROCESS_SUCCESS CHAR,
                PROCESS_MSG VARCHAR(255)
            )
            """,
            """
            -- Hit duplicate credentials
            CREATE UNIQUE INDEX IF NOT EXISTS cl_hash_index ON CREDENTIAL_LOG 
            (CREDENTIAL_HASH);
            """,
            """
            -- Hit for counts and queries
            CREATE INDEX IF NOT EXISTS cl_pd_null ON CREDENTIAL_LOG 
            (PROCESS_DATE) WHERE PROCESS_DATE IS NULL;
            """,
            """
            -- Hit for query
            CREATE INDEX IF NOT EXISTS cl_ri_pd_null_asc ON CREDENTIAL_LOG 
            (RECORD_ID ASC, PROCESS_DATE) WHERE PROCESS_DATE IS NULL;
            """,
            """
            -- Hit for counts
            CREATE INDEX IF NOT EXISTS cl_ps ON CREDENTIAL_LOG
            (process_success)
            """,
            """
            -- Hit for queries
            CREATE INDEX IF NOT EXISTS cl_ps_pd_desc ON CREDENTIAL_LOG
            (process_success, process_date DESC)
            """,
            """
            ALTER TABLE CREDENTIAL_LOG
            SET (autovacuum_vacuum_scale_factor = 0.0);
            """,
            """ 
            ALTER TABLE CREDENTIAL_LOG
            SET (autovacuum_vacuum_threshold = 5000);
            """,
            """
            ALTER TABLE CREDENTIAL_LOG  
            SET (autovacuum_analyze_scale_factor = 0.0);
            """,
            """ 
            ALTER TABLE CREDENTIAL_LOG  
            SET (autovacuum_analyze_threshold = 5000);
            """,
            """ 
            REINDEX TABLE CREDENTIAL_LOG;
            """
            )
        cur = None
        try:
            cur = self.conn.cursor()
            for command in commands:
                cur.execute(command)
            self.conn.commit()
            cur.close()
            cur = None
        except (Exception, psycopg2.DatabaseError) as error:
            print(error)
            print(traceback.print_exc())
            raise
        finally:
            if cur is not None:
                cur.close()

    # record the last event processed
    def insert_processed_event(self, system_type, collection, object_date):
        """ insert a new event into the event table """
        sql = """INSERT INTO LAST_EVENT (SYSTEM_TYPE_CD, COLLECTION, OBJECT_DATE, ENTRY_DATE)
                 VALUES(%s, %s, %s, %s) RETURNING RECORD_ID;"""
        cur = None
        try:
            cur = self.conn.cursor()
            cur.execute(sql, (system_type, collection, object_date, datetime.datetime.now(),))
            _record_id = cur.fetchone()[0]
            self.conn.commit()
            cur.close()
            cur = None
        except (Exception, psycopg2.DatabaseError) as error:
            print(error)
            print(traceback.print_exc())
            raise
        finally:
            if cur is not None:
                cur.close()

    # get the id of the last event processed (of a specific collection)
    def get_last_processed_event(self, system_type, collection):
        cur = None
        try:
            cur = self.conn.cursor()
            cur.execute("""SELECT RECORD_ID, SYSTEM_TYPE_CD, COLLECTION, OBJECT_DATE, ENTRY_DATE
                           FROM LAST_EVENT where SYSTEM_TYPE_CD = %s and COLLECTION = %s
                           ORDER BY OBJECT_DATE desc""", (system_type, collection,))
            row = cur.fetchone()
            cur.close()
            cur = None
            event = None
            if row is not None:
                event = {}
                event['RECORD_ID'] = row[0]
                event['SYSTEM_TYPE_CD'] = row[1]
                event['COLLECTION'] = row[2]
                event['OBJECT_DATE'] = row[3]
                event['ENTRY_DATE'] = row[4]
            return event
        except (Exception, psycopg2.DatabaseError) as error:
            print(error)
            print(traceback.print_exc())
            raise
        finally:
            if cur is not None:
                cur.close()

    # get the last event processed timestamp
    def get_last_processed_event_date(self, system_type):
        cur = None
        try:
            cur = self.conn.cursor()
            cur.execute("""SELECT max(object_date) FROM LAST_EVENT where SYSTEM_TYPE_CD = %s""", (system_type,))
            row = cur.fetchone()
            cur.close()
            cur = None
            prev_event = row[0]
            return prev_event
        except (Exception, psycopg2.DatabaseError) as error:
            print(error)
            print(traceback.print_exc())
            raise
        finally:
            if cur is not None:
                cur.close()

    # insert data for one corp into the history table
    def insert_event_history_log(self, cur, system_type, collection, project_id, project_name, object_id, object_date, upload_date, upload_hash, process_date=None, process_success=None, process_msg=None):
        """ insert a new corps into the corps table """
        sql = """INSERT INTO EVENT_HISTORY_LOG 
                 (SYSTEM_TYPE_CD, COLLECTION, PROJECT_ID, PROJECT_NAME, OBJECT_ID, OBJECT_DATE, UPLOAD_DATE, UPLOAD_HASH, ENTRY_DATE,
                    PROCESS_DATE, PROCESS_SUCCESS, PROCESS_MSG)
                 VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s) RETURNING RECORD_ID;"""
        if process_date is None:
            process_date = datetime.datetime.now()
        if process_success is None:
            process_success = 'Y'
        if process_msg is None:
            process_msg = ''
        cur.execute(sql, (system_type, collection, project_id, project_name, object_id, object_date, upload_date, upload_hash, datetime.datetime.now(), process_date, process_success, process_msg))
        record_id = cur.fetchone()[0]
        return record_id

    # insert a generated JSON credential into our log
    def insert_json_credential(self, cur, system_cd, cred_type, cred_id, schema_name, schema_version, credential):
        sql = """INSERT INTO CREDENTIAL_LOG (SYSTEM_TYPE_CD, CREDENTIAL_TYPE_CD, CREDENTIAL_ID, 
                SCHEMA_NAME, SCHEMA_VERSION, CREDENTIAL_JSON, CREDENTIAL_HASH, ENTRY_DATE)
                VALUES(%s, %s, %s, %s, %s, %s, %s, %s) RETURNING RECORD_ID;"""
        # create row(s) for corp creds json info
        cred_json = json.dumps(credential, cls=CustomJsonEncoder, sort_keys=True)
        cred_hash = hashlib.sha256(cred_json.encode('utf-8')).hexdigest()
        try:
            cur.execute("savepoint save_" + cred_type)
            cur.execute(sql, (system_cd, cred_type, cred_id, schema_name, schema_version, cred_json, cred_hash, datetime.datetime.now()))
            return 1
        except Exception as e:
            # ignore duplicate hash ("duplicate key value violates unique constraint "cl_hash_index"")
            # re-raise all others
            stre = str(e)
            if "duplicate key value violates unique constraint" in stre and "cl_hash_index" in stre:
                print("Hash exception, skipping duplicate credential for corp:", cred_type, cred_id, e)
                cur.execute("rollback to savepoint save_" + cred_type)
                #print(cred_json)
                return 0
            else:
                print(traceback.print_exc())
                raise

    # store credentials for the provided corp
    def store_credentials(self, system_typ_cd, creds):
        cred_count = 0
        cur = None
        try:
            cur = self.conn.cursor()
            for cred in creds:
                cred_count = cred_count + self.insert_json_credential(cur, system_typ_cd, cred['cred_type'], cred['id'], 
                                                        cred['schema'], cred['version'], cred['attributes'])
            self.conn.commit()
            cur.close()
            cur = None
            return cred_count
        except (Exception, psycopg2.DatabaseError) as error:
            print(error)
            print(traceback.print_exc())
            raise
        finally:
            if cur is not None:
                cur.close()


    def random_string(self, N):
        return ''.join(random.SystemRandom().choice(string.ascii_uppercase + string.digits + ' ') for _ in range(N))


    def generate_relationship_credential(self, topic_name, topic_value, related_topic_name, related_topic_value, relationship, cred):
        for attr_name, value in cred['attributes'].items():
            if attr_name == topic_name:
                attr_value = topic_value
            elif attr_name == related_topic_name:
                attr_value = related_topic_value
            elif attr_name == 'relationship' or attr_name == 'relationship_description':
                attr_value = relationship
            elif value == '$UUID':
                attr_value = str(uuid.uuid4())
            elif value == '$Name':
                attr_value = 'Random Name ' + self.random_string(10)
            elif value == '$Text':
                attr_value = 'Random Text ' + self.random_string(25)
            elif value == '$Date':
                if attr_name == 'expiry_date':
                    attr_value = ''
                else:
                    attr_value = '2019-01-01'
            elif value == '$Select':
                attr_value = 'OPT1'
            else:
                attr_value = value
            cred['attributes'][attr_name] = attr_value

        cred['id'] = str(uuid.uuid4())
        cred['cred_type'] = cred['schema'].replace('.', '').replace('-', '').replace('-', '')

        return cred

    def generate_credential(self, topic_name, creds_template, topics):
        creds = []
        for j in range(len(topics)):
            topic_value = topics[j]
            for cred_template in creds_template:
                if cred_template['schema'].startswith('my-relationship'):
                    # special case handling for relationships
                    if j == 0:
                        # create "Owns" relationships
                        for k in range(len(topics)-1):
                            cred = copy.deepcopy(cred_template)
                            cred = self.generate_relationship_credential(topic_name, topic_value, "associated_corp_num", topics[1+k], "Owns", cred)
                            creds.append(cred)

                    else:
                        # create a "DBA" relationship
                        cred = copy.deepcopy(cred_template)
                        cred = self.generate_relationship_credential(topic_name, topic_value, "associated_corp_num", topics[0], "DBA", cred)
                        creds.append(cred)

                else:
                    cred = copy.deepcopy(cred_template)
                    for attr_name, value in cred['attributes'].items():
                        if attr_name == topic_name:
                            attr_value = topic_value
                        elif value == '$UUID':
                            attr_value = str(uuid.uuid4())
                        elif value == '$Name':
                            attr_value = 'Random Name ' + self.random_string(10)
                        elif value == '$Text':
                            attr_value = 'Random Text ' + self.random_string(25)
                        elif value == '$Date':
                            if attr_name == 'expiry_date':
                                attr_value = ''
                            else:
                                attr_value = '2019-01-01'
                        elif value == '$Select':
                            attr_value = 'OPT1'
                        else:
                            attr_value = value
                        cred['attributes'][attr_name] = attr_value

                    cred['id'] = str(uuid.uuid4())
                    cred['cred_type'] = cred['schema'].replace('.', '').replace('-', '').replace('-', '')

                    creds.append(cred)

        self.store_credentials(PIPELINE_SYSTEM_TYPE, creds)

        return creds


    # main entry point for data processing and credential generation job
    # this is just a "faux" method that creates some dummy credentials
    # in real life would pull data from a source DB
    def process_event_queue(self):
        """
        Generate some sample credentials, based on a template like:
        sample_creds_template = [
            {
                "attributes": {
                    "address_line_1": "$Text",
                    "addressee": "$Text",
                    "city": "$Text",
                    "corp_num": "$UUID",
                    "country": "$Text",
                    "effective_date": "$Date",
                    "entity_name": "$Name",
                    "entity_name_effective": "$Date",
                    "entity_status": "$Select",
                    "entity_status_effective": "$Date",
                    "entity_type": "$Text",
                    "expiry_date": "$Date",
                    "postal_code": "$Text",
                    "province": "$Text",
                    "registered_jurisdiction": "$Text",
                    "registration_date": "$Date"
                },
                "schema": "ian-registration.ian-ville",
                "version": "1.0.0"
            },
            {
                "attributes": {
                    "corp_num": "$Text",
                    "effective_date": "$Date",
                    "entity_name": "$Name",
                    "permit_id": "$UUID",
                    "permit_issued_date": "$Date",
                    "permit_status": "$Select",
                    "permit_type": "$Select"
                },
                "schema": "ian-permit.ian-ville",
                "version": "1.0.0"
            }
        ]
        """
        topic_name = 'corp_num'
        with open ("./gen-data.json", "r") as myfile:
            sample_creds_template_str = myfile.read().replace('\n', '')
        sample_creds_template = json.loads(sample_creds_template_str)

        # generate and save some dummy credentials
        count = 0
        for i in range(GEN_TOPIC_COUNT//5):
            topics = []
            for j in range(5):
                topics.append(str(uuid.uuid4()))
            creds = self.generate_credential(topic_name, sample_creds_template, topics)
            count = count + len(creds)
        print("Generated cred count = ", count)


    # main entry point for processing status job
    def display_event_processing_status(self):
        tables = ['event_history_log', 'credential_log']

        for table in tables:
            process_ct     = self.get_record_count(table, False)
            outstanding_ct = self.get_record_count(table, True)
            print('Table:', table, 'Processed:', process_ct, 'Outstanding:', outstanding_ct)

            sql = "select count(*) from " + table + " where process_success = 'N'"
            error_ct = self.get_sql_record_count(sql)
            print('      ', table, 'Process Errors:', error_ct)
            if 0 < error_ct:
                self.print_processing_errors(table)

    def get_outstanding_corps_record_count(self):
        return self.get_record_count('event_by_corp_filing')
        
    def get_outstanding_creds_record_count(self):
        return self.get_record_count('credential_log')
        
    def get_record_count(self, table, unprocessed=True):
        sql_ct_select = 'select count(*) from'
        sql_corp_ct_processed   = 'where process_date is not null'
        sql_corp_ct_outstanding = 'where process_date is null'

        if table == 'credential_log':
            sql_corp_ct_processed = sql_corp_ct_processed

        sql = sql_ct_select + ' ' + table + ' ' + (sql_corp_ct_outstanding if unprocessed else sql_corp_ct_processed)

        return self.get_sql_record_count(sql)

    def get_sql_record_count(self, sql):
        cur = None
        try:
            cur = self.conn.cursor()
            cur.execute(sql)
            ct = cur.fetchone()[0]
            cur.close()
            cur = None
            return ct
        except (Exception, psycopg2.DatabaseError) as error:
            print(error)
            print(traceback.print_exc())
            raise
        finally:
            if cur is not None:
                cur.close()
            cur = None

    def print_processing_errors(self, table):
        sql = """select * from """ + table + """
                 where process_success = 'N'
                 order by process_date DESC
                 limit 20"""
        rows = self.get_sql_rows(sql)
        print("       Recent errors:")
        print(rows)

    def get_sql_rows(self, sql):
        cursor = None
        try:
            cursor = self.conn.cursor()
            cursor.execute(sql)
            desc = cursor.description
            column_names = [col[0] for col in desc]
            rows = [dict(zip(column_names, row))  
                for row in cursor]
            cursor.close()
            cursor = None
            return rows
        except (Exception, psycopg2.DatabaseError) as error:
            print(error)
            print(traceback.print_exc())
            raise
        finally:
            if cursor is not None:
                cursor.close()
            cursor = None
