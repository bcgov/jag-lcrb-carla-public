using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_LegalEntityExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.AdoxioLegalEntity ToViewModel(this Adoxio_legalentity adoxio_legalentity)
        {
            ViewModels.AdoxioLegalEntity result = null;
            if (adoxio_legalentity != null)
            {
                result = new ViewModels.AdoxioLegalEntity();
                if (adoxio_legalentity.Adoxio_legalentityid != null)
                {
                    result.adoxio_legalentityid = adoxio_legalentity.Adoxio_legalentityid.ToString();
                }
                result.adoxio_name = adoxio_legalentity.Adoxio_name;
                                
            }            
            return result;
        }   
        
    }
}
