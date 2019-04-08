BC Liquor and Cannabis Regulation Branch - Cannabis Retail Licence Application
======================

Technology Stack
-----------------

| Layer   | Technology | 
| ------- | ------------ |
| Presentation | Angular 7 |
| Business Logic | C Sharp - Dotnet Core 2.1 |
| Web Server | Kestrel |
| Data    | SQL Server 2017 |
| Data    | MS Dynamics |
| Document Management    | MS SharePoint |   

Repository Map
--------------
- **.s2i**: Source to image (s2i) assembly script
- **agent-protractor**: Automated end to end tests 
- **cllc-interfaces**: Interface libraries for other systems:
  - BCeID
  - BC Express Pay (BCEP)
  - BC Registries
  - Microsoft Dynamics
  - Microsoft SharePoint
- **cllc-interfaces-test**: Automated tests for the above interface libraries
- **cllc-public-app**: Source code for the public facing application
- **cllc-public-app-test**: Automated tests for the public application
- **dotnet-sonar**: Source for a helper image that runs a sonar scan
- **functional-tests**: Source for BDD tests
- **openshift**: Various OpenShift related material, including instructions for setup and templates.
- **sql-server**: Image source for a sql server instance. 

Installation
------------
This application is meant to be deployed to RedHat OpenShift version 3. Full instructions to deploy to OpenShift are in the `openshift` directory.

Developer Prerequisites
-----------------------

**Public Application**
- .Net Core SDK (Dotnet Core 2 is used for all components)
- Node.js version 8 LTS
- .NET Core IDE such as Visual Studio or VS Code
- Local instance of SQL Server

**DevOps**
- RedHat OpenShift tools
- Docker
- A familiarity with Jenkins

Microsoft Dynamics, SharePoint
---------------------------
A MS Dynamics instance containing the necessary solution files is required.  A SharePoint connection is optional.  If no SharePoint connection is available then file operations will not be executed.

Define the following secrets in your development environment (secrets or environment variables):
1. DYNAMICS_NATIVE_ODATA_URI: The URI to the Dynamics Web API endpoint.  Example:  `https://<hostname>/<tenant name>/api/data/v8.2/`.  This URI can be a proxy.
2. DYNAMICS_NATIVE_ODATA_URI: The native URI to the Dynamics Web API endpoint, in other words as the server identifies itself in responses to WebAPI requests.  Do not put a proxy URI here.
3. SSG_USERNAME: API gateway username, if using an API gateway
4. SSG_PASSWORD: API gateway password, if using an API gateway
5. DYNAMICS_AAD_TENANT_ID: ADFS Tenant ID, if using ADFS authentication.  Leave blank if using an API gateway
6. DYNAMICS_SERVER_APP_ID_URI: ADFS Server App ID URI. Leave blank if using an API gateway
7. DYNAMICS_CLIENT_ID: Public Key for the ADFS Enterprise Application app registration. Leave blank if using an API gateway
8. SHAREPOINT_ODATA_URI: Endpoint to be used for SharePoint, exclusive of _api.  Can be a proxy.  Leave blank if not using SharePoint.
9. SHAREPOINT_NATIVE_BASE_URI:  The SharePoint URI as configured in SharePoint.  Do not set to a proxy.
10. SHAREPOINT_SSG_USERNAME, SHAREPOINT_SSG_PASSWORD - optional API Gateway credentials for SharePoint
11. SharePoint may also use the same ADFS credentials as Dynamics.  If that is to be used, leave all SSG parameters empty or undefined.

Local Instance of SQL Server
----------------------------
This application makes light use of SQL Server for some aspects of the public facing portal.  You may use SQL 2008 R2 or newer; SQL 2017 is recommended.  A docker deployment of SQL Server is suggested.  

To run a local instance with Docker:  
1. `docker volume create mssql` - This line creates persistent storage.  Note that this storage will be created within the Linux VM rather than in the host operating system; that is the recommended approach.
2. `docker run -m 4G -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -e 'MSSQL_PID=Express' -p 1433:1433 -v mssql:/var/opt/mssql -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu` - This line runs the database container, selecting the Express variant.  The Express variant is free and is sufficient to run the CARLA application.
3. Download and install Azure Data Studio, a Microsoft supported cross platform SQL client
	1. https://github.com/Microsoft/azuredatastudio
4. Connect to the local instance using the SA password specified and verify operation of the database.
5. Define the following Secrets in your development environment:
		1. "DB_ADMIN_PASSWORD": the sa password
		2. "DB_USER": the database username you would like to use 
  		3. "DB_PASSWORD": The password you would like to use
  		4. "DB_DATABASE": The database name you would like to use
  		5. "DATABASE_SERVICE_NAME": The hostname for the database server.  If using Docker, this will be 127.0.0.1.  You may add a comma and port if you are not using the standard port of 1433.
6. Run the application and verify that the database was created properly 

Mac Environment
---------------
A Mac computer can be used for development.  The easiest way to prepare the Mac for development is to install Visual Studio 2019 for Mac.  

Prior to running the application for the first time, change directory to the "ClientApp" sub directory of cllc-public-app and execute `npm install`.  This will allow the node dependencies for the software to be downloaded.

A script `go-cllc.sh` has been provided to allow the application to be run on a mac.

Edit this file with the various settings you need to run the application.

If you wish to use a port other than 50001 for the app, change the last line to:

dotnet run --server.urls http://0.0.0.0:5000

(Where 5000 is your chosen port).


Note that for development purposes you should set ASPNETCORE_ENVIRONMENT to Development

(It can be set to Staging or Production after you execute a dotnet publish command to build static files)


DevOps Process
-------------

### Jenkins

If any pipeline steps do not start, a common root cause is problems with Jenkins.  Restart the Jenkins service by scaling it down to 0 pods, then back up to 1 pod.

## DEV builds
Dev builds are triggered by source code being committed to the repository.  This process triggers a webhook which initiates the DEV build pipeline.

## Promotion to TEST
Login to the OpenShift Web Console and navigate to the Tools project for the system.  Go to Builds->Pipelines.  Click  Start Pipeline on the Test Pipeline.

## Promotion to PROD
Login to the OpenShift Web Console and navigate to the Tools project for the system.  Go to Builds->Pipelines.  Click  Start Pipeline on the Prod Pipeline.

Other
-----------

**SonarQube**

SonarQube is a code quality service that helps identify problem areas in code through static analysis.

The development pipeline, called during the OpenShift build process, includes a stage where the code is sent to SonarQube for analysis.


Contribution
------------

Please report any [issues](https://github.com/bcgov/jag-lcrb-carla-public/issues).

[Pull requests](https://github.com/bcgov/jag-lcrb-carla-public/pulls) are always welcome.

If you would like to contribute, please see our [contributing](CONTRIBUTING.md) guidelines.

Please note that this project is released with a [Contributor Code of Conduct](CODE_OF_CONDUCT.md). By participating in this project you agree to abide by its terms.

License
-------

    Copyright 2019 Province of British Columbia

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at 

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.

Maintenance
-----------

This repository is maintained by [BC Attorney General]( https://www2.gov.bc.ca/gov/content/governments/organizational-structure/ministries-organizations/ministries/justice-attorney-general ).