using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{

    class MicrosoftDynamicsCRMcontactMetadata
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty(PropertyName = "birthdate")]
        public System.DateTimeOffset? Birthdate { get; set; }
    }
   
    [MetadataType(typeof(MicrosoftDynamicsCRMcontactMetadata))]
    public partial class MicrosoftDynamicsCRMcontact
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "parentcustomerid_account@odata.bind")]
        public string ParentCustomerIdAccountODataBind { get; set; }     
    }

    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
