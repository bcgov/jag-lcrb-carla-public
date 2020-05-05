#!/bin/bash
export MSYS_NO_PATHCONV=1
SCRIPT_HOME="$( cd "$( dirname "$0" )" && pwd )"

# =================================================================================================================
# Usage:
# -----------------------------------------------------------------------------------------------------------------
usage () {
  cat <<-EOF

  Allows you to manage certain aspects of the environment.

  Usage:
    $0 [options] [commands]

  Example:
    $0 -e dev resetEventDatabase
    - This will drop and recreate the event database in the DEV environment.

  Options:
  ========
    -s <ResourceSuffix>; The suffix used to identify the resource instances.
      - Defaults to '${resourceSuffix}'

  Commands:
  ========
    reset
      - Reset the environment.
        The following operations are performed during a reset:
          - All credentials are be requeued for posting.
          - The 'event-processor-log-db' is reset and reinitialized.
          - The wallet is reset, by restoring initial copy of ICIA Wallet.
          - The process pauses at the end to confirm the corresponding ICOB reset process has completed
            before scalling up the controller which performs its issuer registration on startup.
          - Various pods are scaled up and down automatically during the process.

    resetEventDatabase
      - Drop and recreate the event database.

    resetEventProcLogDatabase
      - Drop and recreate the event processor log database.

    dropDatabase <dbPodName/> [<dbName/>]
      - Drop a specific database from a pod.

      <dbPodName/> - The friendly name of the pod.
      [<dbName/>] - Optional - The name of the database to drop. Defaults to '${POSTGRESQL_DATABASE}'

    deleteDatabase
      - Deletes all databases off a pod and recycles the pod leaving it in a clean state.

    requeueFailedEvents
      - Requeue any events (event_by_corp_filing records) that have failed processing.

    requeueFailedCorps
      - Requeue any corporations (corp_history_log records) that have failed processing.

    requeueFailedCreds
      - Requeue any credentials (credential_log records) that have failed posting.

    requeueProcessedCreds
      - Use with caution.  Requeue any credentials that have already been posted.

    getPipelineStatus
      - Get the pipeline status for the given environment.
        Runs the './run-step.sh bcreg/display_pipeline_status.py' pipeline script
        on an instance of a event-processor pod.

    getFailedCredErrors
      - Gets a summary of the errors encounters during credential posting, a summary of the
        error messages reported by getPipelineStatus.
      - Error messages are grouped together with a count of the occurances, plus a total.

    getRunningProcesses
      - Get a list of running processes running on a pod.
        Runs 'ps -v' on the pod.

    getDbDiskUsage <podName/>
      - Get the disk usage information for a given database pod.
        For example;
          $0 -e dev getDbDiskUsage wallet-indy-cat

    getCredentialPostRate [<periodInSeconds/>]
      - Calculates the rate at which credentials are being posted.
      - Expects the './run-step.sh bcreg/bc_reg_pipeline_post_credentials.py' pipeline to be running.
      - Calculations are based on the number of credentials that have been sucessfully posted over the
        specified period of time.

      periodInSeconds - Optional
        - Calculates the rate at which credentials were posted over the last number of seconds.
        - If left blank the period is calculated based on the how long the
          './run-step.sh bcreg/bc_reg_pipeline_post_credentials.py' has been running.

      Examples:
        $0 -e dev getCredentialPostRate
          - Get the average rate since the pipeline was started.

        $0 -e dev getCredentialPostRate 600
          - Get the average rate over the last 10 minutes.

    getCredentialStageRate  [<periodInSeconds/>]
      - Calculates the rate at which credentials are being staged.
      - Expects the './run-step.sh bcreg/bc_reg_pipeline_initial_load.py' pipeline to be running.
      - Calculations are based on the number of credentials that have been staged over the
        specified period of time.

      periodInSeconds - Optional
        - Calculates the rate at which credentials were staged over the last number of seconds.
        - If left blank the period is calculated based on the how long the
          './run-step.sh bcreg/bc_reg_pipeline_initial_load.py' has been running.

      Examples:
        $0 -e dev getCredentialStageRate
          - Get the average rate since the pipeline was started.

        $0 -e dev getCredentialStageRate 600
          - Get the average rate over the last 10 minutes.

    listDatabases <podName/>
      - List the databases hosted on a given postgresql pod instance.
      Example;
        $0 -e dev listdatabases wallet

    getConnections <podName/>
      - List database connection statistics for a given postgresql pod instance.
      Example;
        $0 -e dev getconnections wallet

    getRecordCounts <podName/> [<databaseName/>]
      - Gets a list of tables and the total number of record in each table.
        Examples;
          $0 -e dev getrecordcounts wallet agent_indy_cat_wallet
            - Get the record counts for the 'agent_indy_cat_wallet' database off the 'wallet' pod.

          $0 -e dev getrecordcounts event-db
          - Get the record counts for the '${POSTGRESQL_DATABASE}' (the pod's default database) database off the 'event-db' pod.

    listBuildRefs
      - Lists build configurations and their git references in a convenient column format.

    initialLoadCreds
      - Runs the 'bcreg/bc_reg_pipeline_initial_load.py' pipeline on the event-processor container.

    postCreds
      - Runs the 'bcreg/bc_reg_pipeline_post_credentials.py' pipeline on the event-processor container.

    removeAgentConnections
      - Remove all agent connections.

    scaleUp
      - Scale up one or more pods.
        For example;
          $0 -e dev scaleUp mara

    scaleDown
      - Scale down one or more pods.
        For example;
          $0 -e dev scaleDown mara

    recycle
      - Recycle one or more pods.
        For example;
          $0 -e dev recycle mara

EOF
}

# -----------------------------------------------------------------------------------------------------------------
# Defaults:
# -----------------------------------------------------------------------------------------------------------------
resourceSuffix="${resourceSuffix:--indy-cat}"
# -----------------------------------------------------------------------------------------------------------------

# =================================================================================================================
# Process the local command line arguments and pass everything else along.
# - The 'getopts' options string must start with ':' for this to work.
# -----------------------------------------------------------------------------------------------------------------
while [ ${OPTIND} -le $# ]; do
  if getopts :s: FLAG; then
    case ${FLAG} in
      # List of local options:
      s ) resourceSuffix=$OPTARG ;;

      # Pass unrecognized options ...
      \?) pass+=" -${OPTARG}" ;;
    esac
  else
    # Pass unrecognized arguments ...
    pass+=" ${!OPTIND}"
    let OPTIND++
  fi
done

# Pass the unrecognized arguments along for further processing ...
shift $((OPTIND-1))
set -- "$@" $(echo -e "${pass}" | sed -e 's/^[[:space:]]*//')
# =================================================================================================================

# -----------------------------------------------------------------------------------------------------------------
# Define hook scripts:
# - These must be defined before the main settings script 'settings.sh' is loaded.
# -----------------------------------------------------------------------------------------------------------------
onRequiredOptionsExist() {
  (
    if [ -z "${DEPLOYMENT_ENV_NAME}" ]; then
      _red='\033[0;31m'
      _nc='\033[0m' # No Color
          echo -e "\n${_red}You MUST specify an environment name using the '-e' flag.${_nc}"
          echo -e "${_red}Assuming a default would have unwanted consequences.${_nc}\n"
          return 1
        else
          return 0
    fi
  )
}

onUsesCommandLineArguments() {
  (
    # This script is expecting command line arguments to be passed ...
    return 0
  )
}

# -----------------------------------------------------------------------------------------------------------------
# Initialization:
# -----------------------------------------------------------------------------------------------------------------
# Load the project settings and functions ...
_includeFile="ocFunctions.inc"
_settingsFile="settings.sh"
if [ ! -z $(type -p ${_includeFile}) ]; then
  _includeFilePath=$(type -p ${_includeFile})
  export OCTOOLSBIN=$(dirname ${_includeFilePath})

  if [ -f ${OCTOOLSBIN}/${_settingsFile} ]; then
    . ${OCTOOLSBIN}/${_settingsFile}
  fi

  if [ -f ${OCTOOLSBIN}/${_includeFile} ]; then
    . ${OCTOOLSBIN}/${_includeFile}
  fi
else
  _red='\033[0;31m'
  _yellow='\033[1;33m'
  _nc='\033[0m' # No Color
  echo -e \\n"${_red}${_includeFile} could not be found on the path.${_nc}"
  echo -e "${_yellow}Please ensure the openshift-developer-tools are installed on and registered on your path.${_nc}"
  echo -e "${_yellow}https://github.com/BCDevOps/openshift-developer-tools${_nc}"
fi

# -----------------------------------------------------------------------------------------------------------------
# Functions:
# -----------------------------------------------------------------------------------------------------------------
function resetDatabase() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\nresetDatabase; You MUST specify a pod name.\n"
    exit 1
  fi

  printAndAskToContinue "If you contiune the ${_podName}${resourceSuffix} database will be dropped and recreated.  All data will be lost."
  dropAndRecreatePostgreSqlDatabase ${_podName}${resourceSuffix}
  echoWarning "\nThe ${_podName}${resourceSuffix} database has been dropped and recreated."
}

function resetEventDatabase() {
    _dbPodName=${1}
    _eventProcessorPodName=${2}
    if [ -z "${_dbPodName}" ] || [ -z "${_eventProcessorPodName}" ]; then
      echoError "\nresetEventDatabase; You MUST specify the name of the event-db and event-processor pods.\n"
      exit 1
    fi

    resetDatabase "${_dbPodName}"
    runInContainer -v "${_eventProcessorPodName}${resourceSuffix}" 'cd scripts && ./run-step.sh bcreg/bc_reg_migrate.py'
}

function resetEventProcLogDatabase() {
  (
    local OPTIND
    unset local quiet
    unset local flags
    while getopts q FLAG; do
      case $FLAG in
        q )
          local quiet=1
          local flags="-a"
          ;;
      esac
    done
    shift $((OPTIND-1))

    _dbPodName=${1}
    _eventProcessorPodName=${2}
    if [ -z "${_dbPodName}" ] || [ -z "${_eventProcessorPodName}" ]; then
      echoError "\nresetEventProcLogDatabase; You MUST specify the name of the event-processor-log-db and event-processor pods.\n"
      exit 1
    fi

    if [ -z "${quiet}" ]; then
      printAndAskToContinue "If you contiune the ${_dbPodName}${resourceSuffix} database will be dropped and recreated.  All data will be lost."
    fi

    dropAndRecreateDatabaseWithMigrations ${flags} "${_eventProcessorPodName}${resourceSuffix}" "${_dbPodName}${resourceSuffix}"
    echoWarning "\nThe ${_dbPodName}${resourceSuffix} database has been dropped and recreated."
  )
}

function deleteDatabase() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\ndeleteDatabase; You MUST specify a pod name.\n"
    exit 1
  fi

  printAndAskToContinue "If you contiune all of the databases on ${_podName}${resourceSuffix} will be deleted.  All data will be lost."
  deleteAndRecreateDatabase ${_podName}${resourceSuffix}
  echoWarning "\nThe databases on ${_podName}${resourceSuffix} have been deleted."
}

function reset() {
  (
    agentPod=${1}
    controllerPod=${2}
    eventDbPod=${3}
    eventProcessorPod=${4}
    eventProcessorLogDbPod=${5}
    walletDbPod=${6}
    backupPod=${7}
    walletDbName=${8} # Example; agent_indy_cat_wallet
    walletDbBackupSpec=${9} # Example; "wallet-indy-cat:5432/${walletDbName}"
    walletDbBackupFileFilter=${10} # Example; /backups/initialized-wallet
    walletDbAdminPasswordKey=${11}
    targetNamespace=$(getProjectName)

    if (( $# < 10 )); then
      echo -e \\n"reset; Missing parameter!"\\n
      exit 1
    fi

    # Explain what is about to happen and wait for confirmation ...
txtMsg=$(cat <<-EOF
The [${targetNamespace}] ICIA environment will be RESET using the following settings:
  - agentPod: ${agentPod}${resourceSuffix}
  - controllerPod: ${controllerPod}${resourceSuffix}
  - eventDbPod: ${eventDbPod}${resourceSuffix}
  - eventProcessorPod: ${eventProcessorPod}${resourceSuffix}
  - eventProcessorLogDbPod: ${eventProcessorLogDbPod}${resourceSuffix}
  - walletDbPod: ${walletDbPod}${resourceSuffix}
  - backupPod: ${backupPod}${resourceSuffix}
  - walletDbName: ${walletDbName}
  - walletDbBackupSpec: ${walletDbBackupSpec}
  - walletDbBackupFileFilter: ${walletDbBackupFileFilter}
  - walletDbAdminPasswordKey: ${walletDbAdminPasswordKey}

The following operations will be performed:
  - All credentials will be requeued for posting.
  - The 'event-processor-log-db' will be reset and reinitialized.
  - The wallet will be reset, by restoring initial copy of ICIA Wallet.
  - The process will pause at the end to confirm the corresponding ICOB reset process has completed
    before scalling up the controller which will perform its issuer registration on startup.
  - Various pods will be scaled up and down automatically during the process.\n
EOF
)
    printAndAskToContinue "${txtMsg}"

    # - scaledown ICIA controller and agent
    echo "Scaling down ${agentPod}${resourceSuffix} and ${controllerPod}${resourceSuffix} ..."
    scaleDown -w "${controllerPod}${resourceSuffix}" "${agentPod}${resourceSuffix}"
    exitOnError

    # Nonblocking - Indicate the ICOB reset could safely start now ...
    echoWarning "\n================================================================================="
    echoWarning "*********************************************************************************"
    echo
    echoWarning "You can now safely start the reset process in the corresponding ICOB environment."
    echo
    echoWarning "*********************************************************************************"
    echoWarning "=================================================================================\n\n"

    # - requeue all credentials - This can take a while
    echoWarning "Requeuing all credentials; this could take some time ..."
    requeueFailedCreds "${eventDbPod}"
    exitOnError
    requeueProcessedCreds "${eventDbPod}"
    exitOnError

    # - reset ICIA event-processor-log-db
    echo -e "\nResetting ${eventProcessorLogDbPod}${resourceSuffix} ..."
    resetEventProcLogDatabase -q "${eventProcessorLogDbPod}" "${eventProcessorPod}"
    exitOnError

    # - reset ICIA Wallet Database, by restoring initial copy of ICIA Wallet.
    echo -e "\nResetting ${walletDbPod}${resourceSuffix} ..."
    if isScaledUp ${backupPod}${resourceSuffix}; then
      local backupStarted=1
    else
      local unset backupStarted
      scaleUp -w "${backupPod}${resourceSuffix}"
      exitOnError
    fi

    runInContainer -i \
      ${backupPod}${resourceSuffix} \
      "./backup.sh -s -a $(getSecret ${walletDbPod}${resourceSuffix} ${walletDbAdminPasswordKey}) -r ${walletDbBackupSpec} -f ${walletDbBackupFileFilter}"
    exitOnError

    if [ -z ${backupStarted} ]; then
      # Leave the backup container in the same state we found it.
      scaleDown "${backupPod}${resourceSuffix}"
      exitOnError
    fi

    # - verify ICIA Wallet - There should only be 4 items.
    recordCounts=$(getRecordCounts "${walletDbPod}" "${walletDbName}")
    numItems=$(echo "${recordCounts}" | grep items | awk '{print $5}')
    if (( ${numItems} >= 4 )); then
      echo "Wallet 'items' count verified; ${numItems} items found."
    else
      echoError "Wallet 'items' count verification failed; ${numItems} items found.  Please fix the issue and try again."
      exit 1
    fi
    exitOnError

    # - Pause for ICOB reset
    printAndAskToContinue "Please ensure the ICOB reset is complete before you continue."

    # - scaleup ICIA agent
    echo "Scaling up ${agentPod}${resourceSuffix} ..."
    scaleUp -w "${agentPod}${resourceSuffix}"
    exitOnError

    # - Reset all agent connections ...
    echo "Resetting agent connections ..."
    removeAgentConnections "${agentPod}"
    exitOnError

    # - scaleup ICIA controller
    echo "Scaling up ${controllerPod}${resourceSuffix} ..."
    scaleUp -w "${controllerPod}${resourceSuffix}"
    exitOnError

    # - verify ICIA registered with ICOB
    # - Test posting a few credentials using the pipelines
    echo "You can now verifiy ICIA has successfully registered with ICOB.  Following that you can test things by issuing a few credentials."
  )
}

function requeueProcessedCreds() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\nrequeueProcessedCreds; You MUST specify a pod name.\n"
    exit 1
  fi

  echo
  runInContainer -v \
    ${_podName}${resourceSuffix} \
    'psql -d ${POSTGRESQL_DATABASE} -ac "update credential_log set process_success = null, process_date = null, process_msg = null where process_success = '"'"'Y'"'"';"'
}

function requeueFailedEvents() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\nrequeueFailedEvents; You MUST specify a pod name.\n"
    exit 1
  fi

  echo
  runInContainer -v \
    ${_podName}${resourceSuffix} \
    'psql -d ${POSTGRESQL_DATABASE} -ac "update event_by_corp_filing set process_success = null, process_date = null, process_msg = null where process_success = '"'"'N'"'"';"'
}

function requeueFailedCorps() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\nrequeueFailedCorps; You MUST specify a pod name.\n"
    exit 1
  fi

  # Get affected corp_nums
  corp_nums=$(runInContainer \
    ${_podName}${resourceSuffix} \
    'psql -d ${POSTGRESQL_DATABASE} -t -c "select corp_num from corp_history_log where process_success = '"'"'N'"'"';"')
  corp_nums=$(echo "${corp_nums}" | sed -e "s~[[:space:]]*\(.*\)~'\1'~" | awk -vORS=, '{ print $0 }' | sed 's~,$~\n~')

  # Remove affected credential_log records
  runInContainer -v \
    ${_podName}${resourceSuffix} \
    "psql -d "'${POSTGRESQL_DATABASE}'" -ac \"delete from credential_log where corp_num in (${corp_nums});\""

  # Remove affected corp_history_log records
  runInContainer -v \
    ${_podName}${resourceSuffix} \
    "psql -d "'${POSTGRESQL_DATABASE}'" -ac \"delete from corp_history_log where corp_num in (${corp_nums});\""

  # Requeue affected event_by_corp_filing records
  runInContainer -v \
    ${_podName}${resourceSuffix} \
    "psql -d "'${POSTGRESQL_DATABASE}'" -ac \"update event_by_corp_filing set process_success = null, process_date = null, process_msg = null where corp_num in (${corp_nums});\""
}

function requeueFailedCreds() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\nrequeueFailedCreds; You MUST specify a pod name.\n"
    exit 1
  fi

  echo
  runInContainer -v \
    ${_podName}${resourceSuffix} \
    'psql -d ${POSTGRESQL_DATABASE} -ac "update credential_log set process_success = null, process_date = null, process_msg = null where process_success = '"'"'N'"'"';"'
}

function getPipelineStatus() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\ngetPipelineStatus; You MUST specify a pod name.\n"
    exit 1
  fi

  echo
  runInContainer \
    ${_podName}${resourceSuffix} \
    'cd scripts && ./run-step.sh bcreg/display_pipeline_status.py'
}

function getRunningProcesses() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\ngetRunningProcesses; You MUST specify a pod name.\n"
    exit 1
  fi

  echo
  runInContainer \
    ${_podName}${resourceSuffix} \
    'ps -v'
}

function getCredentialPostRate() {
  _periodInSeconds=${1}
  _dbPodName=${2}
  _eventProcessorPodName=${3}
  if [ -z "${_dbPodName}" ] || [ -z "${_eventProcessorPodName}" ]; then
    echoError "\ngetCredentialPostRate; You MUST specify the name of the event-db and event-processor pods.\n"
    exit 1
  fi

  if [ -z ${_periodInSeconds} ]; then
    runningForSeconds=$(runInContainer "${_eventProcessorPodName}${resourceSuffix}" \
      "ps a | grep '/bin/sh ./run-step.sh bcreg/bc_reg_pipeline_post_credentials.py' | grep -v grep | awk '{print \$1}' | xargs ps -o etimes= -p")
  else
    runningForSeconds=${_periodInSeconds}
  fi

  startTime=$(TZ='UTC' date -d "now - ${runningForSeconds} seconds" +"%Y-%m-%d %H:%M:%S")
  credentialsPosted=$(runInContainer "${_dbPodName}${resourceSuffix}" \
    "psql -d bc_reg_db -t -c \"select count(*) from credential_log where process_success is not null AND process_success = 'Y' AND process_date >= '${startTime}';\"")

  if [ ! -z ${runningForSeconds} ]; then
    # Do floating point math ...
    postsPerSecond=$(awk "BEGIN {print (${credentialsPosted}/${runningForSeconds})}")
    postsPerMinute=$(awk "BEGIN {print (${postsPerSecond}*60)}")
    elapsedTime="$(($runningForSeconds/3600))h:$(($runningForSeconds%3600/60))m:$(($runningForSeconds%60))s"
  else
    echo "'/bin/sh ./run-step.sh bcreg/bc_reg_pipeline_post_credentials.py' is not currently running on ${_eventProcessorPodName}."
  fi

  # echo
  # echo "runningForSeconds: ${runningForSeconds}"
  # echo "elapsedTime: ${elapsedTime}"
  # echo "now: $(date -d "now" +"%Y-%m-%d %H:%M:%S")"
  # echo "startTime: ${startTime}"
  # echo "credentialsPosted: ${credentialsPosted}"
  # echo
  # echo "postsPerSecond: ${postsPerSecond}"
  # echo "postsPerMinute: ${postsPerMinute}"

  echo -e \\n"Credentials are being posted at an average rate of:"
  printf "  - %.0f per second, or\n" "${postsPerSecond}"
  printf "  - %.0f per minute\n" "${postsPerMinute}"
  echo "  - Over the period of ${elapsedTime}"
}

function getCredentialStageRate() {
  _periodInSeconds=${1}
  _dbPodName=${2}
  _eventProcessorPodName=${3}
  if [ -z "${_dbPodName}" ] || [ -z "${_eventProcessorPodName}" ]; then
    echoError "\ngetCredentialStageRate; You MUST specify the name of the event-db and event-processor pods.\n"
    exit 1
  fi

  if [ -z ${_periodInSeconds} ]; then
    runningForSeconds=$(runInContainer "${_eventProcessorPodName}${resourceSuffix}" \
      "ps a | grep '/bin/sh ./run-step.sh bcreg/bc_reg_pipeline_initial_load.py' | grep -v grep | awk '{print \$1}' | xargs ps -o etimes= -p")
  else
    runningForSeconds=${_periodInSeconds}
  fi

  startTime=$(TZ='UTC' date -d "now - ${runningForSeconds} seconds" +"%Y-%m-%d %H:%M:%S")
  credentialsPosted=$(runInContainer "${_dbPodName}${resourceSuffix}" \
    "psql -d bc_reg_db -t -c \"select count(*) from credential_log where process_success is null AND process_success is null AND entry_date >= '${startTime}';\"")

  if [ ! -z ${runningForSeconds} ]; then
    # Do floating point math ...
    postsPerSecond=$(awk "BEGIN {print (${credentialsPosted}/${runningForSeconds})}")
    postsPerMinute=$(awk "BEGIN {print (${postsPerSecond}*60)}")
    elapsedTime="$(($runningForSeconds/3600))h:$(($runningForSeconds%3600/60))m:$(($runningForSeconds%60))s"
  else
    echo "'/bin/sh ./run-step.sh bcreg/bc_reg_pipeline_initial_load.py' is not currently running on ${_eventProcessorPodName}."
  fi

  echo -e \\n"Credentials are being staged at an average rate of:"
  printf "  - %.0f per second, or\n" "${postsPerSecond}"
  printf "  - %.0f per minute\n" "${postsPerMinute}"
  echo "  - Over the period of ${elapsedTime}"
}

function getFailedCredErrors() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\ngetFailedCredErrors; You MUST specify a pod name.\n"
    exit 1
  fi

  echo
  failedCreds=$(runInContainer "${_podName}${resourceSuffix}" \
    'psql -d ${POSTGRESQL_DATABASE} -t -c "select count(*) from credential_log where process_success = '"'"'N'"'"';"')

  runInContainer \
    ${_podName}${resourceSuffix} \
    'psql -d ${POSTGRESQL_DATABASE} -c "select count(*), process_msg from credential_log where process_success = '"'"'N'"'"' group by process_msg;"'

  echo "Total: ${failedCreds}"
}

function postCreds() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\npostCreds; You MUST specify a pod name.\n"
    exit 1
  fi

  runInContainer -v -i \
    ${_podName}${resourceSuffix} \
    'cd scripts && ./run-step.sh bcreg/bc_reg_pipeline_post_credentials.py'
}

function initialLoadCreds() {
  _podName=${1}
  if [ -z "${_podName}" ]; then
    echoError "\ninitialLoadCreds; You MUST specify a pod name.\n"
    exit 1
  fi

  runInContainer -v -i \
    ${_podName}${resourceSuffix} \
    'cd scripts && ./run-step.sh bcreg/bc_reg_pipeline_initial_load.py'
}

function removeAgentConnections(){
  (
    _podName=${1}
    if [ -z "${_podName}" ]; then
      echoError "\nremoveAgentConnections; You MUST specify a pod name.\n"
      exit 1
    fi

    resonse=$(runInContainer \
      ${_podName}${resourceSuffix} \
      'curl -s -X GET -H "x-api-key:${AGENT_ADMIN_API_KEY}" http://localhost:8024/connections?alias=tob-agent')
    connectionIds=$(echo ${resonse} | jq -r '.results[].connection_id')
    echo

    for connectionId in ${connectionIds}; do
      echoWarning "Removing connection: ${connectionId}"
      runInContainer \
        ${_podName}${resourceSuffix} \
        "curl -s -o /dev/null -w \" - %{http_code}\n\" -X POST -H \"x-api-key:\${AGENT_ADMIN_API_KEY}\" http://localhost:8024/connections/${connectionId}/remove"
    done
    echo
  )
}
# =================================================================================================================

pushd ${SCRIPT_HOME} >/dev/null
_cmd=$(toLower ${1})
shift

case "${_cmd}" in
  reseteventdatabase)
    dbPodName=${1:-event-db}
    eventProcessorPodName=${2:-event-processor}
    resetEventDatabase "${dbPodName}" "${eventProcessorPodName}"
    ;;
  reseteventproclogdatabase)
    dbPodName=${1:-event-processor-log-db}
    eventProcessorPodName=${2:-event-processor}
    resetEventProcLogDatabase "${dbPodName}" "${eventProcessorPodName}"
    ;;
  deletedatabase)
    dbPodName=${1}
    deleteDatabase "${dbPodName}"
    ;;
  reset)
    agentPod=${1:-agent}
    controllerPod=${2:-bcreg-controller}
    eventDbPod=${3:-event-db}
    eventProcessorPod=${4:-event-processor}
    eventProcessorLogDbPod=${5:-event-processor-log-db}
    walletDbPod=${6:-wallet}
    backupPod=${7:-backup}
    walletDbName=${8:-"agent_indy_cat_wallet"}
    walletDbBackupSpec=${9:-"wallet-indy-cat:5432/${walletDbName}"}
    walletDbBackupFileFilter=${10:-"/backups/initialized-wallet"}
    walletDbAdminPasswordKey=${11:-"admin-password"}

    reset "${agentPod}" \
          "${controllerPod}" \
          "${eventDbPod}" \
          "${eventProcessorPod}" \
          "${eventProcessorLogDbPod}" \
          "${walletDbPod}" \
          "${backupPod}" \
          "${walletDbName}" \
          "${walletDbBackupSpec}" \
          "${walletDbBackupFileFilter}" \
          "${walletDbAdminPasswordKey}"
    ;;
  requeuefailedcreds)
    dbPodName=${1:-event-db}
    requeueFailedCreds "${dbPodName}"
    ;;
  requeuefailedevents)
    dbPodName=${1:-event-db}
    requeueFailedEvents "${dbPodName}"
    ;;
  requeuefailedcorps)
    dbPodName=${1:-event-db}
    requeueFailedCorps "${dbPodName}"
    ;;
  requeueprocessedcreds)
    dbPodName=${1:-event-db}
    requeueProcessedCreds "${dbPodName}"
    ;;
  getpipelinestatus)
    eventProcessorPodName=${1:-event-processor}
    getPipelineStatus "${eventProcessorPodName}"
    ;;
  getrunningprocesses)
    getRunningProcesses ${@}
    ;;
  getdbdiskusage)
    dbPodName=${1}
    getPostgreSqlDatabaseDiskUsage ${dbPodName}
    ;;
  getcredentialpostrate)
    periodInSeconds=${1}
    dbPodName=${2:-event-db}
    eventProcessorPodName=${3:-event-processor}
    getCredentialPostRate "${periodInSeconds}" "${dbPodName}" "${eventProcessorPodName}"
    ;;
  getcredentialstagerate)
    periodInSeconds=${1}
    dbPodName=${2:-event-db}
    eventProcessorPodName=${3:-event-processor}
    getCredentialStageRate "${periodInSeconds}" "${dbPodName}" "${eventProcessorPodName}"
    ;;
  listdatabases)
    dbPodName=${1}
    listDatabases "${dbPodName}"
    ;;
  getrecordcounts)
    if (( $# <= 1 )); then
      dbPodName=${1}
    else
      dbPodName=${1}
      databaseName=${2}
    fi
    getRecordCounts "${dbPodName}" "${databaseName}"
    ;;
  getconnections)
    dbPodName=${1}
    getConnections "${dbPodName}"
    ;;
  getfailedcrederrors)
    dbPodName=${1:-event-db}
    getFailedCredErrors "${dbPodName}"
    ;;
  listbuildrefs)
    listBuildRefs
    ;;
  postcreds)
    eventProcessorPodName=${1:-event-processor}
    postCreds "${eventProcessorPodName}"
    ;;
  initialcoadcreds)
    eventProcessorPodName=${1:-event-processor}
    initialLoadCreds "${eventProcessorPodName}"
    ;;
  dropdatabase)
    dbPodName=${1}
    dbName=${2}
    dropDatabase "${dbPodName}" "${dbName}"
    ;;
  removeagentconnections)
    agentPodName=${1:-agent}
    removeAgentConnections "${agentPodName}"
    ;;

  scaleup)
    scaleUp -w ${@}
    ;;
  scaledown)
    scaleDown -w ${@}
    ;;
  recycle)
    recyclePods -w ${@}
    ;;
  *)
    echoWarning "Unrecognized command; ${_cmd}"
    globalUsage
    ;;
esac

popd >/dev/null
