using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AccountExtensions
    {

        /// <summary>
        /// Copy values from a Dynamics legal entity to another one
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this Account to, Account from)
        {
            to.Accountid = from.Accountid;
            to.Name = from.Name;
            to.Description = from.Description;
			to.Adoxio_externalid = from.Adoxio_externalid;
        }

        /// <summary>
        /// Copy values from a Dynamics legal entity to a view model.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this Account to, ViewModels.Account from)
        {
            to.Name = from.name;
            to.Description = from.description;
			to.Adoxio_externalid = from.externalId;
        }

        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.Account ToViewModel(this Account account)
        {
            ViewModels.Account result = null;
            if (account != null)
            {
                result = new ViewModels.Account();
                if (account.Accountid != null)
                {
                    result.id = account.Accountid.ToString();
                }

                result.name = account.Name;
                result.description = account.Description;
				result.externalId = account.Adoxio_externalid;

                if (account._primarycontactid_value != null)
                {
                    // add the primary contact.
                    result.primarycontact = new ViewModels.Contact();
                    result.primarycontact.id = account._primarycontactid_value.ToString();
                    // TODO - load other fields (if necessary)
                }
            }            
            return result;
        }   
        
        public static Account ToModel(this ViewModels.Account account)
        {
            Account result = null;
            if (account != null)
            {
                result = new Account();
                if (string.IsNullOrEmpty(account.id))
                {
                    result.Accountid = new Guid();
                }
                else
                {
                    result.Accountid = new Guid(account.id);
                }
                result.Name = account.name;
				result.Adoxio_externalid = account.externalId;
            }
            return result;
        }
    }
}
