# Orgbook Service

## Setup of a project namespace

1. Obtain an invitation JSON from the OrgBook team
2. Login to the OpenShift console
3. Observe the value of the secret for the admin-key
4. Port forward port 8024 on the agent-orgbook-issuer pod
5. Enter http://localhost:8024/api/doc
6. Click the Authorize link and enter the value for the admin-key found above in step #3
7. Use the /connections/receive-invitation service to add the invitation JSON to the agent
8. In the response to this service there will be a connection_id - make note of this for the next step
9. Use the GET /connections/{conn_id} service to get the status of the connection
10. You may need to restart the Agent if you do not see "active" in the "state" field of the response after a few minutes

## Increasing the schema version to re-establish the wallet
This is done in a situation such as DEV or TEST when the wallet database is lost or created new.
1. Edit the config/schemas.yml file and increase the version numbers of each schema in the file.
2. Build the Controller
3. Deploy the Controller. 

## Recurring Jobs
- SyncLicencesToOrgbook
  - Check for new licences and issue them to orgbook
    - Query
      - `All active licences with a null orgbook credential result`
    - Action
      - `Issue licence credential to orgbook`

- SyncOrgbookToLicences
  - Check for licences with missing credentials and update to sync with orgbook
    - Query
      - `All active licences with a null orgbook credential id and a successfully issued orgbook credential`
    - Action
      - `Query orgbook for credential id and update licence's orgbook credential id`

- SyncOrgbookToAccounts
  - Check for orgbook details and update accounts
    - Query
      - `All accounts with a bcincorporationnumber and no orgbook organization link and no business registration number`
    - Action
      - `Add the orgbook organization link and success status to the account`

## Test procedure

Testing should only be done in the DEV environment.  

Issue a licence in the case management system to create a licence that has not been sent to orgbook.

Then ensure the OrgBook service is started and is running.  When the SyncLicencesToOrgbook job runs it should send the necessary data to the OrgBook.  Results may be observed by checking the container logs and also by inspecting the case management system to observe that the OrgBook link becomes populated.

