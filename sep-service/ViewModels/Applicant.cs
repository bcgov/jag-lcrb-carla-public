using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public class Applicant
    {
        public Address Address { get; set; }
        public string ApplicantName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
