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
    /// Microsoft.Dynamics.CRM.datalakeworkspacepermission
    /// </summary>
    public partial class MicrosoftDynamicsCRMdatalakeworkspacepermission
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMdatalakeworkspacepermission class.
        /// </summary>
        public MicrosoftDynamicsCRMdatalakeworkspacepermission()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMdatalakeworkspacepermission class.
        /// </summary>
        public MicrosoftDynamicsCRMdatalakeworkspacepermission(int? statecode = default(int?), string versionnumber = default(string), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), string solutionid = default(string), string tenantid = default(string), string datalakeworkspacepermissionUniquename = default(string), string _modifiedbyValue = default(string), bool? canexecute = default(bool?), int? importsequencenumber = default(int?), string _createdbyValue = default(string), string _workspaceidValue = default(string), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), MicrosoftDynamicsCRMBooleanManagedProperty iscustomizable = default(MicrosoftDynamicsCRMBooleanManagedProperty), string _createdonbehalfbyValue = default(string), string componentidunique = default(string), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), bool? ismanaged = default(bool?), string whitelistedappid = default(string), bool? canread = default(bool?), string datalakeworkspacepermissionid = default(string), string appid = default(string), int? timezoneruleversionnumber = default(int?), string _organizationidValue = default(string), string _modifiedonbehalfbyValue = default(string), bool? canwrite = default(bool?), int? statuscode = default(int?), int? utcconversiontimezonecode = default(int?), string name = default(string), System.DateTimeOffset? overwritetime = default(System.DateTimeOffset?), int? componentstate = default(int?), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMorganization organizationid = default(MicrosoftDynamicsCRMorganization), IList<MicrosoftDynamicsCRMsyncerror> datalakeworkspacepermissionSyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMduplicaterecord> datalakeworkspacepermissionDuplicateMatchingRecord = default(IList<MicrosoftDynamicsCRMduplicaterecord>), IList<MicrosoftDynamicsCRMduplicaterecord> datalakeworkspacepermissionDuplicateBaseRecord = default(IList<MicrosoftDynamicsCRMduplicaterecord>), IList<MicrosoftDynamicsCRMasyncoperation> datalakeworkspacepermissionAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> datalakeworkspacepermissionMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> datalakeworkspacepermissionProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> datalakeworkspacepermissionBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> datalakeworkspacepermissionPrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), MicrosoftDynamicsCRMdatalakeworkspace workspaceid = default(MicrosoftDynamicsCRMdatalakeworkspace))
        {
            Statecode = statecode;
            Versionnumber = versionnumber;
            Modifiedon = modifiedon;
            Solutionid = solutionid;
            Tenantid = tenantid;
            DatalakeworkspacepermissionUniquename = datalakeworkspacepermissionUniquename;
            this._modifiedbyValue = _modifiedbyValue;
            Canexecute = canexecute;
            Importsequencenumber = importsequencenumber;
            this._createdbyValue = _createdbyValue;
            this._workspaceidValue = _workspaceidValue;
            Overriddencreatedon = overriddencreatedon;
            Iscustomizable = iscustomizable;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            Componentidunique = componentidunique;
            Createdon = createdon;
            Ismanaged = ismanaged;
            Whitelistedappid = whitelistedappid;
            Canread = canread;
            Datalakeworkspacepermissionid = datalakeworkspacepermissionid;
            Appid = appid;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            this._organizationidValue = _organizationidValue;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            Canwrite = canwrite;
            Statuscode = statuscode;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            Name = name;
            Overwritetime = overwritetime;
            Componentstate = componentstate;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Organizationid = organizationid;
            DatalakeworkspacepermissionSyncErrors = datalakeworkspacepermissionSyncErrors;
            DatalakeworkspacepermissionDuplicateMatchingRecord = datalakeworkspacepermissionDuplicateMatchingRecord;
            DatalakeworkspacepermissionDuplicateBaseRecord = datalakeworkspacepermissionDuplicateBaseRecord;
            DatalakeworkspacepermissionAsyncOperations = datalakeworkspacepermissionAsyncOperations;
            DatalakeworkspacepermissionMailboxTrackingFolders = datalakeworkspacepermissionMailboxTrackingFolders;
            DatalakeworkspacepermissionProcessSession = datalakeworkspacepermissionProcessSession;
            DatalakeworkspacepermissionBulkDeleteFailures = datalakeworkspacepermissionBulkDeleteFailures;
            DatalakeworkspacepermissionPrincipalObjectAttributeAccesses = datalakeworkspacepermissionPrincipalObjectAttributeAccesses;
            Workspaceid = workspaceid;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statecode")]
        public int? Statecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "solutionid")]
        public string Solutionid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "tenantid")]
        public string Tenantid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_uniquename")]
        public string DatalakeworkspacepermissionUniquename { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "canexecute")]
        public bool? Canexecute { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdby_value")]
        public string _createdbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_workspaceid_value")]
        public string _workspaceidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "iscustomizable")]
        public MicrosoftDynamicsCRMBooleanManagedProperty Iscustomizable { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "componentidunique")]
        public string Componentidunique { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ismanaged")]
        public bool? Ismanaged { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "whitelistedappid")]
        public string Whitelistedappid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "canread")]
        public bool? Canread { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermissionid")]
        public string Datalakeworkspacepermissionid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "appid")]
        public string Appid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timezoneruleversionnumber")]
        public int? Timezoneruleversionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_organizationid_value")]
        public string _organizationidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "canwrite")]
        public bool? Canwrite { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statuscode")]
        public int? Statuscode { get; set; }

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
        [JsonProperty(PropertyName = "overwritetime")]
        public System.DateTimeOffset? Overwritetime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "componentstate")]
        public int? Componentstate { get; set; }

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
        [JsonProperty(PropertyName = "organizationid")]
        public MicrosoftDynamicsCRMorganization Organizationid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> DatalakeworkspacepermissionSyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_DuplicateMatchingRecord")]
        public IList<MicrosoftDynamicsCRMduplicaterecord> DatalakeworkspacepermissionDuplicateMatchingRecord { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_DuplicateBaseRecord")]
        public IList<MicrosoftDynamicsCRMduplicaterecord> DatalakeworkspacepermissionDuplicateBaseRecord { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> DatalakeworkspacepermissionAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> DatalakeworkspacepermissionMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> DatalakeworkspacepermissionProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> DatalakeworkspacepermissionBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "datalakeworkspacepermission_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> DatalakeworkspacepermissionPrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workspaceid")]
        public MicrosoftDynamicsCRMdatalakeworkspace Workspaceid { get; set; }

    }
}
