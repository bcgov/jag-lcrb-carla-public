using System;
using System.Collections.Generic;
namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class BusinessProfile {
        public ViewModels.Account Account { get; set; }
        /// <summary>
        /// The List of all legal entities in the account
        /// </summary>
        /// <value></value>
        public LegalEntity Applicant {get; set; }
    }

    public class LegalEntity
    {
       /// <summary>
       /// This is the adoxio_Account for the Applicant
       /// or adoxio_shareholderAccount for Shareholders
       /// </summary>
       /// <value></value>
        public ViewModels.Account Account { get; set; }

        public ViewModels.AdoxioLegalEntity AdoxioLegalEntity {get; set; }
        public List<LegalEntity> ChildEntities {get; set; }

        /// <summary>
        /// The tied house associated with the <see cref="Account"/>
        /// </summary>
        /// <value></value>
        public ViewModels.TiedHouseConnection TiedHouse { get; set; }
        
    }

}