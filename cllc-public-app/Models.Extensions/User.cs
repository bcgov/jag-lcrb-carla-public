using Gov.Lclb.Cllb.Interfaces.Models;
using System;

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
        public static void FromContact(this User to, MicrosoftDynamicsCRMcontact from)
        {
            if (from.Contactid != null)
            {
                to.ContactId = Guid.Parse(from.Contactid);
            }
            
            if (from._parentcustomeridValue != null)
            {
                to.AccountId = Guid.Parse(from._parentcustomeridValue);
            }
            
            to.GivenName = from.Firstname;
            to.Surname = from.Lastname;
            to.SmUserId = from.Employeeid;
            to.Email = from.Emailaddress1;
            to.Active = true;
        }
    }
}
