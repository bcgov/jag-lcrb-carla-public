namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System;
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

        [JsonProperty(PropertyName = "adoxio_ApplicationTypeId@odata.bind")]
        public string AdoxioApplicationTypeIdODataBind { get; set; }

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

        [JsonProperty(PropertyName = "adoxio_LicenceEstablishment@odata.bind")]
        public string AdoxioLicenceEstablishmentODataBind { get; set; }


        //format date here
        [JsonProperty(PropertyName = "adoxio_establishmentopeningdate")]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public System.DateTimeOffset? AdoxioEstablishmentopeningdate { get; set; }



        [JsonProperty(PropertyName = "adoxio_isreadyvalidinterest")]
        public bool? IsReadyValidInterest { get; set; }

        [JsonProperty(PropertyName = "adoxio_isreadyworkers")]
        public bool? IsReadyWorkers { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadynamebranding")]
        public bool? IsReadyNameBranding { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadydisplays")]
        public bool? IsReadyDisplays { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadyintruderalarm")]
        public bool? IsReadyIntruderAlarm { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadyfirealarm")]
        public bool? IsReadyFireAlarm { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadylockedcases")]
        public bool? IsReadyLockedCases { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadylockedstorage")]
        public bool? IsReadyLockedStorage { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadyperimeter")]
        public bool? IsReadyPerimeter { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadyretailarea")]
        public bool? IsReadyRetailArea { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadystorage")]
        public bool? IsReadyStorage { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadyentranceexit")]
        public bool? IsReadyExtranceExit { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadysurveillancenotice")]
        public bool? IsReadySurveillanceNotice { get; set; }
        
        [JsonProperty(PropertyName = "adoxio_isreadyproductnotvisibleoutside")]
        public bool? IsReadyProductNotVisibleOutside { get; set; }


    }
}
