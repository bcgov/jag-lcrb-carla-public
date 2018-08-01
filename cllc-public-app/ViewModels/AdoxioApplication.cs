using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum AdoxioApplicationStatusCodes
    {
        Active = 1,
        [EnumMember(Value = "In Progress")]
        InProgress = 845280000,
        Intake = 845280001,
        [EnumMember(Value = "Pending for LG/FN/Police Feedback")]
        PendingForLGFNPFeedback = 845280006,
        [EnumMember(Value = "Under Review")]
        UnderReview = 845280003,
        [EnumMember(Value = "Pending for Licence Fee")]
        PendingForLicenceFee = 845280007,        
        Approved = 845280004,
        Denied = 845280005,
        [EnumMember(Value = "Approved in Principle")]
        ApprovedInPrinciple = 845280008,
        Terminated = 845280009
    }

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
        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicationStatusCodes applicationStatus { get; set; } //statuscode
        public AdoxioApplicantTypeCodes applicantType { get; set; } //adoxio_applicanttype
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
		public string adoxioInvoiceId;
		public bool isSubmitted { get; set; }
		public bool isPaid { get; set; }
		public bool prevPaymentFailed { get; set; }
        public DateTimeOffset? paymentreceiveddate { get; set; }
        public DateTimeOffset? modifiedOn { get; set; }

        public DateTimeOffset? createdon { get; set; }
        public DateTimeOffset? modifiedon { get; set; }


        public ViewModels.Account applicant { get; set; }
    

    }
}
