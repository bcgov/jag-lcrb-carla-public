using System;
using System.Collections.Generic;
using System.Text;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Interfaces
{
    /// <summary>
    /// Extensions to support documents for entities.
    /// </summary>
    public static class EntityDocumentExtensions
    {
        public static string CleanGuidForSharePoint(string guidString)
        {
            string result = null;
            if (guidString != null)
            {
                result = guidString.ToUpper().Replace("-", "");
            }
            return result;
        }

        public static FolderSegment GetDocumentFolderName(this MicrosoftDynamicsCRMaccount account)
        {
            string accountIdCleaned = CleanGuidForSharePoint(account.Accountid);

            return new FolderSegment
            {
                FolderName = $"{account.Name}_{accountIdCleaned}",
                FolderNameSegment = account.Name,
                FolderGuidSegment = accountIdCleaned,
            };
        }

        public static FolderSegment GetDocumentFolderName(
            this MicrosoftDynamicsCRMadoxioApplication application
        )
        {
            string applicationIdCleaned = CleanGuidForSharePoint(application.AdoxioApplicationid);

            return new FolderSegment
            {
                FolderName = $"{application.AdoxioJobnumber}_{applicationIdCleaned}",
                FolderNameSegment = application.AdoxioJobnumber,
                FolderGuidSegment = applicationIdCleaned,
            };
        }

        public static FolderSegment GetDocumentFolderName(this MicrosoftDynamicsCRMcontact contact)
        {
            string contactIdCleaned = CleanGuidForSharePoint(contact.Contactid);

            return new FolderSegment
            {
                FolderName = $"contact_{contactIdCleaned}",
                FolderNameSegment = "contact",
                FolderGuidSegment = contactIdCleaned,
            };
        }

        public static FolderSegment GetDocumentFolderName(
            this MicrosoftDynamicsCRMadoxioWorker worker
        )
        {
            string applicationIdCleaned = CleanGuidForSharePoint(worker.AdoxioWorkerid);

            return new FolderSegment
            {
                FolderName = $"{worker.AdoxioName}_{applicationIdCleaned}",
                FolderNameSegment = worker.AdoxioName,
                FolderGuidSegment = applicationIdCleaned,
            };
        }

        public static FolderSegment GetDocumentFolderName(
            this MicrosoftDynamicsCRMadoxioEvent eventEntity
        )
        {
            string entityIdCleaned = CleanGuidForSharePoint(eventEntity.AdoxioEventid);

            return new FolderSegment
            {
                FolderName = $"{eventEntity.AdoxioName}_{entityIdCleaned}",
                FolderNameSegment = eventEntity.AdoxioName,
                FolderGuidSegment = entityIdCleaned,
            };
        }

        public static FolderSegment GetDocumentFolderName(
            this MicrosoftDynamicsCRMadoxioFederalreportexport exportEntity
        )
        {
            string entityIdCleaned = CleanGuidForSharePoint(
                exportEntity.AdoxioFederalreportexportid
            );

            return new FolderSegment
            {
                FolderName = $"{exportEntity.AdoxioName}_{entityIdCleaned}",
                FolderNameSegment = exportEntity.AdoxioName,
                FolderGuidSegment = entityIdCleaned,
            };
        }

        public static FolderSegment GetDocumentFolderName(
            this MicrosoftDynamicsCRMadoxioLicences licence
        )
        {
            string licenceIdCleaned = CleanGuidForSharePoint(licence.AdoxioLicencesid);

            return new FolderSegment
            {
                FolderName = $"{licence.AdoxioName}_{licenceIdCleaned}",
                FolderNameSegment = licence.AdoxioName,
                FolderGuidSegment = licenceIdCleaned,
            };
        }

        public static FolderSegment GetDocumentFolderName(
            this MicrosoftDynamicsCRMadoxioSpecialevent specialEvent
        )
        {
            string idCleaned = CleanGuidForSharePoint(specialEvent.AdoxioSpecialeventid);

            return new FolderSegment
            {
                FolderName = $"{specialEvent.AdoxioEventname}_{idCleaned}",
                FolderNameSegment = specialEvent.AdoxioEventname,
                FolderGuidSegment = idCleaned,
            };
        }
    }
}
