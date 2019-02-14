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
        Cancelled = 2,
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
        Terminated = 845280009,
        [EnumMember(Value = "Terminated and refunded")]
        TerminatedAndRefunded = 845280010
    }

    public enum AdoxioFinalDecisionCodes
    {
        Approved = 845280000,
        Denied = 845280001
    }

    public enum ServiceHours
    {
        [EnumMember(Value = "00:00")]
        sh0000 = 845280000,

        [EnumMember(Value = "00:15")]
        sh0015 = 845280024,

        [EnumMember(Value = "00:30")]
        sh0030 = 845280025,

        [EnumMember(Value = "00:45")]
        sh0045 = 845280026,

        [EnumMember(Value = "01:00")]
        sh0100 = 845280001,

        [EnumMember(Value = "01:15")]
        sh0115 = 845280027,

        [EnumMember(Value = "01:30")]
        sh0130 = 845280028,

        [EnumMember(Value = "01:45")]
        sh0145 = 845280029,

        [EnumMember(Value = "02:00")]
        sh0200 = 845280002,

        [EnumMember(Value = "02:15")]
        sh0215 = 845280030,

        [EnumMember(Value = "02:30")]
        sh0230 = 845280031,

        [EnumMember(Value = "02:45")]
        sh0245 = 845280032,

        [EnumMember(Value = "03:00")]
        sh0300 = 845280003,

        [EnumMember(Value = "03:15")]
        sh0315 = 845280033,

        [EnumMember(Value = "03:30")]
        sh0330 = 845280034,

        [EnumMember(Value = "03:45")]
        sh0345 = 845280035,

        [EnumMember(Value = "04:00")]
        sh0400 = 845280004,

        [EnumMember(Value = "04:15")]
        sh0415 = 845280036,

        [EnumMember(Value = "04:30")]
        sh0430 = 845280037,

        [EnumMember(Value = "04:45")]
        sh0445 = 845280038,

        [EnumMember(Value = "05:00")]
        sh0500 = 845280005,

        [EnumMember(Value = "05:15")]
        sh0515 = 845280039,

        [EnumMember(Value = "05:30")]
        sh0530 = 845280040,

        [EnumMember(Value = "05:45")]
        sh0545 = 845280041,

        [EnumMember(Value = "06:00")]
        sh0600 = 845280006,

        [EnumMember(Value = "06:15")]
        sh0615 = 845280042,

        [EnumMember(Value = "06:30")]
        sh0630 = 845280043,

        [EnumMember(Value = "06:45")]
        sh0645 = 845280044,

        [EnumMember(Value = "07:00")]
        sh0700 = 845280007,

        [EnumMember(Value = "07:15")]
        sh0715 = 845280045,

        [EnumMember(Value = "07:30")]
        sh0730 = 845280046,

        [EnumMember(Value = "07:45")]
        sh0745 = 845280047,

        [EnumMember(Value = "08:00")]
        sh0800 = 845280008,

        [EnumMember(Value = "08:15")]
        sh0815 = 845280048,

        [EnumMember(Value = "08:30")]
        sh0830 = 845280049,

        [EnumMember(Value = "08:45")]
        sh0845 = 845280050,

        [EnumMember(Value = "09:00")]
        sh0900 = 845280009,

        [EnumMember(Value = "09:15")]
        sh0915 = 845280051,

        [EnumMember(Value = "09:30")]
        sh0930 = 845280052,

        [EnumMember(Value = "09:45")]
        sh0945 = 845280053,

        [EnumMember(Value = "10:00")]
        sh1000 = 845280010,

        [EnumMember(Value = "10:15")]
        sh1015 = 845280054,

        [EnumMember(Value = "10:30")]
        sh1030 = 845280055,

        [EnumMember(Value = "10:45")]
        sh1045 = 845280056,

        [EnumMember(Value = "11:00")]
        sh1100 = 845280011,

        [EnumMember(Value = "11:15")]
        sh1115 = 845280057,

        [EnumMember(Value = "11:30")]
        sh1130 = 845280058,

        [EnumMember(Value = "11:45")]
        sh1145 = 845280059,

        [EnumMember(Value = "12:00")]
        sh1200 = 845280012,

        [EnumMember(Value = "12:15")]
        sh1215 = 845280060,

        [EnumMember(Value = "12:30")]
        sh1230 = 845280061,

        [EnumMember(Value = "12:45")]
        sh1245 = 845280062,

        [EnumMember(Value = "13:00")]
        sh1300 = 845280013,

        [EnumMember(Value = "13:15")]
        sh1315 = 845280063,

        [EnumMember(Value = "13:30")]
        sh1330 = 845280064,

        [EnumMember(Value = "13:45")]
        sh1345 = 845280065,

        [EnumMember(Value = "14:00")]
        sh1400 = 845280014,

        [EnumMember(Value = "14:15")]
        sh1415 = 845280066,

        [EnumMember(Value = "14:30")]
        sh1430 = 845280067,

        [EnumMember(Value = "14:45")]
        sh1445 = 845280068,

        [EnumMember(Value = "15:00")]
        sh1500 = 845280015,

        [EnumMember(Value = "15:15")]
        sh1515 = 845280069,

        [EnumMember(Value = "15:30")]
        sh1530 = 845280070,

        [EnumMember(Value = "15:45")]
        sh1545 = 845280071,

        [EnumMember(Value = "16:00")]
        sh1600 = 845280016,

        [EnumMember(Value = "16:15")]
        sh1615 = 845280072,

        [EnumMember(Value = "16:30")]
        sh1630 = 845280073,

        [EnumMember(Value = "16:45")]
        sh1645 = 845280074,

        [EnumMember(Value = "17:00")]
        sh1700 = 845280017,

        [EnumMember(Value = "17:15")]
        sh1715 = 845280075,

        [EnumMember(Value = "17:30")]
        sh1730 = 845280076,

        [EnumMember(Value = "17:45")]
        sh1745 = 845280077,

        [EnumMember(Value = "18:00")]
        sh1800 = 845280018,

        [EnumMember(Value = "18:15")]
        sh1815 = 845280078,

        [EnumMember(Value = "18:30")]
        sh1830 = 845280079,

        [EnumMember(Value = "18:45")]
        sh1845 = 845280080,

        [EnumMember(Value = "19:00")]
        sh1900 = 845280019,

        [EnumMember(Value = "19:15")]
        sh1915 = 845280081,

        [EnumMember(Value = "19:30")]
        sh1930 = 845280082,

        [EnumMember(Value = "19:45")]
        sh1945 = 845280083,

        [EnumMember(Value = "20:00")]
        sh2000 = 845280020,

        [EnumMember(Value = "20:15")]
        sh2015 = 845280084,

        [EnumMember(Value = "20:30")]
        sh2030 = 845280085,

        [EnumMember(Value = "20:45")]
        sh2045 = 845280086,

        [EnumMember(Value = "21:00")]
        sh2100 = 845280021,

        [EnumMember(Value = "21:15")]
        sh2115 = 845280087,

        [EnumMember(Value = "21:30")]
        sh2130 = 845280088,

        [EnumMember(Value = "21:45")]
        sh2145 = 845280089,

        [EnumMember(Value = "22:00")]
        sh2200 = 845280022,

        [EnumMember(Value = "22:15")]
        sh2215 = 845280090,

        [EnumMember(Value = "22:30")]
        sh2230 = 845280091,

        [EnumMember(Value = "22:45")]
        sh2245 = 845280092,

        [EnumMember(Value = "23:00")]
        sh2300 = 845280023,

        [EnumMember(Value = "23:15")]
        sh2315 = 845280093,

        [EnumMember(Value = "23:30")]
        sh2330 = 845280094,

        [EnumMember(Value = "23:45")]
        sh2345 = 845280095
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

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioFinalDecisionCodes AppChecklistFinalDecision { get; set; } //adoxioFinaldecision
        public string adoxioInvoiceId;
        public bool isSubmitted { get; set; }
        public bool isPaid { get; set; }
        public bool prevPaymentFailed { get; set; }
        public DateTimeOffset? paymentreceiveddate { get; set; }
        public DateTimeOffset? modifiedOn { get; set; }

        public bool licenceFeeInvoicePaid { get; set; }

        public DateTimeOffset? createdon { get; set; }
        public DateTimeOffset? modifiedon { get; set; }


        public ViewModels.Account applicant { get; set; }

        public ViewModels.Invoice licenceFeeInvoice { get; set; }

        public ViewModels.AdoxioLicense assignedLicence { get; set; }

        public bool? ServicehHoursStandardHours { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSundayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSundayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursMondayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursMondayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursTuesdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursTuesdayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursWednesdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursWednesdayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursThursdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursThursdayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursFridayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursFridayClose { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSaturdayOpen { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHours? ServiceHoursSaturdayClose { get; set; }


    }
}
