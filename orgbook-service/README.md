# Orgbook Service

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
