# PDF Service #
------------

The purpose of the PDF microservice is to generate PDF documents based on templates and JSON format data.  No database connection is used to generate the PDF.

## Development ##

This service is a Dotnet Core 2.1 application written using the Web SDK.  As such you can use an IDE such as Visual Studio or VS Code to edit the files.  

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

The PDF Microservice uses Json Web Token (JWT) authentication for the REST API server.

JWT authentication is implemented using the "Authentication" header, consisting of "Bearer " plus the text of the token.

To interact with a service using curl, specify the following command line:

`curl --header "Authorization: Bearer <token>"` <rest of Curl command>

Where token is a valid JWT token,  obtained through the Authorization endpoint.  Note that if you copy and paste the token from certain applications it may add Unicode characters; if you run into problems, copy from Notepad or another application that uses plaintext. 

### Triggering the export

Using the above JWT Authentication method, here is the request to start the export:

`curl --header "Authorization: Bearer <TOKEN>" -d {input data} -k <URL to service>`

## Adding a new template

To have multiple templates do the following:

### In the pdf-service project

1. Add your template to the Templates directory.
2. Add your template to the item group in pdf-service.csproj

```
<None Update="Templates\{your-template-name}.mustache">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

### In your service that is connecting to pdf-service

3. Query the service by using the following code where the second parameter is the name of the template without .mustache. The example below assumes the template is called cannabis_licence.mustache:

```
byte[] data = await _pdfClient.GetPdf(parameters, "cannabis_licence");
```