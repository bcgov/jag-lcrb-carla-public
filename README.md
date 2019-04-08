BC Liquor and Cannabis Regulation Branch - Cannabis Retail Licence Application
======================

Technology Stack
-----------------

| Layer   | Technology | 
| ------- | ------------ |
| Presentation | Angular 5 |
| Business Logic | C Sharp - Dotnet Core 2.1 |
| Web Server | Kestrel |
| Data    | SQL Server 2017 |
| Data    | MS Dynamics |   

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

Microsoft Dynamics Instance
---------------------------
A MS Dynamics instance containing the necessary solution files is required.  

Define the following secrets in your development environment:
1. DYNAMICS_NATIVE_ODATA_URI: The URI to the Dynamics Web API endpoint.  Example:  `https://<hostname>/<tenant name>/api/data/v8.2/`

Local Instance of SQL Server
----------------------------
This application makes light use of SQL Server for some aspects of the public facing portal.  You may use SQL 2008 R2 or newer; SQL 2017 is recommended.  A docker deployment of SQL Server is suggested.  

To run a local instance with Docker:  
1. `docker volume create mssql` - This line creates persistent storage.  Note that this storage will be created within the Linux VM rather than in the host operating system; that is the recommended approach.
2. `docker run -m 4G -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong(!)Password' -e 'MSSQL_PID=Express' -p 1433:1433 -v mssql:/var/opt/mssql -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu` - This line runs the database container, selecting the Express variant.  The Express variant is free and is sufficient to run the CARLA application.
3. Download and install Azure Data Studio, a Microsoft supported cross platform SQL client
	1. https://github.com/Microsoft/azuredatastudio
4. Connect to the local instance using the SA password specified and do the following:
	1. Create a working database, with a name such as "carla"
	2. Create a user with sufficient access to this database to create tables and read / write data
	3. Define the following Secrets in your development environment:
		1. "DB_ADMIN_PASSWORD": the sa password
		2. "DB_USER": the username for the user you created in step 4.2
  		3. "DB_PASSWORD": The password for the user create in step 4.2
  		4. "DB_DATABASE": The database name for the database created in step 4.1
  		5. "DATABASE_SERVICE_NAME": The hostname for the database server.  If using Docker, this will be 127.0.0.1.  You may add a comma and port if you are not using the standard port of 1433.
	4. Verify that the application runs 

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