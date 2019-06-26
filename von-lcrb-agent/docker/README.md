# Running the VON-X Agent using Docker

## Checkout the repository.

If you are planning on submitting Pull Requests to this project, fork the repo and clone your on fork.

```
git clone https://github.com/bcgov/von-agent-template.git
cd von-lcrb-agent/docker
```

The database configuration is defined using environment variables defined in the docker/docker-compose.yml file.  You can accept defaults, edit the yml file (be careful about what you check in) or set environment variables to override the values in the yml file.

## Run the application:

* Open 4 (wow!) shells. The last one is just for running any commands you might need.
* In the first shell, build and start the von-network. See the quick start guide in the von-network repo. Check the operation at: http://localhost:9000
* In the second shell, build and start TheOrgBook. See the quick start guide in TheOrgBook repo. Check the operation at: http://localhost:8080
* In the third shell, navigate to this project's repo, go into the docker folder and build the needed docker image:
    * `./manage build`
* Back to the third shell and run the command:
    * `./manage start`
    * After the startup completes (the logging stops - it will take about a minute to fully start up), go in the browser to `http://localhost:5050`

## To stop the application:

* Hit ctrl-c in each of the 3 shells.

## To reset the application data

* Use the reset instructions for `von-network` and `TheOrgBook` (they are pretty similar to the instructions below...)
* In the 3rd shell - the one for this repo:
    * Change into the `docker` folder
    * Run the command: `./manage rm`
