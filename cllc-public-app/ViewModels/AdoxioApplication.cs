using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AdoxioApplication
    {
		public string id { get; set; } //adoxio_applicationid
        public string name { get; set; } //adoxio_name
        public string applyingPerson { get; set; } //_adoxio_applyingperson_value
        public string jobNumber { get; set; } //adoxio_jobnumber
        public string licenseType { get; set; } //_adoxio_licencetype_value
        public string establishmentName { get; set; } //adoxio_establishmentpropsedname
        public string establishmentaddressstreet { get; set; } //adoxio_establishmentaddressstreet
        public string establishmentaddresscity { get; set; } //adoxio_establishmentaddresscity
        public string establishmentaddresspostalcode { get; set; } //adoxio_establishmentaddresspostalcode
        public string establishmentAddress { get; set; } //adoxio_establishmentaddress
        public string applicationStatus { get; set; } //statuscode
        public Adoxio_applicanttypecodes applicantType { get; set; } //adoxio_applicanttype
        public GeneralYesNo registeredEstablishment { get; set; } //adoxio_registeredestablishment
        public string establishmentparcelid { get; set; } //adoxio_establishmentparcelid
        public string additionalpropertyinformation { get; set; } //adoxio_additionalpropertyinformation
        public bool? authorizedtosubmit { get; set; } //adoxio_authorizedtosubmit
        public bool? signatureagreement { get; set; } //adoxio_signatureagreement
        public string contactpersonfirstname { get; set; } //adoxio_contactpersonfirstname
        public string contactpersonlastname { get; set; } //adoxio_contactpersonlastname
        public string contactpersonrole { get; set; } //adoxio_role
        public string contactpersonemail { get; set; } //adoxio_email
        public string contactpersonphone { get; set; } //adoxio_contactpersonphone

		public GeneralYesNo adoxioInvoiceTrigger { get; set; } //adoxio_invoicetrigger

		public ViewModels.Account applicant { get; set; }
    }
}
