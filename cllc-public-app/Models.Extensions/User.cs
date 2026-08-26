extern alias DV;
using System;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;

namespace Gov.Lclb.Cllb.Public.Models
{
    public static class UserExtensions
    {
        public static void FromContact(this User to, DvContact from)
        {
            to.ContactId = from.Id;
            to.AccountId = from.ParentCustomerId?.Id ?? Guid.Empty;
            to.GivenName = from.FirstName;
            to.Surname = from.LastName;
            to.SmUserId = from.EmployeeId;
            to.Email = from.EMailAddress1;
            to.Active = true;
        }
    }
}
