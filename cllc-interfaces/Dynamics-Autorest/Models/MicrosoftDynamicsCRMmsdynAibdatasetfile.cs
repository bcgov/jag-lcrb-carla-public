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
    /// Microsoft.Dynamics.CRM.msdyn_aibdatasetfile
    /// </summary>
    public partial class MicrosoftDynamicsCRMmsdynAibdatasetfile
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMmsdynAibdatasetfile class.
        /// </summary>
        public MicrosoftDynamicsCRMmsdynAibdatasetfile()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMmsdynAibdatasetfile class.
        /// </summary>
        public MicrosoftDynamicsCRMmsdynAibdatasetfile(System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), int? utcconversiontimezonecode = default(int?), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), string _msdynAibfileidValue = default(string), string _owneridValue = default(string), int? statecode = default(int?), string _owningbusinessunitValue = default(string), string _createdonbehalfbyValue = default(string), string _msdynAibdatasetidValue = default(string), string versionnumber = default(string), System.DateTimeOffset? msdynLastmodifieddate = default(System.DateTimeOffset?), string _createdbyValue = default(string), string msdynAibdatasetfileid = default(string), string msdynName = default(string), int? timezoneruleversionnumber = default(int?), int? statuscode = default(int?), int? importsequencenumber = default(int?), string _owningteamValue = default(string), string _modifiedbyValue = default(string), string _modifiedonbehalfbyValue = default(string), string _owninguserValue = default(string), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser owninguser = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMteam owningteam = default(MicrosoftDynamicsCRMteam), MicrosoftDynamicsCRMprincipal ownerid = default(MicrosoftDynamicsCRMprincipal), MicrosoftDynamicsCRMbusinessunit owningbusinessunit = default(MicrosoftDynamicsCRMbusinessunit), IList<MicrosoftDynamicsCRMsyncerror> msdynAibdatasetfileSyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMduplicaterecord> msdynAibdatasetfileDuplicateMatchingRecord = default(IList<MicrosoftDynamicsCRMduplicaterecord>), IList<MicrosoftDynamicsCRMduplicaterecord> msdynAibdatasetfileDuplicateBaseRecord = default(IList<MicrosoftDynamicsCRMduplicaterecord>), IList<MicrosoftDynamicsCRMasyncoperation> msdynAibdatasetfileAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> msdynAibdatasetfileMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> msdynAibdatasetfileProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> msdynAibdatasetfileBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> msdynAibdatasetfilePrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), MicrosoftDynamicsCRMmsdynAibdataset msdynAIBDatasetId = default(MicrosoftDynamicsCRMmsdynAibdataset), MicrosoftDynamicsCRMmsdynAibfile msdynAIBFileId = default(MicrosoftDynamicsCRMmsdynAibfile), IList<MicrosoftDynamicsCRMmsdynAibfileattacheddata> msdynAIBFileAttachedDataMsdynAIB = default(IList<MicrosoftDynamicsCRMmsdynAibfileattacheddata>))
        {
            Modifiedon = modifiedon;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            Createdon = createdon;
            this._msdynAibfileidValue = _msdynAibfileidValue;
            this._owneridValue = _owneridValue;
            Statecode = statecode;
            this._owningbusinessunitValue = _owningbusinessunitValue;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            this._msdynAibdatasetidValue = _msdynAibdatasetidValue;
            Versionnumber = versionnumber;
            MsdynLastmodifieddate = msdynLastmodifieddate;
            this._createdbyValue = _createdbyValue;
            MsdynAibdatasetfileid = msdynAibdatasetfileid;
            MsdynName = msdynName;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            Statuscode = statuscode;
            Importsequencenumber = importsequencenumber;
            this._owningteamValue = _owningteamValue;
            this._modifiedbyValue = _modifiedbyValue;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            this._owninguserValue = _owninguserValue;
            Overriddencreatedon = overriddencreatedon;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Owninguser = owninguser;
            Owningteam = owningteam;
            Ownerid = ownerid;
            Owningbusinessunit = owningbusinessunit;
            MsdynAibdatasetfileSyncErrors = msdynAibdatasetfileSyncErrors;
            MsdynAibdatasetfileDuplicateMatchingRecord = msdynAibdatasetfileDuplicateMatchingRecord;
            MsdynAibdatasetfileDuplicateBaseRecord = msdynAibdatasetfileDuplicateBaseRecord;
            MsdynAibdatasetfileAsyncOperations = msdynAibdatasetfileAsyncOperations;
            MsdynAibdatasetfileMailboxTrackingFolders = msdynAibdatasetfileMailboxTrackingFolders;
            MsdynAibdatasetfileProcessSession = msdynAibdatasetfileProcessSession;
            MsdynAibdatasetfileBulkDeleteFailures = msdynAibdatasetfileBulkDeleteFailures;
            MsdynAibdatasetfilePrincipalObjectAttributeAccesses = msdynAibdatasetfilePrincipalObjectAttributeAccesses;
            MsdynAIBDatasetId = msdynAIBDatasetId;
            MsdynAIBFileId = msdynAIBFileId;
            MsdynAIBFileAttachedDataMsdynAIB = msdynAIBFileAttachedDataMsdynAIB;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_msdyn_aibfileid_value")]
        public string _msdynAibfileidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_ownerid_value")]
        public string _owneridValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statecode")]
        public int? Statecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningbusinessunit_value")]
        public string _owningbusinessunitValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_msdyn_aibdatasetid_value")]
        public string _msdynAibdatasetidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_lastmodifieddate")]
        public System.DateTimeOffset? MsdynLastmodifieddate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdby_value")]
        public string _createdbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfileid")]
        public string MsdynAibdatasetfileid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_name")]
        public string MsdynName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timezoneruleversionnumber")]
        public int? Timezoneruleversionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statuscode")]
        public int? Statuscode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningteam_value")]
        public string _owningteamValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owninguser_value")]
        public string _owninguserValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

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
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> MsdynAibdatasetfileSyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_DuplicateMatchingRecord")]
        public IList<MicrosoftDynamicsCRMduplicaterecord> MsdynAibdatasetfileDuplicateMatchingRecord { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_DuplicateBaseRecord")]
        public IList<MicrosoftDynamicsCRMduplicaterecord> MsdynAibdatasetfileDuplicateBaseRecord { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> MsdynAibdatasetfileAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> MsdynAibdatasetfileMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> MsdynAibdatasetfileProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> MsdynAibdatasetfileBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_aibdatasetfile_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> MsdynAibdatasetfilePrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_AIBDatasetId")]
        public MicrosoftDynamicsCRMmsdynAibdataset MsdynAIBDatasetId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_AIBFileId")]
        public MicrosoftDynamicsCRMmsdynAibfile MsdynAIBFileId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_AIBFileAttachedData_msdyn_AIB")]
        public IList<MicrosoftDynamicsCRMmsdynAibfileattacheddata> MsdynAIBFileAttachedDataMsdynAIB { get; set; }

    }
}
