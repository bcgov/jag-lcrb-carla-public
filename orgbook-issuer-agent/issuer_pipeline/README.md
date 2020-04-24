# BC Registries Event Processor

This project contains a data pipeline for pulling data related to BC Registries events (Registries updates to the records of incorporated entities), using that data to generated Verifiable Credentials, and then delivering those Credentials to [TheOrgBook]((https://github.com/bcgov/TheOrgBook)). The project uses [VON-X](https://github.com/PSPC-SPAC-buyandsell/von-x) as it's Agent to communicate with a [Hyperledger Indy network](https://github.com/hyperledger/indy-node) (eventually [Sovrin](sovrin.org)) and TheOrgBook.

The data pipeline is implemented using the [mara-app](https://github.com/mara/mara-app) framework.

## Mara Example Data Warehouse Project

This project is based on a data warehouse example using the mara framework (https://github.com/mara/mara-example-project).

> A runnable app that demonstrates how to build a data warehouse with mara. Combines the [data-integration](https://github.com/mara/data-integration) and [bigquery-downloader](https://github.com/mara/bigquery-downloader) libraries with the [mara-app](https://github.com/mara/mara-app) framework into a project.

The documentation in this fork of the project describes how to install and run the application.

## Dependencies

The dependencies for the core framework are described in the root Mara project.  This project is setup to use Docker so it doesn't require local setup unless you want to run the application on your local host.

Generated credentials are submitted using eao, a configured instance of VON-X, to TheOrgBook.  Currently the versions used are:

* VON-Network = https://github.com/bcgov/von-network.git, branch master
* TheOrgBook = https://github.com/nrempel/TheOrgBook.git, branch master
* BCReg-X = https://github.com/bcgov/con-bc-registries-agent/eao, branch master

## Running the Event Processor using docker

See the instructions in the [docker folder README](../docker/README.MD).

## Running the Event Processor using OpenShift

Instructions will be added when the OpenShift integration is implemented.

## The Data Pipelines

Pipeline processes are available through the Mara console, and via bash scripts (for scheduled processing).

![Event Processor Dashboard](https://raw.githubusercontent.com/bcgov/von-bc-registries-agent/master/data-pipeline/docs/bc_registries_dashboard.png "Event Processor Dashboard")

The "bc reg event processor" pipeline monitors the BC Registry event queue, loads corporation data, creates and posts credentials.  This job should be run on a schedule (and corresponds to the "bcreg/bc_reg_pipeline.py" script) to continually monitor the BC Registries database for updates.

The "bc reg pipeline status" pipeline lists the number of processed/outstanding records in each stage of the pipeline.  This can be run from the console, or via a script ("bcreg/display_pipeline_status.py").

The "initialization and load tasks" consists of several tasks that are run only once, and correspond to the following scripts:

![Event Processor Dashboard](https://raw.githubusercontent.com/bcgov/von-bc-registries-agent/master/data-pipeline/docs/bc_registries_dashboard_init.png "Event Processor Dashboard")

* "bc reg db init" - bcreg/bc_reg_migrate.py
* "bc reg corp loader" - bcreg/bc_reg_pipeline_initial_load.py
* "bc reg credential poster" - bcreg/bc_reg_pipeline_post_credentials.py

Pipelines under "test and demo tasks" are for test and demonstration purposes only.

## Viewing the Mara Console when it's Deployed into OpenShift

No direct routes to the Mara Console are provisioned, on purpose, by the Mara deployment configurations.  To view the console within a given environment use `oc port-forward` to forward port 8080 from the Mara pod to your local machine and view it in your preferred browser.

Example:
```
oc port-forward mara-9-k9xpd 8080:8080 -n eao-iuc-bc-registries-agent-dev
```

## Running Pipelines from the Command Line

Note that these scripts depend on the database running under docker, based on the  instructions in the [docker README](docker/README.md).

Use the fifth command shell (or open another - why not?) to run the script(s), for example:

```
cd mara-example-project/scripts
BC_REG_DB_USER=<usr> BC_REG_DB_PASSWORD=<pwd> MARA_DB_HOST=localhost MARA_DB_PORT=5444 ./run-step.sh bcreg/<script>.py
```

The logs and run stats can be viewed in the UI.

NOTE: When you run the pipeline using the user interface, it uses a copy of the code in the docker image.  If you run the pipeline for the command line, it uses the code on your local machine. As such, you can edit the code locally and run it from the command line to test things WITHOUT building it into the docker image.

## Running Pipelines to Perform Initialization and Data Load

The following will perform database initialization and do the initial corporation data load:

```
cd mara-example-project/scripts

BC_REG_DB_USER=<usr> BC_REG_DB_PASSWORD=<pwd> MARA_DB_HOST=localhost MARA_DB_PORT=5444 ./run-step.sh bcreg/bc_reg_migrate.py

BC_REG_DB_USER=<usr> BC_REG_DB_PASSWORD=<pwd> MARA_DB_HOST=localhost MARA_DB_PORT=5444 ./run-step.sh bcreg/bc_reg_pipeline_initial_load.py
```

The initial load will load over 930,000 corporations and process almost 2 million credentials.  It will run for almost 12 hours.  

The following can be run at the same time (in a separate console window) to post credentials to TOB (wait for ~15-20 minutes to allow the initial load to start populating the outbound credential queue):

```
cd mara-example-project/scripts

BC_REG_DB_USER=<usr> BC_REG_DB_PASSWORD=<pwd> MARA_DB_HOST=localhost MARA_DB_PORT=5444 ./run-step.sh bcreg/bc_reg_pipeline_post_credentials.py
```

## Performing an Initial Data Load in OpenShift

*The following configuration was found to provide the highest sustained throughput for posting credentials.*  *The eao-agent and mara pods have to be manually scaled because they do not have any resource limits set on which a horizontal autoscaler could operate.  The `tob-api` pods are configured with a horizontal autoscaler.*

Scale the eao-agent deployment to 8 to 10 pods and make sure they have fully started.  When feeding into 5 `tob-api` pods, these 8 to 10 agent pods can provide a throughput of a little over 2,600 credentials per minute at best.  Increasing the number of pods on either side has little to no additional performance benefit.

Scale the mara deployment to 2 pods.

Use `oc rsh` to open a terminal session with each of the mara containers.

In one container run the following scripts to start the credential staging process;
```
cd scripts
./run-step.sh bcreg/bc_reg_migrate.py
./run-step.sh bcreg/bc_reg_pipeline_initial_load.py
```

Allow the credential staging process to run for a while to allow it to get a head start on the credential posting process.  Posting is faster than staging, at the moment, so if you don't allow staging to get a head start the posting process will run out of work and end.

In the second mara container start the credential posting process;
```
cd scripts
./run-step.sh bcreg/bc_reg_pipeline_post_credentials.py
```
Once the initial load is complete the deployments can be scaled back to single pods.

## Running Pipelines to Perform On-going Event Monitoring and Credential Updates

The following should be run at regular intervals (e.g. 15 minutes) on a scheduler:

```
cd mara-example-project/scripts

BC_REG_DB_USER=<user> BC_REG_DB_PASSWORD=<pwd> MARA_DB_HOST=localhost MARA_DB_PORT=5444 ./run-step.sh bcreg/bc_reg_pipeline.py
```

Hint:  If you want to test out the "bc_reg_pipeline.py" script, run the following sub-task under "test and demo tasks" in the Mara console first:

* "bc reg test data"

## Extending Event Processor

The mara setup itself is generic, as long as you run from the provided docker scripts.

There is a single dependency in the following script that references the BC Reg pipelines:

* data-pipeline/app/data_integration/__init__.py

This script initializes the pipelines that are displayed in the UI, so must be customized to include any necessary pipelines.

The BC Registries specific code is all in the data-pipeline/bcreg directory.  Since the pipelines are setup to run Python commands from the command line, there are no code dependencies between the two projects (other than dependencies on the data_integration framework itself).
