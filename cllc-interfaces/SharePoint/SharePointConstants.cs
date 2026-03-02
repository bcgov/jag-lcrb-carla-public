namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Constants, and related utility methods, for SharePoint library names and internal names.
/// </summary>
/// <remarks>
/// The folder name represents the human readable name of the library ("ex: Application"). The internal name represents
/// the URL part used in the SharePoint REST API to identify the library ("ex: adoxio_application").
/// </remarks>
public static class SharePointConstants
{
    public const string AccountFolderDisplayName = "Account";
    public const string AccountFolderInternalName = "account";

    public const string ApplicationFolderDisplayName = "Application";
    public const string ApplicationFolderInternalName = "adoxio_application";

    public const string ContactFolderDisplayName = "Contact";
    public const string ContactFolderInternalName = "contact";

    public const string WorkerFolderDisplayName = "Worker Qualification";
    public const string WorkerFolderInternalName = "adoxio_worker";

    public const string EventFolderDisplayName = "Licensee Event";
    public const string EventFolderInternalName = "adoxio_event";

    public const string FederalReportFolderDisplayName = "Federal Report Export";
    public const string FederalReportFolderInternalName = "adoxio_federalreportexport";

    public const string LicenceFolderDisplayName = "Licence";
    public const string LicenceFolderInternalName = "adoxio_licences";

    public const string SpecialEventFolderDisplayName = "Special Event";
    public const string SpecialEventFolderInternalName = "adoxio_specialevent";

    public const string KnowledgeArticleTemplateFolderDisplayName = "Knowledge Article Template";
    public const string KnowledgeArticleTemplateFolderInternalName =
        "msdyn_knowledgearticletemplate";

    public const string SubscriberFolderDisplayName = "Subscriber";
    public const string SubscriberFolderInternalName = "lead";

    public const string ContraventionFolderDisplayName = "Contravention";
    public const string ContraventionFolderInternalName = "adoxio_contravention";

    public const string InvestigationRequestFolderDisplayName = "Investigation Request";
    public const string InvestigationRequestFolderInternalName = "adoxio_investigationrequest";

    public const string PartnerContactFolderDisplayName = "Partner Contact";
    public const string PartnerContactFolderInternalName = "adoxio_externalcontact";

    public const string ProductFolderDisplayName = "Product";
    public const string ProductFolderInternalName = "product";

    public const string SecurityClearanceFolderDisplayName = "Security Clearance (Individual)";
    public const string SecurityClearanceFolderInternalName = "adoxio_personalhistorysummary";

    public const string OpportunityFolderDisplayName = "Opportunity";
    public const string OpportunityFolderInternalName = "opportunity";

    public const string SalesLiteratureFolderDisplayName = "Sales Literature";
    public const string SalesLiteratureFolderInternalName = "salesliterature";

    public const string SpdExportFolderDisplayName = "SPD Export";
    public const string SpdExportFolderInternalName = "adoxio_spdexports";

    public const string CommunicationListFolderDisplayName = "Communication List";
    public const string CommunicationListFolderInternalName = "list";

    public const string InvestigationEnforcementFolderDisplayName = "Investigation Enforcement";
    public const string InvestigationEnforcementFolderInternalName =
        "adoxio_complianceinvestigation";

    public const string ArticleFolderDisplayName = "Article";
    public const string ArticleFolderInternalName = "kbarticle";

    public const string EndorsementFolderDisplayName = "Endorsement";
    public const string EndorsementFolderInternalName = "adoxio_endorsement";

    public const string EstablishmentIncidentFolderDisplayName = "Establishment Incident";
    public const string EstablishmentIncidentFolderInternalName = "adoxio_establishmentincident";

    public const string AssociationFolderDisplayName = "Association";
    public const string AssociationFolderInternalName = "adoxio_legalentity";

    public const string AccessRequestsFolderDisplayName = "Access Requests";
    public const string AccessRequestsFolderInternalName = "Access Requests";

    public const string LdbOrderFolderDisplayName = "LDB Order";
    public const string LdbOrderFolderInternalName = "adoxio_ldborder";

    public const string QuoteFolderDisplayName = "Quote";
    public const string QuoteFolderInternalName = "quote";

    public const string EnforcementActionFolderDisplayName = "Enforcement Action";
    public const string EnforcementActionFolderInternalName = "adoxio_enforcementaction";

    public const string PartnerProfileFolderDisplayName = "Partner Profile";
    public const string PartnerProfileFolderInternalName = "adoxio_externalprofile";

    public const string DocumentFolderDisplayName = "Document";
    public const string DocumentFolderInternalName = "adoxio_document";

    public const string InspectionFolderDisplayName = "Inspection";
    public const string InspectionFolderInternalName = "incident";

    public const string CategoryFolderDisplayName = "Category";
    public const string CategoryFolderInternalName = "category";

    public const string StatusCounterFolderDisplayName = "Status Counter";
    public const string StatusCounterFolderInternalName = "adoxio_statuscounter";

    public const string KnowledgeArticleFolderDisplayName = "Knowledge Article";
    public const string KnowledgeArticleFolderInternalName = "knowledgearticle";

    public const string ViolationTicketFolderDisplayName = "Violation Ticket";
    public const string ViolationTicketFolderInternalName = "adoxio_violationticket";

    public const string SharedDocumentsFolderDisplayName = "Documents";
    public const string SharedDocumentsFolderInternalName = "Shared Documents";

    public const string DocumentLocationFolderDisplayName = "Document Location";
    public const string DocumentLocationFolderInternalName = "sharepointdocumentlocation";

    public const string PortalCommentFolderDisplayName = "Portal Comment";
    public const string PortalCommentFolderInternalName = "adx_portalcomment";

    public const string InvestigationFolderDisplayName = "Investigation";
    public const string InvestigationFolderInternalName = "adoxio_investigation";

    public const string PlaybookActivityFolderDisplayName = "Playbook activity";
    public const string PlaybookActivityFolderInternalName = "msdyn_playbookactivity";

    public const string ComplaintFolderDisplayName = "Complaint";
    public const string ComplaintFolderInternalName = "adoxio_complaint";

    /// <summary>
    /// Get the document library Display Name (ex: "Special Event).
    /// </summary>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static string GetDocumentListTitle(string entityName)
    {
        switch (entityName.ToLower())
        {
            case "account":
                return AccountFolderDisplayName;
            case "application":
            case "adoxio_application":
                return ApplicationFolderDisplayName;
            case "contact":
                return ContactFolderDisplayName;
            case "worker":
            case "adoxio_worker":
                return WorkerFolderDisplayName;
            case "special event":
            case "adoxio_specialevent":
                return SpecialEventFolderDisplayName;
            case "event":
            case "adoxio_event":
                return EventFolderDisplayName;
            case "federal_report":
            case "adoxio_federalreportexport":
                return FederalReportFolderDisplayName;
            case "licence":
            case "adoxio_licences":
                return LicenceFolderDisplayName;
            case "enforcement action":
            case "adoxio_enforcementaction":
                return EnforcementActionFolderDisplayName;
            case "complaint":
            case "adoxio_complaint":
                return ComplaintFolderDisplayName;
            case "contravention":
            case "adoxio_contravention":
                return ContraventionFolderDisplayName;
            case "investigation enforcement":
            case "adoxio_complianceinvestigation":
                return InvestigationEnforcementFolderDisplayName;
            case "endorsement":
            case "adoxio_endorsement":
                return EndorsementFolderDisplayName;
            case "legal entity":
            case "adoxio_legalentity":
                return AssociationFolderDisplayName;
            case "establishment incident":
            case "adoxio_establishmentincident":
                return EstablishmentIncidentFolderDisplayName;
            case "incident":
            case "adoxio_incident":
                return InspectionFolderDisplayName;
            default:
                return entityName;
        }
    }

    /// <summary>
    /// Get the document library URL part (ex: "adoxio_specialevent).
    /// </summary>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static string GetDocumentTemplateUrlPart(string entityName)
    {
        switch (entityName.ToLower())
        {
            case "account":
                return AccountFolderInternalName;
            case "application":
            case "adoxio_application":
                return ApplicationFolderInternalName;
            case "contact":
                return ContactFolderInternalName;
            case "worker":
            case "adoxio_worker":
                return WorkerFolderInternalName;
            case "special event":
            case "adoxio_specialevent":
                return SpecialEventFolderInternalName;
            case "event":
            case "adoxio_event":
                return EventFolderInternalName;
            case "federal_report":
            case "adoxio_federalreportexport":
                return FederalReportFolderInternalName;
            case "licence":
            case "adoxio_licences":
                return LicenceFolderInternalName;
            case "enforcement action":
            case "adoxio_enforcementaction":
                return EnforcementActionFolderInternalName;
            case "complaint":
            case "adoxio_complaint":
                return ComplaintFolderInternalName;
            case "contravention":
            case "adoxio_contravention":
                return ContraventionFolderInternalName;
            case "investigation enforcement":
            case "adoxio_complianceinvestigation":
                return InvestigationEnforcementFolderInternalName;
            case "endorsement":
            case "adoxio_endorsement":
                return EndorsementFolderInternalName;
            case "legal entity":
            case "adoxio_legalentity":
                return AssociationFolderInternalName;
            case "establishment incident":
            case "adoxio_establishmentincident":
                return EstablishmentIncidentFolderInternalName;
            case "incident":
            case "adoxio_incident":
                return InspectionFolderInternalName;
            default:
                return entityName;
        }
    }
}
