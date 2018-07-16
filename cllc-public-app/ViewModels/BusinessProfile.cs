using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class BusinessProfile
    {
        public ViewModels.Account Account { get; set; }
        /// <summary>
        /// The List of all legal entities in the account
        /// </summary>
        /// <value></value>
        public List<LegalEntity> LegalEntities { get; set; }
    }

    public class ProfileValidation
    {
        public string LegalEntityId { get; set; }
        public bool IsComplete { get; set; }
    }

    public class LegalEntity
    {
        /// <summary>
        /// This is the adoxio_Account for the Applicant
        /// or adoxio_shareholderAccount for Shareholders
        /// </summary>
        /// <value></value>
        public ViewModels.Account Account { get; set; }

        public ViewModels.AdoxioLegalEntity AdoxioLegalEntity { get; set; }
        public List<LegalEntity> ChildEntities { get; set; }

        /// <summary>
        /// The tied house associated with the <see cref="Account"/>
        /// </summary>
        /// <value></value>
        public ViewModels.TiedHouseConnection TiedHouse { get; set; }

        public bool corporateDetailsFilesExists { get; set; }
        public bool organizationStructureFilesExists { get; set; }
        public bool keyPersonnelFilesExists { get; set; }
        public bool financialInformationFilesExists { get; set; }
        public bool shareholderFilesExists { get; set; }

        public bool IsComplete()
        {
            Adoxio_applicanttypecodes? businessType = null;
            if (Account.businessType != null)
            {
                businessType = (Adoxio_applicanttypecodes?)Enum.Parse(typeof(Adoxio_applicanttypecodes), Account.businessType);
            }

            var directorsExist = ChildEntities.Any(c => c.AdoxioLegalEntity.isDirector == true
                                                || c.AdoxioLegalEntity.isOfficer == true
                                                || c.AdoxioLegalEntity.isSeniorManagement == true);

            var shareholdersExist = ChildEntities.Any(c => c.AdoxioLegalEntity.isShareholder == true);
            var partnersExist = ChildEntities.Any(c => c.AdoxioLegalEntity.isPartner == true);

            var isShareholderComplete = 
                ChildEntities.Where(c => c.AdoxioLegalEntity.isShareholder == true)
                    .Select( c => c.AdoxioLegalEntity.isShareholderComplete(businessType, shareholderFilesExists, shareholdersExist, partnersExist))
                    .All(s => s == true);
            var isDirectorAndOfficersComplete =  
                ChildEntities.Where(c => c.AdoxioLegalEntity.isDirector == true
                                                || c.AdoxioLegalEntity.isOfficer == true
                                                || c.AdoxioLegalEntity.isSeniorManagement == true)
                    .Select( c => c.AdoxioLegalEntity.isDirectorOfficerComplete(businessType, directorsExist))
                    .All(s => s == true);


            var result = Account.isCorporateDetailsComplete(businessType, corporateDetailsFilesExists)
                        && isDirectorAndOfficersComplete
                        && isShareholderComplete
                        && TiedHouse.isConnectionToProducersComplete(businessType)
                        && organizationStructureFilesExists
                        && keyPersonnelFilesExists
                        && financialInformationFilesExists;
            return result;
        }

    }

}