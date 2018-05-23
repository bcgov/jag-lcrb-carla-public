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
        }

        /// <summary>
        /// Copy values from a Dynamics legal entity to a view model.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this Account to, ViewModels.Account from)
        {
            to.Accountid = new Guid(from.id);
            to.Name = from.name;
            to.Description = from.description;
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
            }
            return result;
        }
    }
}
