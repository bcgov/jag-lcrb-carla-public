// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Microsoft.Dynamics.CRM.dataperformance
    /// </summary>
    public partial class MicrosoftDynamicsCRMdataperformance
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMdataperformance class.
        /// </summary>
        public MicrosoftDynamicsCRMdataperformance()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMdataperformance class.
        /// </summary>
        public MicrosoftDynamicsCRMdataperformance(string _organizationidValue = default(string), int? count = default(int?), decimal? optimizationstorage = default(decimal?), bool? anyoptimizationavailable = default(bool?), string dataperformanceid = default(string), string lastactionresult = default(string), string component = default(string), decimal? weight = default(decimal?), decimal? mediantime = default(decimal?), string entity = default(string), decimal? estimatedoptimizationimpact = default(decimal?), string solution = default(string), string executionperiod = default(string), string optimizationstatus = default(string), decimal? maxtime = default(decimal?), bool? anyoptimizationapplied = default(bool?), string realizedoptimizationimpact = default(string), System.DateTimeOffset? lastoptimizationdate = default(System.DateTimeOffset?), decimal? mintime = default(decimal?), string operation = default(string), MicrosoftDynamicsCRMorganization organizationid = default(MicrosoftDynamicsCRMorganization))
        {
            this._organizationidValue = _organizationidValue;
            Count = count;
            Optimizationstorage = optimizationstorage;
            Anyoptimizationavailable = anyoptimizationavailable;
            Dataperformanceid = dataperformanceid;
            Lastactionresult = lastactionresult;
            Component = component;
            Weight = weight;
            Mediantime = mediantime;
            Entity = entity;
            Estimatedoptimizationimpact = estimatedoptimizationimpact;
            Solution = solution;
            Executionperiod = executionperiod;
            Optimizationstatus = optimizationstatus;
            Maxtime = maxtime;
            Anyoptimizationapplied = anyoptimizationapplied;
            Realizedoptimizationimpact = realizedoptimizationimpact;
            Lastoptimizationdate = lastoptimizationdate;
            Mintime = mintime;
            Operation = operation;
            Organizationid = organizationid;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_organizationid_value")]
        public string _organizationidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int? Count { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "optimizationstorage")]
        public decimal? Optimizationstorage { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "anyoptimizationavailable")]
        public bool? Anyoptimizationavailable { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "dataperformanceid")]
        public string Dataperformanceid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lastactionresult")]
        public string Lastactionresult { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "component")]
        public string Component { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "weight")]
        public decimal? Weight { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "mediantime")]
        public decimal? Mediantime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "entity")]
        public string Entity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "estimatedoptimizationimpact")]
        public decimal? Estimatedoptimizationimpact { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "solution")]
        public string Solution { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "executionperiod")]
        public string Executionperiod { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "optimizationstatus")]
        public string Optimizationstatus { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "maxtime")]
        public decimal? Maxtime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "anyoptimizationapplied")]
        public bool? Anyoptimizationapplied { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "realizedoptimizationimpact")]
        public string Realizedoptimizationimpact { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lastoptimizationdate")]
        public System.DateTimeOffset? Lastoptimizationdate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "mintime")]
        public decimal? Mintime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "operation")]
        public string Operation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "organizationid")]
        public MicrosoftDynamicsCRMorganization Organizationid { get; set; }

    }
}
