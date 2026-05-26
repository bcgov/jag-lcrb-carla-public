# generate-entities.ps1
# Regenerates Dataverse entity classes using pac modelbuilder.
# Run this script any time a new entity is added to Dataverse or existing entity fields change.
#
# Prerequisites:
#   - Power Platform CLI installed: dotnet tool install --global Microsoft.PowerApps.CLI.Tool
#   - Authenticated: pac auth create --url https://<org>.crm.dynamics.com ...
#
# To add a NEW entity:
#   1. Add the entity logical name (lowercase) to the $entities list below
#   2. Run this script
#   3. The new entity class will appear in cllc-interfaces/Dynamics-Dataverse/Generated/
#   4. Add the corresponding methods to IDataverseClient.cs and DataverseClient.cs

$entities = @(
    # Standard CRM entities
    "account"
    "contact"
    "invoice"
    "lead"
    "list"

    # Application & related
    "adoxio_application"
    "adoxio_applicationextension"
    "adoxio_applicationtype"
    "adoxio_applicationtypecontent"
    "adoxio_applicationtermsconditionslimitation"
    "adoxio_termsconditionslimitationspreset"

    # Licence & related
    "adoxio_licences"
    "adoxio_licencetype"
    "adoxio_licencesubcategory"
    "adoxio_licensechangelog"
    "adoxio_endorsement"

    # Worker & screening
    "adoxio_worker"
    "adoxio_personalhistorysummary"
    "adoxio_previousaddress"
    "adoxio_alias"
    "adoxio_login"

    # Establishment / corporate structure
    "adoxio_establishment"
    "adoxio_legalentity"
    "adoxio_tiedhouseconnection"
    "adoxio_tiedhouseassociation"

    # Special events
    "adoxio_specialevent"
    "adoxio_event"
    "adoxio_eventlocation"
    "adoxio_eventschedule"
    "adoxio_sepcity"
    "adoxio_sepdrinktype"
    "adoxio_sepdrinksalesforecast"

    # Licence operations / service areas
    "adoxio_leconnection"
    "adoxio_annualvolume"
    "adoxio_servicearea"
    "adoxio_hoursofsale"
    "adoxio_hoursofservice"
    "adoxio_offsitestorage"

    # Reporting & sync
    "adoxio_cannabismonthlyreport"
    "adoxio_cannabisinventoryreport"
    "adoxio_ldborder"
    "adoxio_federalreportexport"

    # Reference / policy data
    "adoxio_policydocument"
    "adoxio_policejurisdiction"
    "adoxio_localgovindigenousnation"

    # SharePoint integration
    "sharepointdocumentlocation"

    # File attachments / notes (CRITICAL: required by all supporting-document flows)
    "annotation"
    # Add new entities here
)

pac modelbuilder build `
  --outdirectory "cllc-interfaces/Dynamics-Dataverse/Generated" `
  --namespace "Gov.Lclb.Cllb.Interfaces" `
  --entitynamesfilter ($entities -join ";")

Write-Host "Entity generation complete. Files written to cllc-interfaces/Dynamics-Dataverse/Generated/"
