using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum IdentificationType
    {
        DriversLicence = 845280000,
        BCIdCard = 845280005
    }

    public enum YesNoOptions {
        Yes = 845280000,
        No = 845280001
    }

    public class Contact
    {
        public string id { get; set; }
        public string name { get; set; }

        public string firstname { get; set; }

        public string middlename { get; set; }

        public string lastname { get; set; }

        public string emailaddress1 { get; set; }

        public string jobTitle { get; set; }

        public string telephone1 { get; set; }

        public string address1_line1 { get; set; }

        public string address1_city { get; set; }

        public string address1_country { get; set; }

        public string address1_stateorprovince { get; set; }

        public string address1_postalcode { get; set; }

        public string address2_line1 { get; set; }

        public string address2_city { get; set; }

        public string address2_country { get; set; }

        public string address2_stateorprovince { get; set; }

        public string address2_postalcode { get; set; }

        public Boolean? adoxio_cansignpermanentchangeapplications { get; set; }

        public Boolean? adoxio_canattendeducationsessions { get; set; }

        public Boolean? adoxio_cansigntemporarychangeapplications { get; set; }

        public Boolean? adoxio_canattendcompliancemeetings { get; set; }

        public Boolean? adoxio_canobtainlicenceinfofrombranch { get; set; }

        public Boolean? adoxio_canrepresentlicenseeathearings { get; set; }

        public Boolean? adoxio_cansigngrocerystoreproofofsalesrevenue { get; set; }

        public DateTimeOffset? Birthdate { get; set; }


        public string BirthPlace { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Gender? Gender { get; set; }

        public string MobilePhone { get; set; }
        public string PrimaryIdNumber { get; set; }
        public string SecondaryIdNumber { get; set; }
        public bool? IsWorker { get; set; }
        public int? SelfDisclosure { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public IdentificationType? SecondaryIdentificationType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public IdentificationType? PrimaryIdentificationType { get; set; }

        public string PhsConnectionsDetails { get; set; }
        public DateTimeOffset? PhsDateSubmitted { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoOptions? PhsLivesInCanada { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoOptions? PhsExpired { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoOptions? PhsComplete { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoOptions? PhsConnectionsToOtherLicences { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoOptions? PhsCanadianDrugAlchoholDrivingOffence { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public YesNoOptions? PhsForeignDrugAlchoholOffence { get; set; }
        
    }
}
