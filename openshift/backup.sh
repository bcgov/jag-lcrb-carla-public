# bash script used to backup from OpenShift to the file system.

SETTINGSFOLDER=$(dirname $0)


if [ -f ${SETTINGSFOLDER}/settings.sh ]; then
  . ${SETTINGSFOLDER}/settings.sh
else
  echo "No settings.sh file found - please run this script from the base openshift folder in the project repository."
  exit 1
fi

TOOLSPROJECT=${PROJECT_NAMESPACE}-tools
PRODPROJECT=${PROJECT_NAMESPACE}-prod

# backup builds

oc project ${TOOLSPROJECT} 

for component in ${components}; do
  echo -e \\n"Backing up component ${component} ..."\\n    
	pushd ../${component}/openshift >/dev/null
	BUILDFILE=${component}-build.json
	oc get is/${component} -o json >${BUILDFILE}
	echo "," >>${BUILDFILE}
	oc get bc/${component} -o json >>${BUILDFILE}	
	popd >/dev/null
done

