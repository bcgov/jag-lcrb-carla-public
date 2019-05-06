using System;
namespace SpdSync.models
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Address Address { get; set; }
    }
}
