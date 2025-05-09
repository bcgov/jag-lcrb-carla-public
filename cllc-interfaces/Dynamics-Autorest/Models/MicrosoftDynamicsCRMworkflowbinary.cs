// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Microsoft.Dynamics.CRM.workflowbinary
    /// </summary>
    public partial class MicrosoftDynamicsCRMworkflowbinary
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMworkflowbinary class.
        /// </summary>
        public MicrosoftDynamicsCRMworkflowbinary()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMworkflowbinary class.
        /// </summary>
        public MicrosoftDynamicsCRMworkflowbinary(int? timezoneruleversionnumber = default(int?), int? statuscode = default(int?), string versionnumber = default(string), string mimetype = default(string), string metadata = default(string), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), string _owningteamValue = default(string), string dataName = default(string), string _owninguserValue = default(string), string _processValue = default(string), string type = default(string), string workflowbinaryid = default(string), int? statecode = default(int?), int? utcconversiontimezonecode = default(int?), string _createdonbehalfbyValue = default(string), string _owneridValue = default(string), string _modifiedonbehalfbyValue = default(string), string _owningbusinessunitValue = default(string), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), int? importsequencenumber = default(int?), string _createdbyValue = default(string), string _flowsessionidValue = default(string), byte[] data = default(byte[]), string referencename = default(string), string _modifiedbyValue = default(string), string name = default(string), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser owninguser = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMteam owningteam = default(MicrosoftDynamicsCRMteam), MicrosoftDynamicsCRMprincipal ownerid = default(MicrosoftDynamicsCRMprincipal), MicrosoftDynamicsCRMbusinessunit owningbusinessunit = default(MicrosoftDynamicsCRMbusinessunit), IList<MicrosoftDynamicsCRMsyncerror> workflowbinarySyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMasyncoperation> workflowbinaryAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> workflowbinaryMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> workflowbinaryProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> workflowbinaryBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> workflowbinaryPrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), IList<MicrosoftDynamicsCRMfileattachment> workflowbinaryFileAttachments = default(IList<MicrosoftDynamicsCRMfileattachment>), MicrosoftDynamicsCRMflowsession flowSessionId = default(MicrosoftDynamicsCRMflowsession), MicrosoftDynamicsCRMworkflow process = default(MicrosoftDynamicsCRMworkflow))
        {
            Timezoneruleversionnumber = timezoneruleversionnumber;
            Statuscode = statuscode;
            Versionnumber = versionnumber;
            Mimetype = mimetype;
            Metadata = metadata;
            Overriddencreatedon = overriddencreatedon;
            this._owningteamValue = _owningteamValue;
            DataName = dataName;
            this._owninguserValue = _owninguserValue;
            this._processValue = _processValue;
            Type = type;
            Workflowbinaryid = workflowbinaryid;
            Statecode = statecode;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            this._owneridValue = _owneridValue;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            this._owningbusinessunitValue = _owningbusinessunitValue;
            Createdon = createdon;
            Importsequencenumber = importsequencenumber;
            this._createdbyValue = _createdbyValue;
            this._flowsessionidValue = _flowsessionidValue;
            Data = data;
            Referencename = referencename;
            this._modifiedbyValue = _modifiedbyValue;
            Name = name;
            Modifiedon = modifiedon;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Owninguser = owninguser;
            Owningteam = owningteam;
            Ownerid = ownerid;
            Owningbusinessunit = owningbusinessunit;
            WorkflowbinarySyncErrors = workflowbinarySyncErrors;
            WorkflowbinaryAsyncOperations = workflowbinaryAsyncOperations;
            WorkflowbinaryMailboxTrackingFolders = workflowbinaryMailboxTrackingFolders;
            WorkflowbinaryProcessSession = workflowbinaryProcessSession;
            WorkflowbinaryBulkDeleteFailures = workflowbinaryBulkDeleteFailures;
            WorkflowbinaryPrincipalObjectAttributeAccesses = workflowbinaryPrincipalObjectAttributeAccesses;
            WorkflowbinaryFileAttachments = workflowbinaryFileAttachments;
            FlowSessionId = flowSessionId;
            Process = process;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

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
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "mimetype")]
        public string Mimetype { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public string Metadata { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningteam_value")]
        public string _owningteamValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data_name")]
        public string DataName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owninguser_value")]
        public string _owninguserValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_process_value")]
        public string _processValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowbinaryid")]
        public string Workflowbinaryid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statecode")]
        public int? Statecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_ownerid_value")]
        public string _owneridValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningbusinessunit_value")]
        public string _owningbusinessunitValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

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
        [JsonProperty(PropertyName = "_flowsessionid_value")]
        public string _flowsessionidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonConverter(typeof(Base64UrlJsonConverter))]
        [JsonProperty(PropertyName = "data")]
        public byte[] Data { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "referencename")]
        public string Referencename { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

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
        [JsonProperty(PropertyName = "workflowbinary_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> WorkflowbinarySyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowbinary_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> WorkflowbinaryAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowbinary_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> WorkflowbinaryMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowbinary_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> WorkflowbinaryProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowbinary_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> WorkflowbinaryBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowbinary_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> WorkflowbinaryPrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowbinary_FileAttachments")]
        public IList<MicrosoftDynamicsCRMfileattachment> WorkflowbinaryFileAttachments { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "FlowSessionId")]
        public MicrosoftDynamicsCRMflowsession FlowSessionId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Process")]
        public MicrosoftDynamicsCRMworkflow Process { get; set; }

    }
}
