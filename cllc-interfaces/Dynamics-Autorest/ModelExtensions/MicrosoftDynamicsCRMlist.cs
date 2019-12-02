namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;

    public partial class MicrosoftDynamicsCRMlist
    {

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "listlead_association@odata.bind")]
        public string Listlead_associationODataBind { get; set; }


    }
}
