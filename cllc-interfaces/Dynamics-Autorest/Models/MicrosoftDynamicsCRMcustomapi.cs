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
    /// Microsoft.Dynamics.CRM.customapi
    /// </summary>
    public partial class MicrosoftDynamicsCRMcustomapi
    {
        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMcustomapi
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMcustomapi()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMcustomapi
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMcustomapi(string solutionid = default(string), string _owningteamValue = default(string), int? allowedcustomprocessingsteptype = default(int?), string _createdonbehalfbyValue = default(string), string iscustomizable = default(string), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), string componentidunique = default(string), string _owningbusinessunitValue = default(string), string uniquename = default(string), int? componentstate = default(int?), int? statuscode = default(int?), bool? isprivate = default(bool?), bool? ismanaged = default(bool?), string _owninguserValue = default(string), int? utcconversiontimezonecode = default(int?), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), string executeprivilegename = default(string), string description = default(string), int? timezoneruleversionnumber = default(int?), string _createdbyValue = default(string), string boundentitylogicalname = default(string), string displayname = default(string), string _modifiedonbehalfbyValue = default(string), string _modifiedbyValue = default(string), string _sdkmessageidValue = default(string), string _plugintypeidValue = default(string), string name = default(string), int? statecode = default(int?), string _owneridValue = default(string), System.DateTimeOffset? overwritetime = default(System.DateTimeOffset?), string versionnumber = default(string), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), int? bindingtype = default(int?), string customapiid = default(string), int? importsequencenumber = default(int?), bool? isfunction = default(bool?), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser owninguser = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMteam owningteam = default(MicrosoftDynamicsCRMteam), MicrosoftDynamicsCRMprincipal ownerid = default(MicrosoftDynamicsCRMprincipal), MicrosoftDynamicsCRMbusinessunit owningbusinessunit = default(MicrosoftDynamicsCRMbusinessunit), IList<MicrosoftDynamicsCRMsyncerror> customapiSyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMasyncoperation> customapiAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> customapiMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> customapiProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> customapiBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> customapiPrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), IList<MicrosoftDynamicsCRMcustomapirequestparameter> customAPIRequestParameters = default(IList<MicrosoftDynamicsCRMcustomapirequestparameter>), IList<MicrosoftDynamicsCRMcustomapiresponseproperty> customAPIResponseProperties = default(IList<MicrosoftDynamicsCRMcustomapiresponseproperty>), IList<MicrosoftDynamicsCRMcatalogassignment> catalogAssignments = default(IList<MicrosoftDynamicsCRMcatalogassignment>), MicrosoftDynamicsCRMsdkmessage sdkMessageId = default(MicrosoftDynamicsCRMsdkmessage), MicrosoftDynamicsCRMplugintype pluginTypeId = default(MicrosoftDynamicsCRMplugintype))
        {
            Solutionid = solutionid;
            this._owningteamValue = _owningteamValue;
            Allowedcustomprocessingsteptype = allowedcustomprocessingsteptype;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            Iscustomizable = iscustomizable;
            Modifiedon = modifiedon;
            Componentidunique = componentidunique;
            this._owningbusinessunitValue = _owningbusinessunitValue;
            Uniquename = uniquename;
            Componentstate = componentstate;
            Statuscode = statuscode;
            Isprivate = isprivate;
            Ismanaged = ismanaged;
            this._owninguserValue = _owninguserValue;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            Overriddencreatedon = overriddencreatedon;
            Executeprivilegename = executeprivilegename;
            Description = description;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            this._createdbyValue = _createdbyValue;
            Boundentitylogicalname = boundentitylogicalname;
            Displayname = displayname;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            this._modifiedbyValue = _modifiedbyValue;
            this._sdkmessageidValue = _sdkmessageidValue;
            this._plugintypeidValue = _plugintypeidValue;
            Name = name;
            Statecode = statecode;
            this._owneridValue = _owneridValue;
            Overwritetime = overwritetime;
            Versionnumber = versionnumber;
            Createdon = createdon;
            Bindingtype = bindingtype;
            Customapiid = customapiid;
            Importsequencenumber = importsequencenumber;
            Isfunction = isfunction;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Owninguser = owninguser;
            Owningteam = owningteam;
            Ownerid = ownerid;
            Owningbusinessunit = owningbusinessunit;
            CustomapiSyncErrors = customapiSyncErrors;
            CustomapiAsyncOperations = customapiAsyncOperations;
            CustomapiMailboxTrackingFolders = customapiMailboxTrackingFolders;
            CustomapiProcessSession = customapiProcessSession;
            CustomapiBulkDeleteFailures = customapiBulkDeleteFailures;
            CustomapiPrincipalObjectAttributeAccesses = customapiPrincipalObjectAttributeAccesses;
            CustomAPIRequestParameters = customAPIRequestParameters;
            CustomAPIResponseProperties = customAPIResponseProperties;
            CatalogAssignments = catalogAssignments;
            SdkMessageId = sdkMessageId;
            PluginTypeId = pluginTypeId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "solutionid")]
        public string Solutionid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningteam_value")]
        public string _owningteamValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "allowedcustomprocessingsteptype")]
        public int? Allowedcustomprocessingsteptype { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "iscustomizable")]
        public string Iscustomizable { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "componentidunique")]
        public string Componentidunique { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningbusinessunit_value")]
        public string _owningbusinessunitValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "uniquename")]
        public string Uniquename { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "componentstate")]
        public int? Componentstate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statuscode")]
        public int? Statuscode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isprivate")]
        public bool? Isprivate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ismanaged")]
        public bool? Ismanaged { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owninguser_value")]
        public string _owninguserValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "executeprivilegename")]
        public string Executeprivilegename { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timezoneruleversionnumber")]
        public int? Timezoneruleversionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdby_value")]
        public string _createdbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "boundentitylogicalname")]
        public string Boundentitylogicalname { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "displayname")]
        public string Displayname { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_sdkmessageid_value")]
        public string _sdkmessageidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_plugintypeid_value")]
        public string _plugintypeidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statecode")]
        public int? Statecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_ownerid_value")]
        public string _owneridValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overwritetime")]
        public System.DateTimeOffset? Overwritetime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "bindingtype")]
        public int? Bindingtype { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customapiid")]
        public string Customapiid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isfunction")]
        public bool? Isfunction { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdby")]
        public MicrosoftDynamicsCRMsystemuser Createdby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdonbehalfby")]
        public MicrosoftDynamicsCRMsystemuser Createdonbehalfby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedby")]
        public MicrosoftDynamicsCRMsystemuser Modifiedby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedonbehalfby")]
        public MicrosoftDynamicsCRMsystemuser Modifiedonbehalfby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "owninguser")]
        public MicrosoftDynamicsCRMsystemuser Owninguser { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "owningteam")]
        public MicrosoftDynamicsCRMteam Owningteam { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ownerid")]
        public MicrosoftDynamicsCRMprincipal Ownerid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "owningbusinessunit")]
        public MicrosoftDynamicsCRMbusinessunit Owningbusinessunit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customapi_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> CustomapiSyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customapi_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> CustomapiAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customapi_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> CustomapiMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customapi_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> CustomapiProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customapi_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> CustomapiBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "customapi_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> CustomapiPrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CustomAPIRequestParameters")]
        public IList<MicrosoftDynamicsCRMcustomapirequestparameter> CustomAPIRequestParameters { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CustomAPIResponseProperties")]
        public IList<MicrosoftDynamicsCRMcustomapiresponseproperty> CustomAPIResponseProperties { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "CatalogAssignments")]
        public IList<MicrosoftDynamicsCRMcatalogassignment> CatalogAssignments { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SdkMessageId")]
        public MicrosoftDynamicsCRMsdkmessage SdkMessageId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PluginTypeId")]
        public MicrosoftDynamicsCRMplugintype PluginTypeId { get; set; }

    }
}