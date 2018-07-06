using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum Adoxio_stateprovince
    {
        [Display(Name = "Alberta")]
        AB = 845280000,
        [Display(Name = "British Columbia")]
        BC,
        [Display(Name = "Manitoba")]
        MN,
        [Display(Name = "New Brunswick")]
        NB,
        [Display(Name = "Newfoundland and Labrador")]
        NL,
        [Display(Name = "Northwest Territories")]
        NT,
        [Display(Name = "Nova Scotia")]
        NS,
        [Display(Name = "Nunavut")]
        NU,
        [Display(Name = "Ontario")]
        ON,
        [Display(Name = "Prince Edward Island")]
        PE,
        [Display(Name = "Quebec")]
        QC,
        [Display(Name = "Saskatchewan")]
        SK,
        [Display(Name = "Yukon")]
        YT
    }

    public class Account
    {
        public string id { get; set; }
        public string name { get; set; } //dynamics = name
        public string description { get; set; } //dynamics = description
        public string externalId { get; set; }
        public string bcIncorporationNumber { get; set; } //dynamics = adoxio_bcincorporationnumber
        public DateTimeOffset? dateOfIncorporationInBC { get; set; } //dynamics = adoxio_dateofincorporationinbc
        public string businessNumber { get; set; } //dynamics = accountnumber
        public string pstNumber { get; set; } //dynamics = adoxio_pstnumber
        public string contactEmail { get; set; } //dynamics = emailaddress1
        public string contactPhone { get; set; } //dynamics = telephone1
        public string mailingAddressName { get; set; } //dynamics = address1_name
        public string mailingAddressStreet { get; set; } //dynamics = address1_line1
        public string mailingAddressCity { get; set; } //dynamics = address1_city
        public string mailingAddressCountry { get; set; } //dynamics = address1_country
        [JsonConverter(typeof(StringEnumConverter))]
        public Adoxio_stateprovince mailingAddressProvince { get; set; } //dynamics = adoxio_stateprovince
        public string mailingAddresPostalCode { get; set; } //dynamics = address1_postalcode

        public ViewModels.Contact primarycontact { get; set; }

        public string businessType { get; set; }

    }
}
