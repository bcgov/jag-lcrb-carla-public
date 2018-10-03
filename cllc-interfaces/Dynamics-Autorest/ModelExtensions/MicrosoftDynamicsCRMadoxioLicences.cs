
namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class MicrosoftDynamicsCRMadoxioLicences
    {
        
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_account_adoxio_legalentity_Account@odata.bind")]
        public string AdoxioAccountAdoxioLegalentityAccountODataBind { get; set; }



        //enable accessing properties using string names
        public object this[string propertyName]
        {
            get {
                if (propertyName.IndexOf(".") > -1)
                {
                    string[] tokens = propertyName.Split(".");

                    var property = this.GetType().GetProperty(tokens[0]).GetValue(this, null);
                    return property.GetType().GetProperty(tokens[1]).GetValue(this, null);
                }
                else
                {
                    return this.GetType().GetProperty(propertyName).GetValue(this, null);
                }
            }
                
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_termsandconditions@odata.bind")]
        public string adoxio_termsandconditions { get; set; }

    }
}
