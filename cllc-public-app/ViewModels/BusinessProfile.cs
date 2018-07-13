using System;
using System.Collections.Generic;
namespace Gov.Lclb.Cllb.Public.ViewModels {
    public class BusinessProfile {
        public ViewModels.Account Account { get; set; }
        /// <summary>
        /// The List of all legal entities in the account
        /// </summary>
        /// <value></value>
        public List<LegalEntity> LegalEntities { get; set; }
    }

    public class ProfileValidation {
        public string LegalEntityId { get; set; }
        public bool IsComplete { get; set; }
    }

    public class LegalEntity {
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

        public bool IsComplete(){
            var result = Account.isCorporateDetailsComplete(AdoxioLegalEntity.legalentitytype)
                        && AdoxioLegalEntity.isDirectorOfficerComplete()
                        && AdoxioLegalEntity.isShareholderComplete()
                        && TiedHouse.isConnectionToProducersComplete(AdoxioLegalEntity.legalentitytype)
                        && corporateDetailsFilesExists
                        && organizationStructureFilesExists
                        && keyPersonnelFilesExists
                        && financialInformationFilesExists
                        && shareholderFilesExists;
            return result;
        }

    }

}