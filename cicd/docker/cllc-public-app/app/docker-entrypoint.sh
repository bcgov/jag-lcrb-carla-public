#!/bin/bash

# Script to process nginx.conf.template with environment variables
# This can be used in container startup or build process

# Default values
REAL_IP_FROM=${REAL_IP_FROM:-"127.0.0.1"}
IP_FILTER_RULES=${IP_FILTER_RULES:-""}
API_CONFIG_SECTION=${API_CONFIG_SECTION:-""}
EXTRA_CONFIG=${EXTRA_CONFIG:-""}
HTTP_BASIC=${HTTP_BASIC:-""}
ADDITIONAL_REAL_IP_FROM_RULES=${ADDITIONAL_REAL_IP_FROM_RULES:-""}

# Process template
envsubst '${REAL_IP_FROM} ${IP_FILTER_RULES} ${API_CONFIG_SECTION} ${EXTRA_CONFIG} ${HTTP_BASIC} ${ADDITIONAL_REAL_IP_FROM_RULES}' \
  < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf

# Start nginx
exec nginx -g "daemon off;"
