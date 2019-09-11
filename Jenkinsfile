node('master') {
	
    stage('Build Image') {
        options {
            timeout(time: 30, unit `MINUTES`)
        }
        echo "Building..."
        openshiftBuild bldCfg: 'cllc-public', showBuildLogs: 'true'
        openshiftTag destStream: 'cllc-public', verbose: 'true', destTag: '$BUILD_ID', srcStream: 'cllc-public', srcTag: 'latest'
    }

	stage('Deploy to Dev') {
        echo "Deploying to dev..."
		openshiftTag destStream: 'cllc-public', verbose: 'true', destTag: 'dev', srcStream: 'cllc-public', srcTag: '$BUILD_ID'
    }	
}

stage('Deploy on Test') {
    input "Deploy to Test?"
    node('master') {
        openshiftTag destStream: 'cllc-public', verbose: 'true', destTag: 'test', srcStream: 'cllc-public', srcTag: '$BUILD_ID'
    }
}

stage('Deploy on Prod') {
    input "Deploy to Prod?"
    node('master') {
        openshiftTag destStream: 'cllc-public', verbose: 'true', destTag: 'prod', srcStream: 'cllc-public', srcTag: '$BUILD_ID'
    }
}

