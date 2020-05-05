#
# Copyright 2017-2018 Government of Canada - Public Services and Procurement Canada - buyandsell.gc.ca
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#

# Generate passwd file based on current uid
# This is used in OpenShift to create an entry in the /etc/passwd file
# for the randomly assigned user context of the container.
#
# In order for this to work your Docker file must grant write access to the /etc/passwd file
# to the root group using `chmod g+rw /etc/passwd`
#
# This is required by Flask, actually `werkzeug`, when it it calls getpwuid();
#   File "/data-pipeline/.venv/lib/python3.6/site-packages/werkzeug/debug/__init__.py", line 148, in get_pin_and_cookie_name
#     username = getpass.getuser()
#   File "/usr/lib/python3.6/getpass.py", line 169, in getuser
#     return pwd.getpwuid(os.getuid())[0]
#   KeyError: 'getpwuid(): uid not found: 1002170000'
#
# This is a known issue with Python on OpenShift; https://bugs.python.org/issue10496
#
# References:
# - https://github.com/openshift/jenkins/blob/master/2/contrib/jenkins/jenkins-common.sh
function generate_passwd_file() {
  USER_ID=$(id -u)
  GROUP_ID=$(id -g)
  if [ x"$USER_ID" != x"0" -a x"$USER_ID" != x"1001" ]; then
    echo "default:x:${USER_ID}:${GROUP_ID}:Default Application User:${HOME}:/usr/sbin/nologin" >> /etc/passwd
  fi
}

function start_cron_jobs() {
  echo "Starting go-crond as a background task ..."
  CRON_CMD="go-crond -v --allow-unprivileged --include=${CRON_FOLDER}"
  exec ${CRON_CMD} &
}

export HOST_IP=${HOST_IP:-0.0.0.0}
export HOST_PORT=${HOST_PORT:-5000}
export CRON_FOLDER=${CRON_FOLDER:-"/data-pipeline/scripts/cron"}

CMD="$@"
if [ -z "$CMD" ]; then 
  if [ ! -z "$START_CRON" ]; then
    start_cron_jobs
  fi
  echo "Initializing Mara ..."
  generate_passwd_file
  make
  source .venv/bin/activate
  CMD="flask run --host=${HOST_IP} --port=${HOST_PORT} --with-threads --reload --eager-loading"
fi

echo "Starting server ..."
exec $CMD