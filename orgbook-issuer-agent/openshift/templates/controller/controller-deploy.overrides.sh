_includeFile=$(type -p overrides.inc)
if [ ! -z ${_includeFile} ]; then
  . ${_includeFile}
else
  _red='\033[0;31m'; _yellow='\033[1;33m'; _nc='\033[0m'; echo -e \\n"${_red}overrides.inc could not be found on the path.${_nc}\n${_yellow}Please ensure the openshift-developer-tools are installed on and registered on your path.${_nc}\n${_yellow}https://github.com/BCDevOps/openshift-developer-tools${_nc}"; exit 1;
fi

# ======================================================
# Special Deployment Parameters needed for Deployment
# ------------------------------------------------------
# The results need to be encoded as OpenShift template
# parameters for use with oc process.
# ======================================================

if createOperation; then
  readParameter "CR_AGENT_ADMIN_URL - Please provide the URL for the credential registry admin api.\nThe default is an empty string:" CR_AGENT_ADMIN_URL "" "false"
  readParameter "CR_ADMIN_API_KEY - Please provide the key for the credential registry admin api.\nThe default is an empty string:" CR_ADMIN_API_KEY "" "false"
else
  # Secrets are removed from the configurations during update operations ...
  printStatusMsg "Update operation detected ...\nSkipping the prompts for the CR_AGENT_ADMIN_URL, and CR_ADMIN_API_KEY secrets ...\n"
  # Prompted
  writeParameter "CR_AGENT_ADMIN_URL" "generation_skipped" "false"
  writeParameter "CR_ADMIN_API_KEY" "generation_skipped" "false"
fi

SPECIALDEPLOYPARMS="--param-file=${_overrideParamFile}"
echo ${SPECIALDEPLOYPARMS}