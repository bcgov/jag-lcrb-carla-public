namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMadoxioApplication
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_Applicant@odata.bind")]
        public string AdoxioApplicantODataBind { get; set; }

        [JsonProperty(PropertyName = "statuscode@odata.bind")]
        public string StatusCodeODataBind { get; set; }

        //adoxio_LicenceFeeInvoice
        [JsonProperty(PropertyName = "adoxio_LicenceType@odata.bind")]
        public string AdoxioLicenceTypeODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_LicenceFeeInvoice@odata.bind")]
        public string AdoxioLicenceFeeInvoiceODataBind { get; set; }

        [JsonProperty(PropertyName = "adoxio_Invoice@odata.bind")]
        public string AdoxioInvoiceODataBind { get; set; }

        
        [JsonProperty(PropertyName = "adoxio_AssignedLicence@odata.bind")]
        public string AdoxioAssignedLicenceODataBind { get; set; }


        [JsonProperty(PropertyName = "adoxio_application_SharePointDocumentLocations@odata.bind")]
        public string[] AdoxioApplicationSharePointDocumentLocationsODataBind { get; set; }

        
        [JsonProperty(PropertyName = "adoxio_localgovindigenousnationid@odata.bind")]
        public string AdoxioLocalgovindigenousnationidODataBind { get; set; }
    }
}
