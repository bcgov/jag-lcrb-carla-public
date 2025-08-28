#!/bin/sh

# Script to process nginx.conf.template with environment variables
# This can be used in container startup or build process

echo "---> Replacing Configuration ..."

# Default values
REAL_IP_FROM=${REAL_IP_FROM:-"127.0.0.1/32"}
IP_FILTER_RULES=${IP_FILTER_RULES:-""}
API_CONFIG_SECTION=${API_CONFIG_SECTION:-""}
EXTRA_CONFIG=${EXTRA_CONFIG:-""}
HTTP_BASIC=${HTTP_BASIC:-""}
ADDITIONAL_REAL_IP_FROM_RULES=${ADDITIONAL_REAL_IP_FROM_RULES:-""}
API_URL=${API_URL:-""}

# Log the configuration being applied
echo "Setting:"
echo "REAL_IP_FROM = ${REAL_IP_FROM}"
echo "IP_FILTER_RULES = ${IP_FILTER_RULES}"
echo "API_CONFIG_SECTION = ${API_CONFIG_SECTION}"
echo "EXTRA_CONFIG = ${EXTRA_CONFIG}"
echo "HTTP_BASIC = ${HTTP_BASIC}"
echo "ADDITIONAL_REAL_IP_FROM_RULES = ${ADDITIONAL_REAL_IP_FROM_RULES}"
echo "API_URL = ${API_URL}"

# Process template and write to tmp, then use it as the main config
envsubst '${REAL_IP_FROM} ${IP_FILTER_RULES} ${API_CONFIG_SECTION} ${EXTRA_CONFIG} ${HTTP_BASIC} ${ADDITIONAL_REAL_IP_FROM_RULES} ${API_URL}' \
  < /etc/nginx/nginx.conf.template > /tmp/nginx.conf

echo "---> Starting nginx ..."

# Start nginx with our processed config
exec nginx -c /tmp/nginx.conf -g "daemon off;"
