using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioSpecialeventCollection
    {
        [JsonProperty(PropertyName = "@odata.nextLink")]
        public string OdataNextLink { get; set; }

        [JsonProperty(PropertyName = "@odata.count")]
        public string Count { get; set; }
    }
}
