BDD Testing Configuration
======================

The following configuration must be completed for the automated tests to run.

Secrets Definition
---------------------------
Define the following secrets in your development environment (secrets or environment variables):

baseURI: The target environment for the automated tests.
test_start: The login token for a cannabis retail store application.
test_start_worker: The login token for a cannabis worker application.
test_cc: The test credit card for making application test payments.
test_ccv: The corresponding CCV number for the test credit card used.