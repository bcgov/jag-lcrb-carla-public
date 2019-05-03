using System;
namespace SpdSync.models
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }

        public Address Address { get; set; }
    }
}
