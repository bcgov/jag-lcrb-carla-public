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
    /// Microsoft.Dynamics.CRM.workflowlog
    /// </summary>
    public partial class MicrosoftDynamicsCRMworkflowlog
    {
        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMworkflowlog
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMworkflowlog()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMworkflowlog
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMworkflowlog(string stepname = default(string), string _childworkflowinstanceidValue = default(string), string _owneridValue = default(string), string _modifiedbyValue = default(string), string activityname = default(string), string _owningbusinessunitValue = default(string), string outputsName = default(string), string interactionactivityresult = default(string), string _owninguserValue = default(string), string _createdbyValue = default(string), byte[] outputs = default(byte[]), byte[] inputs = default(byte[]), int? iterationcount = default(int?), System.DateTimeOffset? completedon = default(System.DateTimeOffset?), string workflowlogid = default(string), string message = default(string), int? duration = default(int?), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), string _createdonbehalfbyValue = default(string), int? status = default(int?), string _modifiedonbehalfbyValue = default(string), string _owningteamValue = default(string), string description = default(string), string inputsName = default(string), string errortext = default(string), string _regardingobjectidValue = default(string), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), string stagename = default(string), System.DateTimeOffset? startedon = default(System.DateTimeOffset?), int? repetitioncount = default(int?), string _asyncoperationidValue = default(string), int? errorcode = default(int?), string repetitionid = default(string), MicrosoftDynamicsCRMprocesssession asyncoperationidProcesssession = default(MicrosoftDynamicsCRMprocesssession), MicrosoftDynamicsCRMprocesssession childworkflowinstanceidProcesssession = default(MicrosoftDynamicsCRMprocesssession), MicrosoftDynamicsCRMteam owningteam = default(MicrosoftDynamicsCRMteam), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMasyncoperation childworkflowinstanceidAsyncoperation = default(MicrosoftDynamicsCRMasyncoperation), MicrosoftDynamicsCRMexpiredprocess expiredProcessAsyncoperationid = default(MicrosoftDynamicsCRMexpiredprocess), MicrosoftDynamicsCRMtranslationprocess translationProcessAsyncoperationid = default(MicrosoftDynamicsCRMtranslationprocess), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMbusinessunit owningbusinessunit = default(MicrosoftDynamicsCRMbusinessunit), MicrosoftDynamicsCRMasyncoperation asyncoperationidAsyncoperation = default(MicrosoftDynamicsCRMasyncoperation), MicrosoftDynamicsCRMnewprocess newProcessAsyncoperationid = default(MicrosoftDynamicsCRMnewprocess), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMadoxioApplicationbpfv3 asyncoperationidAdoxioApplicationbpfv3 = default(MicrosoftDynamicsCRMadoxioApplicationbpfv3), MicrosoftDynamicsCRMadoxioApplicationlicenseechangesv10 asyncoperationidAdoxioApplicationlicenseechangesv10 = default(MicrosoftDynamicsCRMadoxioApplicationlicenseechangesv10), MicrosoftDynamicsCRMadoxioApplicationrelocationv1 asyncoperationidAdoxioApplicationrelocationv1 = default(MicrosoftDynamicsCRMadoxioApplicationrelocationv1), MicrosoftDynamicsCRMadoxioApplicationstructurechangev1 asyncoperationidAdoxioApplicationstructurechangev1 = default(MicrosoftDynamicsCRMadoxioApplicationstructurechangev1), MicrosoftDynamicsCRMadoxioApplicationtransferownershipv1 asyncoperationidAdoxioApplicationtransferownershipv1 = default(MicrosoftDynamicsCRMadoxioApplicationtransferownershipv1), MicrosoftDynamicsCRMadoxioApplicationliquorbpfv2 asyncoperationidAdoxioApplicationliquorbpfv2 = default(MicrosoftDynamicsCRMadoxioApplicationliquorbpfv2), MicrosoftDynamicsCRMadoxioEnforcementactionbpf asyncoperationidAdoxioEnforcementactionbpf = default(MicrosoftDynamicsCRMadoxioEnforcementactionbpf), MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4 asyncoperationidAdoxioApplicationcrsbpfv4 = default(MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4), MicrosoftDynamicsCRMadoxioLqrlicencetransferbpf asyncoperationidAdoxioLqrlicencetransferbpf = default(MicrosoftDynamicsCRMadoxioLqrlicencetransferbpf), MicrosoftDynamicsCRMadoxioApplicenseechangebpfv2 asyncoperationidAdoxioApplicenseechangebpfv2 = default(MicrosoftDynamicsCRMadoxioApplicenseechangebpfv2), MicrosoftDynamicsCRMphonetocaseprocess phoneToCaseProcessAsyncoperationid = default(MicrosoftDynamicsCRMphonetocaseprocess), MicrosoftDynamicsCRMleadtoopportunitysalesprocess leadToOpportunitySalesProcessAsyncoperationid = default(MicrosoftDynamicsCRMleadtoopportunitysalesprocess), MicrosoftDynamicsCRMopportunitysalesprocess opportunitySalesProcessAsyncoperationid = default(MicrosoftDynamicsCRMopportunitysalesprocess), IList<MicrosoftDynamicsCRMfileattachment> workflowlogFileAttachments = default(IList<MicrosoftDynamicsCRMfileattachment>), MicrosoftDynamicsCRMadoxioApplicationbpfDormancy asyncoperationidAdoxioApplicationbpfDormancy = default(MicrosoftDynamicsCRMadoxioApplicationbpfDormancy))
        {
            Stepname = stepname;
            this._childworkflowinstanceidValue = _childworkflowinstanceidValue;
            this._owneridValue = _owneridValue;
            this._modifiedbyValue = _modifiedbyValue;
            Activityname = activityname;
            this._owningbusinessunitValue = _owningbusinessunitValue;
            OutputsName = outputsName;
            Interactionactivityresult = interactionactivityresult;
            this._owninguserValue = _owninguserValue;
            this._createdbyValue = _createdbyValue;
            Outputs = outputs;
            Inputs = inputs;
            Iterationcount = iterationcount;
            Completedon = completedon;
            Workflowlogid = workflowlogid;
            Message = message;
            Duration = duration;
            Createdon = createdon;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            Status = status;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            this._owningteamValue = _owningteamValue;
            Description = description;
            InputsName = inputsName;
            Errortext = errortext;
            this._regardingobjectidValue = _regardingobjectidValue;
            Modifiedon = modifiedon;
            Stagename = stagename;
            Startedon = startedon;
            Repetitioncount = repetitioncount;
            this._asyncoperationidValue = _asyncoperationidValue;
            Errorcode = errorcode;
            Repetitionid = repetitionid;
            AsyncoperationidProcesssession = asyncoperationidProcesssession;
            ChildworkflowinstanceidProcesssession = childworkflowinstanceidProcesssession;
            Owningteam = owningteam;
            Createdonbehalfby = createdonbehalfby;
            ChildworkflowinstanceidAsyncoperation = childworkflowinstanceidAsyncoperation;
            ExpiredProcessAsyncoperationid = expiredProcessAsyncoperationid;
            TranslationProcessAsyncoperationid = translationProcessAsyncoperationid;
            Modifiedonbehalfby = modifiedonbehalfby;
            Modifiedby = modifiedby;
            Owningbusinessunit = owningbusinessunit;
            AsyncoperationidAsyncoperation = asyncoperationidAsyncoperation;
            NewProcessAsyncoperationid = newProcessAsyncoperationid;
            Createdby = createdby;
            AsyncoperationidAdoxioApplicationbpfv3 = asyncoperationidAdoxioApplicationbpfv3;
            AsyncoperationidAdoxioApplicationlicenseechangesv10 = asyncoperationidAdoxioApplicationlicenseechangesv10;
            AsyncoperationidAdoxioApplicationrelocationv1 = asyncoperationidAdoxioApplicationrelocationv1;
            AsyncoperationidAdoxioApplicationstructurechangev1 = asyncoperationidAdoxioApplicationstructurechangev1;
            AsyncoperationidAdoxioApplicationtransferownershipv1 = asyncoperationidAdoxioApplicationtransferownershipv1;
            AsyncoperationidAdoxioApplicationliquorbpfv2 = asyncoperationidAdoxioApplicationliquorbpfv2;
            AsyncoperationidAdoxioEnforcementactionbpf = asyncoperationidAdoxioEnforcementactionbpf;
            AsyncoperationidAdoxioApplicationcrsbpfv4 = asyncoperationidAdoxioApplicationcrsbpfv4;
            AsyncoperationidAdoxioLqrlicencetransferbpf = asyncoperationidAdoxioLqrlicencetransferbpf;
            AsyncoperationidAdoxioApplicenseechangebpfv2 = asyncoperationidAdoxioApplicenseechangebpfv2;
            PhoneToCaseProcessAsyncoperationid = phoneToCaseProcessAsyncoperationid;
            LeadToOpportunitySalesProcessAsyncoperationid = leadToOpportunitySalesProcessAsyncoperationid;
            OpportunitySalesProcessAsyncoperationid = opportunitySalesProcessAsyncoperationid;
            WorkflowlogFileAttachments = workflowlogFileAttachments;
            AsyncoperationidAdoxioApplicationbpfDormancy = asyncoperationidAdoxioApplicationbpfDormancy;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "stepname")]
        public string Stepname { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_childworkflowinstanceid_value")]
        public string _childworkflowinstanceidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_ownerid_value")]
        public string _owneridValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "activityname")]
        public string Activityname { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningbusinessunit_value")]
        public string _owningbusinessunitValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "outputs_name")]
        public string OutputsName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "interactionactivityresult")]
        public string Interactionactivityresult { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owninguser_value")]
        public string _owninguserValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdby_value")]
        public string _createdbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonConverter(typeof(Base64UrlJsonConverter))]
        [JsonProperty(PropertyName = "outputs")]
        public byte[] Outputs { get; set; }

        /// <summary>
        /// </summary>
        [JsonConverter(typeof(Base64UrlJsonConverter))]
        [JsonProperty(PropertyName = "inputs")]
        public byte[] Inputs { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "iterationcount")]
        public int? Iterationcount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "completedon")]
        public System.DateTimeOffset? Completedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowlogid")]
        public string Workflowlogid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "duration")]
        public int? Duration { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public int? Status { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningteam_value")]
        public string _owningteamValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "inputs_name")]
        public string InputsName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "errortext")]
        public string Errortext { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_regardingobjectid_value")]
        public string _regardingobjectidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "stagename")]
        public string Stagename { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "startedon")]
        public System.DateTimeOffset? Startedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "repetitioncount")]
        public int? Repetitioncount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_asyncoperationid_value")]
        public string _asyncoperationidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "errorcode")]
        public int? Errorcode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "repetitionid")]
        public string Repetitionid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_processsession")]
        public MicrosoftDynamicsCRMprocesssession AsyncoperationidProcesssession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "childworkflowinstanceid_processsession")]
        public MicrosoftDynamicsCRMprocesssession ChildworkflowinstanceidProcesssession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "owningteam")]
        public MicrosoftDynamicsCRMteam Owningteam { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdonbehalfby")]
        public MicrosoftDynamicsCRMsystemuser Createdonbehalfby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "childworkflowinstanceid_asyncoperation")]
        public MicrosoftDynamicsCRMasyncoperation ChildworkflowinstanceidAsyncoperation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ExpiredProcess_asyncoperationid")]
        public MicrosoftDynamicsCRMexpiredprocess ExpiredProcessAsyncoperationid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "TranslationProcess_asyncoperationid")]
        public MicrosoftDynamicsCRMtranslationprocess TranslationProcessAsyncoperationid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedonbehalfby")]
        public MicrosoftDynamicsCRMsystemuser Modifiedonbehalfby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedby")]
        public MicrosoftDynamicsCRMsystemuser Modifiedby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "owningbusinessunit")]
        public MicrosoftDynamicsCRMbusinessunit Owningbusinessunit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_asyncoperation")]
        public MicrosoftDynamicsCRMasyncoperation AsyncoperationidAsyncoperation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "NewProcess_asyncoperationid")]
        public MicrosoftDynamicsCRMnewprocess NewProcessAsyncoperationid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdby")]
        public MicrosoftDynamicsCRMsystemuser Createdby { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationbpfv3")]
        public MicrosoftDynamicsCRMadoxioApplicationbpfv3 AsyncoperationidAdoxioApplicationbpfv3 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationlicenseechangesv10")]
        public MicrosoftDynamicsCRMadoxioApplicationlicenseechangesv10 AsyncoperationidAdoxioApplicationlicenseechangesv10 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationrelocationv1")]
        public MicrosoftDynamicsCRMadoxioApplicationrelocationv1 AsyncoperationidAdoxioApplicationrelocationv1 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationstructurechangev1")]
        public MicrosoftDynamicsCRMadoxioApplicationstructurechangev1 AsyncoperationidAdoxioApplicationstructurechangev1 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationtransferownershipv1")]
        public MicrosoftDynamicsCRMadoxioApplicationtransferownershipv1 AsyncoperationidAdoxioApplicationtransferownershipv1 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationliquorbpfv2")]
        public MicrosoftDynamicsCRMadoxioApplicationliquorbpfv2 AsyncoperationidAdoxioApplicationliquorbpfv2 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_enforcementactionbpf")]
        public MicrosoftDynamicsCRMadoxioEnforcementactionbpf AsyncoperationidAdoxioEnforcementactionbpf { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationcrsbpfv4")]
        public MicrosoftDynamicsCRMadoxioApplicationcrsbpfv4 AsyncoperationidAdoxioApplicationcrsbpfv4 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_lqrlicencetransferbpf")]
        public MicrosoftDynamicsCRMadoxioLqrlicencetransferbpf AsyncoperationidAdoxioLqrlicencetransferbpf { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicenseechangebpfv2")]
        public MicrosoftDynamicsCRMadoxioApplicenseechangebpfv2 AsyncoperationidAdoxioApplicenseechangebpfv2 { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "PhoneToCaseProcess_asyncoperationid")]
        public MicrosoftDynamicsCRMphonetocaseprocess PhoneToCaseProcessAsyncoperationid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "LeadToOpportunitySalesProcess_asyncoperationid")]
        public MicrosoftDynamicsCRMleadtoopportunitysalesprocess LeadToOpportunitySalesProcessAsyncoperationid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "OpportunitySalesProcess_asyncoperationid")]
        public MicrosoftDynamicsCRMopportunitysalesprocess OpportunitySalesProcessAsyncoperationid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "workflowlog_FileAttachments")]
        public IList<MicrosoftDynamicsCRMfileattachment> WorkflowlogFileAttachments { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asyncoperationid_adoxio_applicationbpf_dormancy")]
        public MicrosoftDynamicsCRMadoxioApplicationbpfDormancy AsyncoperationidAdoxioApplicationbpfDormancy { get; set; }

    }
}
