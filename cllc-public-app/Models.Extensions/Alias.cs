extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Xrm.Sdk;
using System;
using adoxio_alias = DV::Gov.Lclb.Cllb.Interfaces.adoxio_alias;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AliasExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        

        public static ViewModels.Alias ToViewModel(this adoxio_alias alias)
        {
            if (alias == null) return null;
            var result = new ViewModels.Alias();
            result.id = alias.Id.ToString();
            result.firstname = alias.adoxio_FirstName;
            result.middlename = alias.adoxio_MiddleName;
            result.lastname = alias.adoxio_LastName;
            return result;
        }

        public static void CopyValues(this adoxio_alias to, ViewModels.Alias from, string contactId = null)
        {
            to.adoxio_FirstName = from.firstname;
            to.adoxio_MiddleName = from.middlename;
            to.adoxio_LastName = from.lastname;
            if (!string.IsNullOrEmpty(contactId))
                to.adoxio_ContactId = new EntityReference(DvContact.EntityLogicalName, new Guid(contactId));
        }
    }
}
