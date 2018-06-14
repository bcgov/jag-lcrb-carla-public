using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class UserExtensions
    {

        /// <summary>
        /// Copy values from a Dynamics legal entity to another one
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void FromContact(this User to, Contact from)
        {
            if (from.Contactid != null)
            {
                to.ContactId = (Guid)from.Contactid;
            }
            
            if (from._parentcustomerid_value != null)
            {
                to.AccountId = (Guid)from._parentcustomerid_value;
            }
            
            to.GivenName = from.Firstname;
            to.Surname = from.Lastname;
            to.SmUserId = from.Employeeid;
            to.Email = from.Emailaddress1;
            to.Active = true;
        }
    }
}
