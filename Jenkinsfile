node('master') {
	
    stage('Build Image') {
        echo "Building..."
        openshiftBuild bldCfg: 'lclb-public', showBuildLogs: 'true'
        openshiftTag destStream: 'lclb-public', verbose: 'true', destTag: '$BUILD_ID', srcStream: 'lclb-public', srcTag: 'latest'
    }

	stage('Deploy to Dev') {
        echo "Deploying to dev..."
		openshiftTag destStream: 'lclb-public', verbose: 'true', destTag: 'dev', srcStream: 'lclb-public', srcTag: '$BUILD_ID'
    }	
}

stage('Deploy on Test') {
    input "Deploy to Test?"
    node('master') {
        openshiftTag destStream: 'lclb-public', verbose: 'true', destTag: 'test', srcStream: 'lclb-public', srcTag: '$BUILD_ID'
    }
}

stage('Deploy on Prod') {
    input "Deploy to Prod?"
    node('master') {
        openshiftTag destStream: 'lclb-public', verbose: 'true', destTag: 'prod', srcStream: 'lclb-public', srcTag: '$BUILD_ID'
    }
}

