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
    /// Microsoft.Dynamics.CRM.adoxio_applicationcrsbpfv4
    /// </summary>
    public partial class MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4 class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4 class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4(string _createdonbehalfbyValue = default(string), System.DateTimeOffset? activestagestartedon = default(System.DateTimeOffset?), string _bpfAdoxioApplicationidValue = default(string), int? utcconversiontimezonecode = default(int?), string _activestageidValue = default(string), int? statecode = default(int?), int? bpfDuration = default(int?), int? statuscode = default(int?), int? timezoneruleversionnumber = default(int?), System.DateTimeOffset? completedon = default(System.DateTimeOffset?), string _createdbyValue = default(string), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), int? importsequencenumber = default(int?), string businessprocessflowinstanceid = default(string), string _processidValue = default(string), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), string _modifiedonbehalfbyValue = default(string), string traversedpath = default(string), string bpfName = default(string), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), string _organizationidValue = default(string), string _modifiedbyValue = default(string), string versionnumber = default(string), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMorganization organizationid = default(MicrosoftDynamicsCRMorganization), MicrosoftDynamicsCRMprocessstage activestageid = default(MicrosoftDynamicsCRMprocessstage), MicrosoftDynamicsCRMworkflow processid = default(MicrosoftDynamicsCRMworkflow), IList<MicrosoftDynamicsCRMsyncerror> adoxioApplicationcrsbpfv4SyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMasyncoperation> adoxioApplicationcrsbpfv4AsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMworkflowlog> adoxioApplicationcrsbpfv4WorkflowLogs = default(IList<MicrosoftDynamicsCRMworkflowlog>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> adoxioApplicationcrsbpfv4MailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> adoxioApplicationcrsbpfv4ProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> adoxioApplicationcrsbpfv4BulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> adoxioApplicationcrsbpfv4PrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), MicrosoftDynamicsCRMadoxioApplication bpfAdoxioApplicationid = default(MicrosoftDynamicsCRMadoxioApplication))
        {
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            Activestagestartedon = activestagestartedon;
            this._bpfAdoxioApplicationidValue = _bpfAdoxioApplicationidValue;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            this._activestageidValue = _activestageidValue;
            Statecode = statecode;
            BpfDuration = bpfDuration;
            Statuscode = statuscode;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            Completedon = completedon;
            this._createdbyValue = _createdbyValue;
            Modifiedon = modifiedon;
            Importsequencenumber = importsequencenumber;
            Businessprocessflowinstanceid = businessprocessflowinstanceid;
            this._processidValue = _processidValue;
            Createdon = createdon;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            Traversedpath = traversedpath;
            BpfName = bpfName;
            Overriddencreatedon = overriddencreatedon;
            this._organizationidValue = _organizationidValue;
            this._modifiedbyValue = _modifiedbyValue;
            Versionnumber = versionnumber;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Organizationid = organizationid;
            Activestageid = activestageid;
            Processid = processid;
            AdoxioApplicationcrsbpfv4SyncErrors = adoxioApplicationcrsbpfv4SyncErrors;
            AdoxioApplicationcrsbpfv4AsyncOperations = adoxioApplicationcrsbpfv4AsyncOperations;
            AdoxioApplicationcrsbpfv4WorkflowLogs = adoxioApplicationcrsbpfv4WorkflowLogs;
            AdoxioApplicationcrsbpfv4MailboxTrackingFolders = adoxioApplicationcrsbpfv4MailboxTrackingFolders;
            AdoxioApplicationcrsbpfv4ProcessSession = adoxioApplicationcrsbpfv4ProcessSession;
            AdoxioApplicationcrsbpfv4BulkDeleteFailures = adoxioApplicationcrsbpfv4BulkDeleteFailures;
            AdoxioApplicationcrsbpfv4PrincipalObjectAttributeAccesses = adoxioApplicationcrsbpfv4PrincipalObjectAttributeAccesses;
            BpfAdoxioApplicationid = bpfAdoxioApplicationid;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "activestagestartedon")]
        public System.DateTimeOffset? Activestagestartedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_bpf_adoxio_applicationid_value")]
        public string _bpfAdoxioApplicationidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_activestageid_value")]
        public string _activestageidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statecode")]
        public int? Statecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "bpf_duration")]
        public int? BpfDuration { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statuscode")]
        public int? Statuscode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timezoneruleversionnumber")]
        public int? Timezoneruleversionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "completedon")]
        public System.DateTimeOffset? Completedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdby_value")]
        public string _createdbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "businessprocessflowinstanceid")]
        public string Businessprocessflowinstanceid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_processid_value")]
        public string _processidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "traversedpath")]
        public string Traversedpath { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "bpf_name")]
        public string BpfName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_organizationid_value")]
        public string _organizationidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

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
        [JsonProperty(PropertyName = "activestageid")]
        public MicrosoftDynamicsCRMprocessstage Activestageid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "processid")]
        public MicrosoftDynamicsCRMworkflow Processid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_applicationcrsbpfv4_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> AdoxioApplicationcrsbpfv4SyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_applicationcrsbpfv4_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> AdoxioApplicationcrsbpfv4AsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_applicationcrsbpfv4_WorkflowLogs")]
        public IList<MicrosoftDynamicsCRMworkflowlog> AdoxioApplicationcrsbpfv4WorkflowLogs { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_applicationcrsbpfv4_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> AdoxioApplicationcrsbpfv4MailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_applicationcrsbpfv4_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> AdoxioApplicationcrsbpfv4ProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_applicationcrsbpfv4_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> AdoxioApplicationcrsbpfv4BulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_applicationcrsbpfv4_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> AdoxioApplicationcrsbpfv4PrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "bpf_adoxio_applicationid")]
        public MicrosoftDynamicsCRMadoxioApplication BpfAdoxioApplicationid { get; set; }

    }
}
