namespace Gov.Lclb.Cllb.Interfaces
{
    public static class SharePointHelpers
    {
        public const string DefaultDocumentListTitle = "Account";
        public const string DefaultDocumentUrlTitle = "account";
        public const string ApplicationDocumentListTitle = "Application";
        public const string ApplicationDocumentUrlTitle = "adoxio_application";
        public const string ContactDocumentListTitle = "contact";
        public const string ContactDocumentUrlTitle = "contact";
        public const string WorkerDocumentListTitle = "Worker Qualification";
        public const string WorkerDocumentUrlTitle = "adoxio_worker";
        public const string SpecialEventDocumentListTitle = "Special Event";
        public const string SpecialEventDocumentUrlTitle = "adoxio_specialevent";
        public const string EventDocumentListTitle = "adoxio_event";
        public const string EventDocumentUrlTitle = "adoxio_event";
        public const string FederalReportListTitle = "adoxio_federalreportexport";
        public const string FederalReportUrlTitle = "adoxio_federalreportexport";
        public const string LicenceDocumentUrlTitle = "adoxio_licences";
        public const string LicenceDocumentListTitle = "Licence";
        public const string EnforcementActionDocumentListTitle = "enforcement action";
        public const string EnforcementActionDocumentUrlTitle = "adoxio_enforcementaction";
        public const string ComplaintDocumentListTitle = "complaint";
        public const string ComplaintDocumentUrlTitle = "adoxio_complaint";
        public const string ContraventionDocumentListTitle = "contravention";
        public const string ContraventionDocumentUrlTitle = "adoxio_contravention";
        public const string InvestigationEnforcementDocumentListTitle = "investigation enforcement";
        public const string InvestigationEnforcementDocumentUrlTitle =
            "adoxio_complianceinvestigation";
        public const string EndorsementDocumentListTitle = "endorsement";
        public const string EndorsementDocumentUrlTitle = "adoxio_endorsement";
        public const string LegalEntityDocumentListTitle = "legal entity";
        public const string LegalEntityDocumentUrlTitle = "adoxio_legalentity";
        public const string EstablishmentIncidentDocumentListTitle = "establishment incident";
        public const string EstablishmentIncidentDocumentUrlTitle = "adoxio_establishmentincident";
        public const string IncidentDocumentListTitle = "inspection";
        public const string IncidentDocumentUrlTitle = "incident";

        /// <summary>
        /// Get the document library Display Name (ex: "Special Event).
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetDocumentListTitle(string entityName)
        {
            string listTitle;
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = DefaultDocumentListTitle;
                    break;
                case "application":
                case "adoxio_application":
                    listTitle = ApplicationDocumentListTitle;
                    break;
                case "contact":
                    listTitle = ContactDocumentListTitle;
                    break;
                case "worker":
                case "adoxio_worker":
                    listTitle = WorkerDocumentListTitle;
                    break;
                case "special event":
                case "adoxio_specialevent":
                    listTitle = SpecialEventDocumentListTitle;
                    break;
                case "event":
                case "adoxio_event":
                    listTitle = EventDocumentListTitle;
                    break;
                case "federal_report":
                case "adoxio_federalreportexport":
                    listTitle = FederalReportListTitle;
                    break;
                case "licence":
                case "adoxio_licences":
                    listTitle = LicenceDocumentListTitle;
                    break;
                case "enforcement action":
                case "adoxio_enforcementaction":
                    listTitle = EnforcementActionDocumentListTitle;
                    break;
                case "complaint":
                case "adoxio_complaint":
                    listTitle = ComplaintDocumentListTitle;
                    break;
                case "contravention":
                case "adoxio_contravention":
                    listTitle = ContraventionDocumentListTitle;
                    break;
                case "investigation enforcement":
                case "adoxio_complianceinvestigation":
                    listTitle = InvestigationEnforcementDocumentListTitle;
                    break;
                case "endorsement":
                case "adoxio_endorsement":
                    listTitle = EndorsementDocumentListTitle;
                    break;
                case "legal entity":
                case "adoxio_legalentity":
                    listTitle = LegalEntityDocumentListTitle;
                    break;
                case "establishment incident":
                case "adoxio_establishmentincident":
                    listTitle = EstablishmentIncidentDocumentListTitle;
                    break;
                case "incident":
                case "adoxio_incident":
                    listTitle = IncidentDocumentListTitle;
                    break;
                default:
                    listTitle = entityName;
                    break;
            }

            return listTitle;
        }

        /// <summary>
        /// Get the document library URL part (ex: "adoxio_specialevent).
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetDocumentTemplateUrlPart(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = DefaultDocumentUrlTitle;
                    break;
                case "application":
                case "adoxio_application":
                    listTitle = ApplicationDocumentUrlTitle;
                    break;
                case "contact":
                    listTitle = ContactDocumentUrlTitle;
                    break;
                case "worker":
                case "adoxio_worker":
                    listTitle = WorkerDocumentUrlTitle;
                    break;
                case "special event":
                case "adoxio_specialevent":
                    listTitle = SpecialEventDocumentUrlTitle;
                    break;
                case "event":
                case "adoxio_event":
                    listTitle = EventDocumentUrlTitle;
                    break;
                case "federal_report":
                case "adoxio_federalreportexport":
                    listTitle = FederalReportUrlTitle;
                    break;
                case "licence":
                case "adoxio_licences":
                    listTitle = LicenceDocumentUrlTitle;
                    break;
                case "enforcement action":
                case "adoxio_enforcementaction":
                    listTitle = EnforcementActionDocumentUrlTitle;
                    break;
                case "complaint":
                case "adoxio_complaint":
                    listTitle = ComplaintDocumentUrlTitle;
                    break;
                case "contravention":
                case "adoxio_contravention":
                    listTitle = ContraventionDocumentUrlTitle;
                    break;
                case "investigation enforcement":
                case "adoxio_complianceinvestigation":
                    listTitle = InvestigationEnforcementDocumentUrlTitle;
                    break;
                case "endorsement":
                case "adoxio_endorsement":
                    listTitle = EndorsementDocumentUrlTitle;
                    break;
                case "legal entity":
                case "adoxio_legalentity":
                    listTitle = LegalEntityDocumentUrlTitle;
                    break;
                case "establishment incident":
                case "adoxio_establishmentincident":
                    listTitle = EstablishmentIncidentDocumentUrlTitle;
                    break;
                case "incident":
                case "adoxio_incident":
                    listTitle = IncidentDocumentUrlTitle;
                    break;
                default:
                    listTitle = entityName;
                    break;
            }

            return listTitle;
        }
    }
}
