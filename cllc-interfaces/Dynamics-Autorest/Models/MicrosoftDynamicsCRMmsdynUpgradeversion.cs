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
    /// Microsoft.Dynamics.CRM.msdyn_upgradeversion
    /// </summary>
    public partial class MicrosoftDynamicsCRMmsdynUpgradeversion
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMmsdynUpgradeversion class.
        /// </summary>
        public MicrosoftDynamicsCRMmsdynUpgradeversion()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMmsdynUpgradeversion class.
        /// </summary>
        public MicrosoftDynamicsCRMmsdynUpgradeversion(string _organizationidValue = default(string), int? utcconversiontimezonecode = default(int?), int? importsequencenumber = default(int?), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), string msdynTargetversion = default(string), System.DateTimeOffset? msdynFinished = default(System.DateTimeOffset?), int? timezoneruleversionnumber = default(int?), int? statecode = default(int?), string _modifiedonbehalfbyValue = default(string), string _createdbyValue = default(string), int? msdynStatus = default(int?), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), string msdynUpgradeversionid = default(string), string msdynSummary = default(string), string _createdonbehalfbyValue = default(string), string _msdynUpgraderunValue = default(string), string versionnumber = default(string), int? statuscode = default(int?), string msdynStartingversion = default(string), string _modifiedbyValue = default(string), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMorganization organizationid = default(MicrosoftDynamicsCRMorganization), IList<MicrosoftDynamicsCRMsyncerror> msdynUpgradeversionSyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMasyncoperation> msdynUpgradeversionAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> msdynUpgradeversionMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> msdynUpgradeversionProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> msdynUpgradeversionBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> msdynUpgradeversionPrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), MicrosoftDynamicsCRMmsdynUpgraderun msdynUpgradeRun = default(MicrosoftDynamicsCRMmsdynUpgraderun), IList<MicrosoftDynamicsCRMmsdynUpgradestep> msdynMsdynUpgradeversionMsdynUpgradestepUpgradeVersion = default(IList<MicrosoftDynamicsCRMmsdynUpgradestep>))
        {
            this._organizationidValue = _organizationidValue;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            Importsequencenumber = importsequencenumber;
            Overriddencreatedon = overriddencreatedon;
            MsdynTargetversion = msdynTargetversion;
            MsdynFinished = msdynFinished;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            Statecode = statecode;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            this._createdbyValue = _createdbyValue;
            MsdynStatus = msdynStatus;
            Modifiedon = modifiedon;
            MsdynUpgradeversionid = msdynUpgradeversionid;
            MsdynSummary = msdynSummary;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            this._msdynUpgraderunValue = _msdynUpgraderunValue;
            Versionnumber = versionnumber;
            Statuscode = statuscode;
            MsdynStartingversion = msdynStartingversion;
            this._modifiedbyValue = _modifiedbyValue;
            Createdon = createdon;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Organizationid = organizationid;
            MsdynUpgradeversionSyncErrors = msdynUpgradeversionSyncErrors;
            MsdynUpgradeversionAsyncOperations = msdynUpgradeversionAsyncOperations;
            MsdynUpgradeversionMailboxTrackingFolders = msdynUpgradeversionMailboxTrackingFolders;
            MsdynUpgradeversionProcessSession = msdynUpgradeversionProcessSession;
            MsdynUpgradeversionBulkDeleteFailures = msdynUpgradeversionBulkDeleteFailures;
            MsdynUpgradeversionPrincipalObjectAttributeAccesses = msdynUpgradeversionPrincipalObjectAttributeAccesses;
            MsdynUpgradeRun = msdynUpgradeRun;
            MsdynMsdynUpgradeversionMsdynUpgradestepUpgradeVersion = msdynMsdynUpgradeversionMsdynUpgradestepUpgradeVersion;
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
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_targetversion")]
        public string MsdynTargetversion { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_finished")]
        public System.DateTimeOffset? MsdynFinished { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timezoneruleversionnumber")]
        public int? Timezoneruleversionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statecode")]
        public int? Statecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdby_value")]
        public string _createdbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_status")]
        public int? MsdynStatus { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_upgradeversionid")]
        public string MsdynUpgradeversionid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_summary")]
        public string MsdynSummary { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_msdyn_upgraderun_value")]
        public string _msdynUpgraderunValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statuscode")]
        public int? Statuscode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_startingversion")]
        public string MsdynStartingversion { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

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
        [JsonProperty(PropertyName = "msdyn_upgradeversion_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> MsdynUpgradeversionSyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_upgradeversion_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> MsdynUpgradeversionAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_upgradeversion_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> MsdynUpgradeversionMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_upgradeversion_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> MsdynUpgradeversionProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_upgradeversion_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> MsdynUpgradeversionBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_upgradeversion_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> MsdynUpgradeversionPrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_UpgradeRun")]
        public MicrosoftDynamicsCRMmsdynUpgraderun MsdynUpgradeRun { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "msdyn_msdyn_upgradeversion_msdyn_upgradestep_UpgradeVersion")]
        public IList<MicrosoftDynamicsCRMmsdynUpgradestep> MsdynMsdynUpgradeversionMsdynUpgradestepUpgradeVersion { get; set; }

    }
}
