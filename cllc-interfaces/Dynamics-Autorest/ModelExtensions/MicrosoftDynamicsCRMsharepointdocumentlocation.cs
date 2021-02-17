namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMsharepointdocumentlocation
    {

        [JsonProperty(PropertyName = "regardingobjectid_adoxio_application@odata.bind")]
        public string RegardingobjectidAdoxioApplicationODataBind { get; set; }

        [JsonProperty(PropertyName = "parentsiteorlocation_sharepointsite@odata.bind")]
        public string ParentSiteODataBind { get; set; }

        [JsonProperty(PropertyName = "regardingobjectid_adoxio_worker@odata.bind")]
        public string RegardingobjectidWorkerApplicationODataBind { get; set; }

        [JsonProperty(PropertyName = "regardingobjectid_account@odata.bind")]
        public string RegardingobjectIdAccountODataBind { get; set; }

        [JsonProperty(PropertyName = "regardingobjectid_contact@odata.bind")]
        public string RegardingobjectIdContactODataBind { get; set; }

        [JsonProperty(PropertyName = "regardingobjectid_adoxio_event@odata.bind")]
        public string RegardingobjectIdEventODataBind { get; set; }

        [JsonProperty(PropertyName = "regardingobjectid_adoxio_licences@odata.bind")]
        public string RegardingobjectIdLicenceODataBind { get; set; }

        [JsonProperty(PropertyName = "regardingobjectid_adoxio_federalreportexport@odata.bind")]
        public string RegardingobjectIdFederalReportExportODataBind { get; set; }


        [JsonProperty(PropertyName = "parentsiteorlocation_sharepointdocumentlocation@odata.bind")]
        public string ParentsiteorlocationSharepointdocumentlocationODataBind { get; set; }

    }
}
