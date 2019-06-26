using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Gov.Lclb.Cllb.Interfaces.Models
{

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
