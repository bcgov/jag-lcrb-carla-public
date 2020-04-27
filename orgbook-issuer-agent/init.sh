#/bin/bash

echo This script initializes some config data a run of a new VON Issuer/Verifier Agent.
echo Please answer the following questions and we will get things all set up for you.

while [[ -z "$ORG_TITLE" ]]; do
	read -p 'Please provide a descriptive title for your permit-issuing organization (e.g. City of Victoria): ' ORG_TITLE
done
while [[ -z "$MY_ORG" ]]; do
	read -p 'Please provide a domain for your permit-issuing organization - no spaces (e.g. city-of-victoria): ' MY_ORG
	MY_ORG=`echo ${MY_ORG// /} | xargs`
done
while [[ -z "$MY_PERMIT" ]]; do
	read -p 'Please provide the name of the permit your organization will issue - no spaces (e.g. museum-permit): ' MY_PERMIT
	MY_PERMIT=`echo ${MY_PERMIT// /} | xargs`
done

echo ""

# Generate the seed from MY_ORG, making sure it is 32 characters long
export MY_SEED=`echo ${MY_ORG}_00000000000000000000000000000000 | cut -c 1-32`

echo How will you be the VON Issuer/Verifier Agent:
echo
DEPLOY_OPTS=("Using Play with Docker in your browser"
			 "Using docker on your own machine - with local von-network and TheOrgBook instances"
			 "Some other way")

# Determine the example to expand and expand it
select example in "${DEPLOY_OPTS[@]}"; do
    case $REPLY in
        1 ) 
            if [ $PWD_HOST_FQDN == "labs.play-with-docker.com" ]
              then
                export ETH_CONFIG="eth1"
              elif [ $PWD_HOST_FQDN == "play-with-docker.vonx.io" ]
              then
                export ETH_CONFIG="eth0"
              else
                export ETH_CONFIG="eth0"
              fi
            myhost=`ifconfig ${ETH_CONFIG} | grep inet | cut -d':' -f2 | cut -d' ' -f1 | sed 's/\./\-/g'`
            export ENDPOINT_HOST="ip${myhost}-${SESSION_ID}-5001.direct.${PWD_HOST_FQDN}"
            export APPLICATION_URL=http://${ENDPOINT_HOST}
            export LEDGER=http://greenlight.bcovrin.vonx.io
            export LEDGER_URL=http://greenlight.bcovrin.vonx.io
            export GENESIS_URL=${LEDGER}/genesis
            __TOBAPIURL=https://demo-api.orgbook.gov.bc.ca/api/v2
            __TOBAPPURL=https://demo.orgbook.gov.bc.ca/en/home

            break;;
        2 ) 
            unset ENDPOINT_HOST
            export LEDGER=http://localhost:9000
            unset LEDGER_URL
            export GENESIS_URL=${LEDGER}/genesis
            __TOBAPIURL=http://tob-api:8080/api/v2
            __TOBAPPURL=http://localhost:8080

            # Adjustments to files for local execution
            sed -i.bak "s/ INDY_GENESIS_URL/ #INDY_GENESIS_URL/" issuer_controller/config/settings.yml
            # sed -i.bak "s/ AUTO_REGISTER_DID/ #AUTO_REGISTER_DID/" issuer_controller/config/settings.yml
            find docker issuer_controller -name "*.bak" -type f|xargs rm -f

            break;;
        3 ) 
            read -p "Enter the agent host you are using (e.g. localhost:5001): " __AGENTHOST
            export ENDPOINT_HOST=${__AGENTHOST}
            read -p "Enter the URL of the ledger you are using: " __LEDGER
            export LEDGER=${__LEDGER}
            export LEDGER_URL=${__LEDGER}
            export GENESIS_URL=${LEDGER}/genesis
            __TOBAPIURL=Update-With-OrgBook-API-URL
            __TOBAPPURL=Update-With-OrgBook-Application-URL
            echo NOTE: TheOrgBook API and Application URLs must be updated in issuer_controller/config/settings.yml

            break;;
    esac
done
echo ""

# OK - time to make all the substitutions...
sed -i.bak "s/my-organization_0000000000000000/${MY_SEED}/g" issuer_controller/config/settings.yml
sed -i.bak "s#TOBAPIURL#${__TOBAPIURL}#g" issuer_controller/config/settings.yml
sed -i.bak "s#TOBAPPURL#${__TOBAPPURL}#g" issuer_controller/config/settings.yml
sed -i.bak "s#GENESISURL#${GENESIS_URL}#g" issuer_controller/config/settings.yml
find issuer_controller/config -name "*.yml" -exec sed -i.bak "s/my-org-full-name/${ORG_TITLE}/g" {} +
find issuer_controller/config -name "*.yml" -exec sed -i.bak s/my-organization/${MY_ORG}/g {} +
find issuer_controller/config -name "*.yml" -exec sed -i.bak s/my-permit/${MY_PERMIT}/g {} +
find issuer_controller/config -name "*.json" -exec sed -i.bak s/my-organization/${MY_ORG}/g {} +
find issuer_controller/config -name "*.json" -exec sed -i.bak s/my-permit/${MY_PERMIT}/g {} +
find issuer_controller -name "*.bak" -type f|xargs rm -f
cp issuer_controller/config/gen-data.json issuer_pipeline/

# Register DID
# https://gist.github.com/subfuzion/08c5d85437d5d4f00e58
# echo ""
# echo Registering DID on Ledger ${LEDGER} - the Ledger MUST be running for this to work
# echo ""
# echo \{\"role\":\"TRUST_ANCHOR\",\"alias\":\"${MY_ORG}\",\"did\":null,\"seed\":\"${MY_SEED}\"\} >tmp.json
# MY_DID=`curl -s -d "@tmp.json" -X POST ${LEDGER}/register | awk -F'"' '/did/ { print $4 }'`
# echo My DID was registered as: $MY_DID
# rm tmp.json
# echo ""

# Update the MY-DID entries in the yml files
# find issuer_controller/config -name "*.yml" -exec sed -i.bak s/X3tCbZSE9uUb223KYDWd6o/$MY_DID/g {} +
# find issuer_controller -name "*.bak" -type f|xargs rm -f

echo -------------------------
echo The following updates were made to the configuration files:
echo ""

grep -E "${ORG_TITLE}|${MY_ORG}|${MY_PERMIT}|${MY_SEED}|${__TOBAPIURL}|${__TOBAPPURL}|${GENESIS_URL}" issuer_controller/config/*.yml
# grep ${ORG_TITLE} issuer_controller/config/*.yml
# grep ${MY_ORG} issuer_controller/config/*.yml
# grep ${MY_PERMIT} issuer_controller/config/*.yml
# grep ${MY_DID} issuer_controller/config/*.yml
# grep ${MY_SEED} issuer_controller/config/*.yml
# grep ${__TOBAPIURL} issuer_controller/config/*.yml
# grep ${__TOBAPPURL} issuer_controller/config/*.yml
# grep ${GENESIS_URL} issuer_controller/config/*.yml

# Clean up
unset ORG_TITLE MY_ORG MY_PERMIT MY_DID MY_SEED __TOBAPIURL __TOBAPPURL

