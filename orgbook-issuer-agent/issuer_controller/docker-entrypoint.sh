#!/bin/bash

export APP_NAME=${APP_NAME:-app}
export APP_MODULE=${APP_MODULE:-src.${APP_NAME}:${APP_NAME}}
export HOST_IP=${HOST_IP:-0.0.0.0}
export HOST_PORT=${HOST_PORT:-5000}
export WORKER_CONNECTIONS=${WORKER_CONNECTIONS:-1000}
export WORKER_CLASS=${WORKER_CLASS:-gevent}

CMD="$@"
if [ -z "$CMD" ]; then
  CMD="gunicorn -k ${WORKER_CLASS} --worker-connections ${WORKER_CONNECTIONS} --bind ${HOST_IP}:${HOST_PORT} ${APP_MODULE}"
fi

echo "Starting server ..."
exec $CMD