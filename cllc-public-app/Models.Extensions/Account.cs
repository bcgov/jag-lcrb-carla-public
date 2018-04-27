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
            }            
            return result;
        }   
        
        public static Account ToModel(this ViewModels.Account account)
        {
            Account result = null;
            if (account != null)
            {
                if (! string.IsNullOrEmpty(account.id))
                {
                    result.Accountid = new Guid(account.id);
                }
                result.Name = account.name;                
            }
            return result;
        }
    }
}
