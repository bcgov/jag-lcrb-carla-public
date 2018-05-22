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
                    result.id = adoxio_legalentity.Adoxio_legalentityid.ToString();
                }
                result.name = adoxio_legalentity.Adoxio_name;
                               
            }            
            return result;
        }


        public static void CopyValues(this Adoxio_legalentity to, Adoxio_legalentity from)
        {
            to.Adoxio_legalentityid = from.Adoxio_legalentityid;
            to.Adoxio_name = from.Adoxio_name;
        }

        public static void CopyValues(this Adoxio_legalentity to, ViewModels.AdoxioLegalEntity from)
        {
            to.Adoxio_legalentityid = new Guid(from.id);
            to.Adoxio_name = from.name;
        }


        public static Adoxio_legalentity ToModel(this ViewModels.AdoxioLegalEntity adoxioLegalEntity)
        {
            Adoxio_legalentity result = null;
            if (adoxioLegalEntity != null)
            {
                result = new Adoxio_legalentity();
                result.Adoxio_legalentityid = new Guid(adoxioLegalEntity.id);
                result.Adoxio_name = adoxioLegalEntity.name;
            }
            return result;
        }

    }
}
