using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    
    public class CovidApplication
    {
        public string Id { get; set; } //adoxio_applicationid

        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicantTypeCodes ApplicantType { get; set; } //adoxio_applicanttype

        public string JobNumber { get; set; } //adoxio_jobnumber
        public string NameOfApplicant { get; set; } 

        public string AdditionalPropertyInformation { get; set; } //adoxio_additionalpropertyinformation
        public string InvoiceId  { get; set; }
        public string ApplyingPerson { get; set; } //_adoxio_applyingperson_value
        public bool? AuthorizedToSubmit { get; set; } //adoxio_authorizedtosubmit        

        public DateTimeOffset? CreatedOn { get; set; }
        public string ContactPersonEmail { get; set; } //adoxio_email
        public string ContactPersonFirstName { get; set; } //adoxio_contactpersonfirstname
        public string ContactPersonLastName { get; set; } //adoxio_contactpersonlastname
        public string ContactPersonPhone { get; set; } //adoxio_contactpersonphone
        public string ContactPersonRole { get; set; } //adoxio_role

        public string AddressCity { get; set; } //adoxio_addresscity
        public string AddressPostalCode { get; set; } //adoxio_addresspostalcode
        public string AddressStreet { get; set; } //adoxio_addressstreet

        public string EstablishmentAddress { get; set; } //adoxio_establishmentaddress
        public string EstablishmentName { get; set; } //adoxio_establishmentpropsedname
        public string EstablishmentAddressCity { get; set; } //adoxio_establishmentaddresscity
        public string EstablishmentAddressPostalCode { get; set; } //adoxio_establishmentaddresspostalcode
        public string EstablishmentAddressStreet { get; set; } //adoxio_establishmentaddressstreet
        public string EstablishmentEmail { get; set; }
        public string EstablishmentParcelId { get; set; } //adoxio_establishmentparcelid
        public string EstablishmentPhone { get; set; }
                
        public string LicenceType { get; set; } //_adoxio_licencetype_value		
        
        public DateTimeOffset? ModifiedOn { get; set; }

        public string Name { get; set; } //adoxio_name
     
        public ApplicationType ApplicationType { get; set; }

        public string Description1 { get; set; }


        [JsonConverter(typeof(StringEnumConverter))]
        public GeneralYesNo? IsApplicationComplete { get; set; }

        public bool? ProposedEstablishmentIsAlr { get; set; }
    }
}
