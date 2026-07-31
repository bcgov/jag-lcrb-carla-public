# CARLA Portal — Dataverse Migration QA Verification Plan

**Branch:** `dv-migration`  
**Environment:** OpenShift sandbox-dev  
**Last updated:** 2026-07-28  
**Also see:** `tasks/QA-verification-plan.md` for build/grep commands and post-merge actions.  
**Validation tickets:** `tasks/VAL-001` through `tasks/VAL-014`

---

## Change Log

| Date | Change |
|---|---|
| 2026-06-24 | Initial plan (QA-001–QA-016) |
| 2026-07-06 | FileController download fix — `UpdateEntityModifiedOnDate` now passes `false` for downloads |
| 2026-07-17 | CI/CD GitHub Actions workflows + Dockerfiles added for all services; Startup.cs ILoggerFactory injection pattern refactored (federal-reporting, geocoder, one-stop, orgbook) |
| 2026-07-20 | file-manager-service `IsOpenShift` detection fix — now checks `KUBERNETES_SERVICE_HOST` |
| 2026-07-22 | nginx `underscores_in_headers on` — SiteMinder auth headers were silently dropped |
| 2026-07-23 | Redis Data Protection key ring — session cookies now survive pod restarts and multi-pod routing |
| 2026-07-23 | SharePoint folder name fix — `GetFolderNameAsync` now handles account, contact, application, worker, licence, and event entity types (previously only event worked) |

---

## Critical Path — Test These First

These fixes directly caused production-blocking issues on OpenShift. Validate before anything else:

| Priority | Test | Ticket |
|---|---|---|
| P0 | Login via SiteMinder and reach dashboard | VAL-001 |
| P0 | Session persists after pod restart / across multiple pods | VAL-002 |
| P0 | Upload a file to an application | VAL-004 |
| P0 | Upload a file to an account | VAL-004 |
| P1 | Download a file — no Dynamics workflow incorrectly triggered | VAL-004 |
| P1 | All services health endpoint responds Healthy | VAL-013 |

---

## CLLC Public App — Authentication

---

### QA-001 — Login: New User Registration via BCeID

**Description**  
When a user logs into the portal via BCeID for the first time, the system must create a contact record and a bridge login record in CRM, then redirect the user to the onboarding/registration flow.

**What Changed**  
The contact lookup during authentication now uses the Dataverse SDK. nginx now passes SiteMinder headers with underscores (`sm_universalid`, `sm_user`, etc.) which was previously blocked.

**What Needs to Be Verified**  
- Portal redirects to the new user onboarding screen without errors  
- Contact record is created in CRM  
- An `adoxio_login` bridge record is created linking the BCeID GUID to the contact  

**Steps**  
1. Open the portal in a private/incognito browser window  
2. Click **Log in with BCeID** and authenticate using a BCeID account that has never accessed this portal  
3. Confirm the portal redirects to the new user registration or onboarding screen (not a 500 error or blank page)  
4. In CRM, locate the newly created contact record for this user  
5. In CRM, confirm an `adoxio_login` record exists for this contact with the BCeID SiteMinder GUID populated  

---

### QA-002 — Login: Returning User Whose GUID is Stored Directly on the Contact

**Description**  
Some users were created before the bridge login system was introduced. Their BCeID GUID is stored directly on the contact record. These users must be able to log in and have a bridge record auto-created.

**What Changed**  
The authentication handler now performs a two-step lookup: bridge table first, then ExternalID fallback. Bridge record is auto-created on fallback.

**What Needs to Be Verified**  
- User successfully reaches their account dashboard  
- An `adoxio_login` bridge record is created automatically in CRM after this login  

**Steps**  
1. Identify a test account in CRM where the contact has `adoxio_ExternalId` populated but no `adoxio_login` bridge row exists  
2. Log in via BCeID using this account  
3. Confirm the portal loads the correct dashboard  
4. In CRM, confirm a new `adoxio_login` record has been created  

---

### QA-003 — Login: Pre-Created Business Account with ExternalID Set

**What Needs to Be Verified**  
- Business owner is associated with the correct pre-existing account on first login  
- No duplicate account is created in CRM  

**Steps**  
1. Identify a test account in CRM that was pre-created by staff with `adoxio_ExternalId` populated  
2. Log in via BCeID as the business owner  
3. Confirm the portal dashboard displays the correct pre-existing business name  
4. In CRM, confirm no duplicate account was created  

---

### QA-004 — Login: SEP Police Representative Post-Login Routing

**What Needs to Be Verified**  
- SEP police representative user is redirected to `/sep/dashboard`  
- Standard user is redirected to `/dashboard`  

**Steps**  
1. Log in with an SEP police representative account  
2. Confirm URL ends with `/sep/dashboard`  
3. Log out; log in with a standard account  
4. Confirm URL ends with `/dashboard`  

---

## CLLC Public App — Applications

---

### QA-005 — Application Status: "Permanent Change to a Licensee" Payment Logic

**What Needs to Be Verified**  
- Application with both invoices paid shows **"Under Review"**  
- Application with outstanding invoice does not show **"Under Review"**  

**Steps**  
1. Find a PCL application with both invoices present and paid → confirm **"Under Review"**  
2. Find a PCL application with an unpaid invoice → confirm it does NOT show "Under Review"  

---

### QA-006 — Application Dashboard: List Completeness and Status Labels

**What Needs to Be Verified**  
- Application count and statuses match the same account in the current production system  

**Steps**  
1. Record count and statuses from production for a test account  
2. Log in with the same account in sandbox-dev  
3. Confirm count matches; no missing or extra applications; all status labels match  

---

## CLLC Public App — Licences

---

### QA-007 — Licence Dashboard: List, Payment Badge, and Transfer Status

**What Needs to Be Verified**  
- All active licences appear  
- **"Payment Required"** badge on licences with unpaid fee  
- **"Transfer in Progress"** badge on pending-transfer licences  
- Cancelled/expired licences excluded  

**Steps**  
1. Log in with an account that has a licence needing fee payment and one with a pending transfer  
2. Confirm each badge appears on the correct licence  
3. Confirm no cancelled/expired licences appear  

---

### QA-008 — Licence Transfer: Business Name Search

**What Needs to Be Verified**  
- Autocomplete returns ≤ 10 active businesses matching the entered text  
- Inactive businesses are excluded (server-side `statecode = 0` filter)  

**Steps**  
1. Navigate to a licence transfer form  
2. Type 3–4 characters of a known business name  
3. Confirm results appear, ≤ 10 results, no inactive businesses  

---

## CLLC Public App — Payments

---

### QA-009 — Payment Flow: Application and Licence Fee Payments via BCEP

**What Needs to Be Verified**  
- "Pay Now" generates a valid BCEP redirect URL  
- Application status updates to **"Under Review"** after successful payment  
- Licence fee payment route functions identically  

**Steps**  
1. Log in with an account with an outstanding invoice → click **Pay Now**  
2. Confirm redirect to BCEP portal  
3. Complete payment with test card  
4. Confirm application status updated to **"Under Review"**  
5. Repeat for the licence fee payment route  

---

## CLLC Public App — Contact & Workers

---

### QA-010 — Contact Profile: Save and Persist All Fields

**What Needs to Be Verified**  
- All profile fields persist after save and page reload  

**Steps**  
1. Navigate to contact profile  
2. Update name, address, phone  
3. Save; reload page  
4. Confirm all updated fields show new values  

---

### QA-011 — Worker Registration and Contact Enrichment

**What Changed**  
`WorkerController` now fetches full contact via `GetContactByIdAsync` — contact name/email/phone/birthdate all populated on the worker record.

**What Needs to Be Verified**  
- New worker can be registered and appears with correct status  
- Worker detail shows full contact info (not a stub with only the ID)  
- Existing worker with completed screening shows correct status  

**Steps**  
1. Register a new worker → confirm in list with **"Active"** status  
2. Open the worker record → confirm full name, birthdate, phone, email all appear  
3. Open an existing worker with a completed screening → confirm screening status is not reset  

---

## CLLC Public App — Documents

---

### QA-012 — Document Upload and File Access

**What Changed (Jul 6, Jul 23)**  
- `FileController` download no longer sets `AdoxioFileuploadedfromportal = true` (was incorrectly triggering Dynamics upload workflow on downloads)  
- `GetFolderNameAsync` now handles all entity types — uploads to applications, accounts, workers, and licences previously returned a null folder name and crashed

**What Needs to Be Verified**  
- Upload to an **application** — file appears in SharePoint and is downloadable  
- Upload to an **account** — file appears in SharePoint  
- Upload to a **worker** — file appears in SharePoint  
- Upload to a **licence** — file appears in SharePoint  
- Download a file — the application/worker record's status does NOT change after the download (no Dynamics workflow triggered)  

**Steps**  
1. Upload a PDF to an application → confirm file appears in document list → confirm downloadable  
2. Repeat for account, worker, and licence  
3. Download a file; wait 30 seconds; reload the record → confirm status/checklist is unchanged  
4. Check application logs — no "GetFolderNameAsync returned null" or SharePoint 500 errors  

---

## CLLC Public App — Business Profile

---

### QA-013 — Account Business Profile: Primary Contact Populated

**What Changed**  
`AccountsController.GetCurrentAccount`, `GetAccount`, and `CreateAccount` now fetch primary contact via `GetContactByIdAsync` and populate `vm.primarycontact`.

**What Needs to Be Verified**  
- Business profile page shows the primary contact name and email (not null)  

**Steps**  
1. Log in as an account with a primary contact set in CRM  
2. Navigate to business profile  
3. Confirm primary contact name and email are displayed  

---

## one-stop-service — Provincial OneStop Integration

---

### QA-014 — OneStop: Outbound Licence Notifications and Inbound Hub Messages

**What Needs to Be Verified**  
- Outbound notification triggered when a licence status changes  
- Notification XML contains: BN9, legal name, licence number, establishment postal code  
- Inbound hub message processed and CRM record updated  

**Steps**  
1. Update a licence status enrolled with OneStop in staging  
2. Confirm outbound notification sent (check service logs or hub ack)  
3. Inspect payload: BN9, name, licence number, postal code correct  
4. Trigger/simulate inbound hub message → confirm CRM record updated  

---

## carla-spice-sync-service — SPICE Police Screening

---

### QA-015 — SPICE: Worker and Application Screening Request Payloads

**What Needs to Be Verified**  
- Outbound worker screening payload contains all required fields  
- Outbound application screening payload contains applicant, establishment, associate data  
- Incoming screening result updates worker/application record in CRM  

**Steps**  
1. Submit a new worker registration → confirm outbound payload contains name, birthdate, address, ID, aliases  
2. Submit a new application → confirm payload contains applicant, establishment, associates  
3. Receive/simulate SPICE callback → confirm worker/application status updated in CRM  

---

## federal-reporting-service — Federal Cannabis Reporting

---

### QA-016 — Federal Reporting: Monthly Cannabis Export

**What Changed**  
`federal-reporting-service/Startup.cs` — `ILoggerFactory` injection refactored; file-manager connection failure is now caught at startup (non-fatal) so the service starts even if file-manager is not yet ready.

**What Needs to Be Verified**  
- Service starts without throwing a startup exception even if file-manager is unavailable  
- Export job completes; CSV created in SharePoint  
- CSV contains all required fields  

**Steps**  
1. Confirm service starts (check pod logs — no fatal startup exception)  
2. Confirm at least one submitted cannabis monthly report exists in staging  
3. Trigger `ExportFederalReports` Hangfire job → confirm job completed without errors  
4. Locate exported CSV in SharePoint → confirm required fields present  

---

## Supporting Services

---

### QA-017 — SharePoint Sync, LDB Orders, Geocoder, and OrgBook Services

**What Changed**  
- `geocoder-service/Startup.cs` and `orgbook-service/Startup.cs` — ILoggerFactory injection refactored; Hangfire job registration updated  
- `file-manager-service/OpenShift.cs` — `IsOpenShift` now correctly checks `KUBERNETES_SERVICE_HOST` env var (was broken on OpenShift, causing incorrect code path)  

**What Needs to Be Verified**  
- **File Manager:** `IsOpenShift` returns `true` in the pod; correct code path used for file storage  
- **SharePoint Sync:** Sync cycle completes without errors  
- **LDB Orders:** Returns results for a known order  
- **Geocoder:** Returns coordinates for a valid BC address  
- **OrgBook:** Returns business registration data for a known BC company  

**Steps**  
1. File Manager: check pod logs for `IsOpenShift=true`; upload and retrieve a file  
2. SharePoint Sync: trigger sync job → confirm completes without errors  
3. LDB Orders: trigger order retrieval for a known account → confirm data returned  
4. Geocoder: submit valid BC address → confirm lat/lng returned  
5. OrgBook: look up a known BC business → confirm name and registration number returned  

---

## Post-Analysis Fixes — Additional Tests

---

### QA-018 — SharePoint Document Location: Root Library Query (Jul 3 fix)

**What Needs to Be Verified**  
The `parentsiteorlocation` attribute fix ensures the root library query succeeds so SharePoint folder creation doesn't silently fail.

**Steps**  
1. Upload to a brand-new entity (first upload ever for this account/worker/licence)  
2. Confirm a new SharePoint folder is created (not just a document location record pointing to nothing)  
3. Check logs — no `parentsiteorlocation` query errors  

---

### QA-019 — Legal Entity Tree: Multi-Level Shareholder Structure

**What Changed**  
BFS parallel refactor — test multi-level (3+) shareholder trees to confirm all nodes returned.

**Steps**  
1. Find an account with a 3+ level shareholder tree  
2. Open the Legal Entities section → confirm all shareholders at all levels appear  
3. Confirm no duplicates; compare node count with CRM  

---

### QA-020 — Application Detail: Parallel Lookup Completeness

**Steps**  
1. Open an application with a fee invoice, application type with form, and a linked LGIN  
2. Confirm all five fields populated: LicenceFeeInvoice, ApplicationType, DynamicsForm, AssignedLicence, IndigenousNation  
3. Open an application without a fee invoice — confirm no null reference error  

---

### QA-021 — .NET 8.0 Upgrade: Service Runtime

**Steps**  
1. Check each pod's logs for `.NET 8` runtime version message at startup  
2. Confirm `global.json` pins SDK 8.0.100 (`dotnet --version` on build agent)  
3. Import a test CSV via LDB orders — confirm CsvHelper 33.x parses without exception  
4. Inspect each `Dockerfile.gha` — confirm `FROM mcr.microsoft.com/dotnet/aspnet:8.0`  
