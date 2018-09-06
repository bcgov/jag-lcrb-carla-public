BC Liquor and Cannabis Regulation Branch - Cannabis Retail Licence Application
======================

Technology Stack
-----------------

|         |            |
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

**DevOps**
- RedHat OpenShift tools
- Docker
- A familiarity with Jenkins

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

    Copyright 2018 Province of British Columbia

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