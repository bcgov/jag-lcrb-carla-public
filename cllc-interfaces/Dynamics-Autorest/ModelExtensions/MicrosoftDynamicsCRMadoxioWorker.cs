namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public enum SecurityStatusPicklist
    {
        PASS = 845280000,
        FAIL = 845280001,
        WITHDRAWN = 845280003
    }

    class MicrosoftDynamicsCRMadoxioWorkerMetadata
    {
        // 2020-12-11 ASR - Removed extra annotations for "adoxio_dateofbirth" to fix data serialization error
    }

    [MetadataType(typeof(MicrosoftDynamicsCRMadoxioWorkerMetadata))]
    public partial class MicrosoftDynamicsCRMadoxioWorker
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_ContactId@odata.bind")]
        public string ContactIdAccountODataBind { get; set; }
    }
}
