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
    /// Microsoft.Dynamics.CRM.botcomponent
    /// </summary>
    public partial class MicrosoftDynamicsCRMbotcomponent
    {
        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMbotcomponent
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMbotcomponent()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MicrosoftDynamicsCRMbotcomponent
        /// class.
        /// </summary>
        public MicrosoftDynamicsCRMbotcomponent(string _owningteamValue = default(string), MicrosoftDynamicsCRMBooleanManagedProperty iscustomizable = default(MicrosoftDynamicsCRMBooleanManagedProperty), string _modifiedbyValue = default(string), string _owningbusinessunitValue = default(string), int? timezoneruleversionnumber = default(int?), string _createdbyValue = default(string), System.DateTimeOffset? overwritetime = default(System.DateTimeOffset?), string _createdonbehalfbyValue = default(string), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), int? statuscode = default(int?), string category = default(string), string content = default(string), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), string name = default(string), string _owninguserValue = default(string), string componentidunique = default(string), int? utcconversiontimezonecode = default(int?), string _owneridValue = default(string), string botcomponentid = default(string), string data = default(string), int? componenttype = default(int?), string solutionid = default(string), bool? ismanaged = default(bool?), string _parentbotcomponentidValue = default(string), string description = default(string), int? language = default(int?), string schemaname = default(string), int? componentstate = default(int?), string _modifiedonbehalfbyValue = default(string), string versionnumber = default(string), int? importsequencenumber = default(int?), int? statecode = default(int?), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser owninguser = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMteam owningteam = default(MicrosoftDynamicsCRMteam), MicrosoftDynamicsCRMprincipal ownerid = default(MicrosoftDynamicsCRMprincipal), MicrosoftDynamicsCRMbusinessunit owningbusinessunit = default(MicrosoftDynamicsCRMbusinessunit), IList<MicrosoftDynamicsCRMsyncerror> botcomponentSyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMasyncoperation> botcomponentAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> botcomponentMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> botcomponentProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> botcomponentBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> botcomponentPrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), IList<MicrosoftDynamicsCRMbot> botBotcomponent = default(IList<MicrosoftDynamicsCRMbot>), IList<MicrosoftDynamicsCRMbotcomponent> botcomponentBotcomponent = default(IList<MicrosoftDynamicsCRMbotcomponent>), IList<MicrosoftDynamicsCRMbotcomponent> botcomponentBotcomponentReferenced = default(IList<MicrosoftDynamicsCRMbotcomponent>), MicrosoftDynamicsCRMbotcomponent parentBotComponentId = default(MicrosoftDynamicsCRMbotcomponent), IList<MicrosoftDynamicsCRMbotcomponent> botcomponentParentBotcomponent = default(IList<MicrosoftDynamicsCRMbotcomponent>), IList<MicrosoftDynamicsCRMworkflow> botcomponentWorkflow = default(IList<MicrosoftDynamicsCRMworkflow>))
        {
            this._owningteamValue = _owningteamValue;
            Iscustomizable = iscustomizable;
            this._modifiedbyValue = _modifiedbyValue;
            this._owningbusinessunitValue = _owningbusinessunitValue;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            this._createdbyValue = _createdbyValue;
            Overwritetime = overwritetime;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            Overriddencreatedon = overriddencreatedon;
            Statuscode = statuscode;
            Category = category;
            Content = content;
            Createdon = createdon;
            Modifiedon = modifiedon;
            Name = name;
            this._owninguserValue = _owninguserValue;
            Componentidunique = componentidunique;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            this._owneridValue = _owneridValue;
            Botcomponentid = botcomponentid;
            Data = data;
            Componenttype = componenttype;
            Solutionid = solutionid;
            Ismanaged = ismanaged;
            this._parentbotcomponentidValue = _parentbotcomponentidValue;
            Description = description;
            Language = language;
            Schemaname = schemaname;
            Componentstate = componentstate;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            Versionnumber = versionnumber;
            Importsequencenumber = importsequencenumber;
            Statecode = statecode;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Owninguser = owninguser;
            Owningteam = owningteam;
            Ownerid = ownerid;
            Owningbusinessunit = owningbusinessunit;
            BotcomponentSyncErrors = botcomponentSyncErrors;
            BotcomponentAsyncOperations = botcomponentAsyncOperations;
            BotcomponentMailboxTrackingFolders = botcomponentMailboxTrackingFolders;
            BotcomponentProcessSession = botcomponentProcessSession;
            BotcomponentBulkDeleteFailures = botcomponentBulkDeleteFailures;
            BotcomponentPrincipalObjectAttributeAccesses = botcomponentPrincipalObjectAttributeAccesses;
            BotBotcomponent = botBotcomponent;
            BotcomponentBotcomponent = botcomponentBotcomponent;
            BotcomponentBotcomponentReferenced = botcomponentBotcomponentReferenced;
            ParentBotComponentId = parentBotComponentId;
            BotcomponentParentBotcomponent = botcomponentParentBotcomponent;
            BotcomponentWorkflow = botcomponentWorkflow;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningteam_value")]
        public string _owningteamValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "iscustomizable")]
        public MicrosoftDynamicsCRMBooleanManagedProperty Iscustomizable { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningbusinessunit_value")]
        public string _owningbusinessunitValue { get; set; }

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
        [JsonProperty(PropertyName = "overwritetime")]
        public System.DateTimeOffset? Overwritetime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statuscode")]
        public int? Statuscode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owninguser_value")]
        public string _owninguserValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "componentidunique")]
        public string Componentidunique { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_ownerid_value")]
        public string _owneridValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponentid")]
        public string Botcomponentid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "componenttype")]
        public int? Componenttype { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "solutionid")]
        public string Solutionid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ismanaged")]
        public bool? Ismanaged { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_parentbotcomponentid_value")]
        public string _parentbotcomponentidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "language")]
        public int? Language { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "schemaname")]
        public string Schemaname { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "componentstate")]
        public int? Componentstate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statecode")]
        public int? Statecode { get; set; }

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
        [JsonProperty(PropertyName = "botcomponent_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> BotcomponentSyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> BotcomponentAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> BotcomponentMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> BotcomponentProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> BotcomponentBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> BotcomponentPrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "bot_botcomponent")]
        public IList<MicrosoftDynamicsCRMbot> BotBotcomponent { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_botcomponent")]
        public IList<MicrosoftDynamicsCRMbotcomponent> BotcomponentBotcomponent { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_botcomponent_referenced")]
        public IList<MicrosoftDynamicsCRMbotcomponent> BotcomponentBotcomponentReferenced { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ParentBotComponentId")]
        public MicrosoftDynamicsCRMbotcomponent ParentBotComponentId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_parent_botcomponent")]
        public IList<MicrosoftDynamicsCRMbotcomponent> BotcomponentParentBotcomponent { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "botcomponent_workflow")]
        public IList<MicrosoftDynamicsCRMworkflow> BotcomponentWorkflow { get; set; }

    }
}
