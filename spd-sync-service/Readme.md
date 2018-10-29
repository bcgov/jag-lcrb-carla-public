# SPD Sync #
------------

The purpose of the SPD Sync microservice is send and receive communications to the SPD Figaro system.

Communication is currently delivered using SMTP.

## Development ##

SPD Sync is a Dotnet Core 2.1 application written using the Web SDK.  As such you can use an IDE such as Visual Studio or VS Code to edit the files.  

## Installation ##

Follow the instructions in the `openshift` directory to install the service in OpenShift.

## Testing ##

This section describes concepts and tools necessary for testing the service.

### Curl

Curl is a command line tool for sending HTTP requests.  

The website for Curl is https://curl.haxx.se

###Authorization

To obtain an Authorization token, access the following URL:

<Microservice Base URL>/api/authentication/token?secret=<SECRET>

Note that if your route is using a self signed certificate, you will need to use the -k parameter with Curl to disable certificate validation.

Substitute <SECRET> with the value for the JWT Token Key for the environment.  The token endpoint will only provide a token if the secret provided matches that in the application configuration.

The lengthy string of characters at the start of the response is the token, followed by an expires declaration.  Copy the first string and use that as your token.


### JWT Authentication

The SPD-Sync Microservice uses Json Web Token (JWT) authentication for the REST API server.

JWT authentication is implemented using the "Authentication" header, consisting of "Bearer " plus the text of the token.

To interact with a service using curl, specify the following command line:

`curl --header "Authorization: Bearer <token>"` <rest of Curl command>

Where token is a valid JWT token,  obtained through the Authorization endpoint.  Note that if you copy and paste the token from certain applications it may add Unicode characters; if you run into problems, copy from Notepad or another application that uses plaintext. 

### Triggering the export

Using the above JWT Authentication method, here is the request to start the export:

`curl --header "Authorization: Bearer <TOKEN>" -k https://lcrb-spd-sync-test.pathfinder.bcgov/api/spd/send`

### Viewing Progress of the export

Using the OpenShift platform tool "oc", port forward port 8080 from the SPD Sync Microservice to your local machine, and then access /hangfire on that forwarded port.   Note that you will need at least Edit access to the environment in order to do this. 

# Manual Worker Qualification Results Processing

This process was added October 26th, 2018.

## Worker Updater

There is a hangfire job that fires every 3 minutes. This job checks SharePoint for new files. The directory where job looks for files is as follows:

```
[SharePoint]/spd_worker/SPD WORKER FILES/
```

Files with the prefix of "processed_" will not be processed.

The fields that will be updated by this process are as follows:

```
AdoxioWorker:
    SecurityStatus
    SecurityCompletedOn

PersonalHistorySummary:
    SecurityStatus
    SecurityCompletedOn
```

### Enum for security status

```
PASS = 845280000
FAIL = 845280001
WITHDRAWN = 845280003
```

Once the fields are updated, the job will rename the file with the "processed_" prefix.