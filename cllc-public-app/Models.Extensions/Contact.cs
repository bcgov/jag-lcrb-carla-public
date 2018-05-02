using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ContactExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.Contact ToViewModel(this Contact contact)
        {
            ViewModels.Contact result = null;
            if (contact != null)
            {
                result = new ViewModels.Contact();
                if (contact.Contactid != null)
                {
                    result.id = contact.Contactid.ToString();
                }

                result.name = contact.Fullname;                
            }            
            return result;
        }   
        
        public static Contact ToModel(this ViewModels.Contact contact)
        {
            Contact result = null;
            if (contact != null)
            {
                result = new Contact();
                if (! string.IsNullOrEmpty(contact.id))
                {
                    result.Contactid = new Guid(contact.id);
                }
                result.Fullname = contact.name;
                result.Emailaddress1 = contact.Emailaddress1;
                result.Firstname = contact.firstname;
                result.Lastname = contact.lastname;
                if (string.IsNullOrEmpty(result.Fullname) && (!string.IsNullOrEmpty(result.Firstname) || !string.IsNullOrEmpty(result.Lastname)))
                {
                    result.Fullname = "";
                    if (!string.IsNullOrEmpty(result.Firstname))
                    {
                        result.Fullname += result.Firstname;
                    }
                    if (!string.IsNullOrEmpty(result.Lastname))
                    {
                        if (!string.IsNullOrEmpty(result.Fullname))
                        {
                            result.Fullname += " ";
                        }
                        result.Fullname += result.Lastname;
                    }
                }
            }
            return result;
        }
    }
}
