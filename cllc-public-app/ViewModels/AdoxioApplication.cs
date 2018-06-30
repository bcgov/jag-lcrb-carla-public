using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AdoxioApplication
    {
		public string id { get; set; } //adoxio_applicationid
        public string name { get; set; }
        public string applyingPerson { get; set; }
        public string jobNumber { get; set; }
        public string licenseType { get; set; } //adoxio_licencetype
        public string establishmentName { get; set; }
		public string establishmentaddressstreet { get; set; }
		public string establishmentaddresscity { get; set; }
		public string establishmentaddresspostalcode { get; set; }
		public string establishmentAddress { get; set; }
        public string applicationStatus { get; set; }
        public Adoxio_applicanttypecodes applicantType { get; set; } //adoxio_applicanttype
        public GeneralYesNo registeredEstablishment { get; set; } //adoxio_registeredestablishment

        public ViewModels.Account applicant { get; set; }
    }
}
