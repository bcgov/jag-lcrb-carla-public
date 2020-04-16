#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE USER mara_db with password 'mara_db_pwd';
    CREATE DATABASE mara_db;
    GRANT ALL PRIVILEGES ON DATABASE mara_db TO mara_db;
EOSQL

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE USER pipeline_db with password 'pipeline_db_pwd';
    CREATE DATABASE pipeline_db;
    GRANT ALL PRIVILEGES ON DATABASE pipeline_db TO pipeline_db;
EOSQL

