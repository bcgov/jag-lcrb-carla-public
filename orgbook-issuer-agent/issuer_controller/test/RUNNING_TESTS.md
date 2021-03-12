# Running test suite    

1 find pod name with `docker ps | grep myorg_controller`, it will be the last value the returned line. ('myorg_myorg-controller_1' by default)

2 Open interactive terminal in pod with `docker exec -it myorg_myorg-controller_1 bash` 

3 `pwd` should show 'home/indy/' and `ls` should show the contents of  _/issuer_controller_ folder.

4 run `pytest` to get test results.

## Reporting Test Coverage

One user of the template has built a github action to execute and report test coverage to code climate, that example can be found [here](https://github.com/bcgov/mines-digital-trust/blob/develop/.github/workflows/code_climate.yml)