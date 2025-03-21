// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Microsoft.Dynamics.CRM.leadproduct
    /// </summary>
    public partial class MicrosoftDynamicsCRMleadproduct
    {
        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMleadproduct
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMleadproduct()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMleadproduct
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMleadproduct(string leadproductid = default(string), string leadid = default(string), string versionnumber = default(string), string productid = default(string), int? timezoneruleversionnumber = default(int?), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), int? importsequencenumber = default(int?), int? utcconversiontimezonecode = default(int?), string name = default(string), IList<MicrosoftDynamicsCRMteam> leadproductTeams = default(IList<MicrosoftDynamicsCRMteam>), IList<MicrosoftDynamicsCRMasyncoperation> leadproductAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> leadproductMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMbulkdeletefailure> leadproductBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> leadproductPrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>))
        {
            Leadproductid = leadproductid;
            Leadid = leadid;
            Versionnumber = versionnumber;
            Productid = productid;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            Overriddencreatedon = overriddencreatedon;
            Importsequencenumber = importsequencenumber;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            Name = name;
            LeadproductTeams = leadproductTeams;
            LeadproductAsyncOperations = leadproductAsyncOperations;
            LeadproductMailboxTrackingFolders = leadproductMailboxTrackingFolders;
            LeadproductBulkDeleteFailures = leadproductBulkDeleteFailures;
            LeadproductPrincipalObjectAttributeAccesses = leadproductPrincipalObjectAttributeAccesses;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "leadproductid")]
        public string Leadproductid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "leadid")]
        public string Leadid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "productid")]
        public string Productid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timezoneruleversionnumber")]
        public int? Timezoneruleversionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "leadproduct_Teams")]
        public IList<MicrosoftDynamicsCRMteam> LeadproductTeams { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "leadproduct_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> LeadproductAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "leadproduct_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> LeadproductMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "leadproduct_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> LeadproductBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "leadproduct_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> LeadproductPrincipalObjectAttributeAccesses { get; set; }

    }
}
