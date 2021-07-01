# CARLA SPICE Sync #
------------

The purpose of the CARLA-SPICE Sync microservice is send and receive communications to the SPD SPICE system.

Communication is currently delivered using REST.

## Development ##

CARLA-SPICE Sync is a .NET 5 application written using the Web SDK.  As such you can use an IDE such as Visual Studio or VS Code to edit the files.  

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


# Worker Qualification Results Processing

Worker qualification result processing was deactivated in June 2021 as a result of changing regulations.