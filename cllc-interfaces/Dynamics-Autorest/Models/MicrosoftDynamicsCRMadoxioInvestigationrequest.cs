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
    /// Microsoft.Dynamics.CRM.adoxio_investigationrequest
    /// </summary>
    public partial class MicrosoftDynamicsCRMadoxioInvestigationrequest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioInvestigationrequest class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioInvestigationrequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioInvestigationrequest class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioInvestigationrequest(string adoxioAdditionalinformation = default(string), bool? adoxioIsproduct = default(bool?), System.DateTimeOffset? overriddencreatedon = default(System.DateTimeOffset?), string adoxioFinancialintegrityconcerns = default(string), bool? adoxioIsidentityissues = default(bool?), bool? adoxioIsoverservice = default(bool?), string adoxioName = default(string), int? adoxioDgmdecision = default(int?), string adoxioDgmdecisionreason = default(string), string adoxioReferraldatelongformdate = default(string), string adoxioInvestigationrequestid = default(string), string adoxioInvestigationsummary = default(string), string _adoxioLicenceidValue = default(string), System.DateTimeOffset? adoxioSubmitteddate = default(System.DateTimeOffset?), string adoxioLicenseereputation = default(string), string adoxioRequestreasoning = default(string), string _adoxioOfficeidValue = default(string), bool? adoxioIswitnesses = default(bool?), bool? adoxioIsopensource = default(bool?), string _owningteamValue = default(string), bool? adoxioIsother = default(bool?), int? utcconversiontimezonecode = default(int?), int? statuscode = default(int?), bool? adoxioIsnonlicensedentitycomplaints = default(bool?), int? timezoneruleversionnumber = default(int?), bool? adoxioIsvideo = default(bool?), string adoxioOtherinvolving = default(string), System.DateTimeOffset? adoxioReferraldate = default(System.DateTimeOffset?), int? adoxioInvestigationmanagersdecision = default(int?), string _owninguserValue = default(string), bool? adoxioIssubmitted = default(bool?), string adoxioCompliancehistoryconcerns = default(string), bool? adoxioIsestablishmentrecords = default(bool?), int? adoxioInvestigationoutcome = default(int?), string adoxioEstablishmentstreet = default(string), string _owningbusinessunitValue = default(string), string adoxioPhone = default(string), string adoxioEstablishmentcity = default(string), string _modifiedbyValue = default(string), bool? adoxioIsdeathharm = default(bool?), string adoxioEmail = default(string), string _createdonbehalfbyValue = default(string), string adoxioCommunityconcerns = default(string), string _adoxioComplaintidValue = default(string), string versionnumber = default(string), bool? adoxioIscrossregional = default(bool?), string _adoxioAreaidValue = default(string), bool? adoxioIsmanufacturing = default(bool?), string adoxioContactnumber = default(string), string _adoxioApplicationidValue = default(string), string _owneridValue = default(string), string _createdbyValue = default(string), int? adoxioType = default(int?), bool? adoxioIsaudits = default(bool?), bool? adoxioIsobservational = default(bool?), string adoxioLeadershipdiscussion = default(string), int? importsequencenumber = default(int?), string _modifiedonbehalfbyValue = default(string), System.DateTimeOffset? createdon = default(System.DateTimeOffset?), string _adoxioEstablishmentidValue = default(string), string adoxioEstablishmentnametext = default(string), System.DateTimeOffset? modifiedon = default(System.DateTimeOffset?), string _adoxioAccountidValue = default(string), string adoxioEstablishmentpostalcode = default(string), string _adoxioRegionidValue = default(string), int? statecode = default(int?), MicrosoftDynamicsCRMsystemuser createdby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser createdonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser modifiedonbehalfby = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMsystemuser owninguser = default(MicrosoftDynamicsCRMsystemuser), MicrosoftDynamicsCRMteam owningteam = default(MicrosoftDynamicsCRMteam), MicrosoftDynamicsCRMprincipal ownerid = default(MicrosoftDynamicsCRMprincipal), MicrosoftDynamicsCRMbusinessunit owningbusinessunit = default(MicrosoftDynamicsCRMbusinessunit), IList<MicrosoftDynamicsCRMsyncerror> adoxioInvestigationrequestSyncErrors = default(IList<MicrosoftDynamicsCRMsyncerror>), IList<MicrosoftDynamicsCRMduplicaterecord> adoxioInvestigationrequestDuplicateMatchingRecord = default(IList<MicrosoftDynamicsCRMduplicaterecord>), IList<MicrosoftDynamicsCRMduplicaterecord> adoxioInvestigationrequestDuplicateBaseRecord = default(IList<MicrosoftDynamicsCRMduplicaterecord>), IList<MicrosoftDynamicsCRMasyncoperation> adoxioInvestigationrequestAsyncOperations = default(IList<MicrosoftDynamicsCRMasyncoperation>), IList<MicrosoftDynamicsCRMmailboxtrackingfolder> adoxioInvestigationrequestMailboxTrackingFolders = default(IList<MicrosoftDynamicsCRMmailboxtrackingfolder>), IList<MicrosoftDynamicsCRMprocesssession> adoxioInvestigationrequestProcessSession = default(IList<MicrosoftDynamicsCRMprocesssession>), IList<MicrosoftDynamicsCRMbulkdeletefailure> adoxioInvestigationrequestBulkDeleteFailures = default(IList<MicrosoftDynamicsCRMbulkdeletefailure>), IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> adoxioInvestigationrequestPrincipalObjectAttributeAccesses = default(IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess>), IList<MicrosoftDynamicsCRMadoxioInvestigation> adoxioInvestigationrequestInvestigations = default(IList<MicrosoftDynamicsCRMadoxioInvestigation>), MicrosoftDynamicsCRMadoxioApplication adoxioApplicationId = default(MicrosoftDynamicsCRMadoxioApplication), MicrosoftDynamicsCRMaccount adoxioAccountId = default(MicrosoftDynamicsCRMaccount), MicrosoftDynamicsCRMadoxioLicences adoxioLicenceId = default(MicrosoftDynamicsCRMadoxioLicences), MicrosoftDynamicsCRMadoxioEstablishment adoxioEstablishmentId = default(MicrosoftDynamicsCRMadoxioEstablishment), MicrosoftDynamicsCRMadoxioTerritory adoxioOfficeId = default(MicrosoftDynamicsCRMadoxioTerritory), MicrosoftDynamicsCRMadoxioComplaint adoxioComplaintId = default(MicrosoftDynamicsCRMadoxioComplaint), IList<MicrosoftDynamicsCRMsharepointdocumentlocation> adoxioInvestigationrequestSharePointDocumentLocations = default(IList<MicrosoftDynamicsCRMsharepointdocumentlocation>), MicrosoftDynamicsCRMadoxioArea adoxioAreaId = default(MicrosoftDynamicsCRMadoxioArea), MicrosoftDynamicsCRMadoxioRegion adoxioRegionId = default(MicrosoftDynamicsCRMadoxioRegion))
        {
            AdoxioAdditionalinformation = adoxioAdditionalinformation;
            AdoxioIsproduct = adoxioIsproduct;
            Overriddencreatedon = overriddencreatedon;
            AdoxioFinancialintegrityconcerns = adoxioFinancialintegrityconcerns;
            AdoxioIsidentityissues = adoxioIsidentityissues;
            AdoxioIsoverservice = adoxioIsoverservice;
            AdoxioName = adoxioName;
            AdoxioDgmdecision = adoxioDgmdecision;
            AdoxioDgmdecisionreason = adoxioDgmdecisionreason;
            AdoxioReferraldatelongformdate = adoxioReferraldatelongformdate;
            AdoxioInvestigationrequestid = adoxioInvestigationrequestid;
            AdoxioInvestigationsummary = adoxioInvestigationsummary;
            this._adoxioLicenceidValue = _adoxioLicenceidValue;
            AdoxioSubmitteddate = adoxioSubmitteddate;
            AdoxioLicenseereputation = adoxioLicenseereputation;
            AdoxioRequestreasoning = adoxioRequestreasoning;
            this._adoxioOfficeidValue = _adoxioOfficeidValue;
            AdoxioIswitnesses = adoxioIswitnesses;
            AdoxioIsopensource = adoxioIsopensource;
            this._owningteamValue = _owningteamValue;
            AdoxioIsother = adoxioIsother;
            Utcconversiontimezonecode = utcconversiontimezonecode;
            Statuscode = statuscode;
            AdoxioIsnonlicensedentitycomplaints = adoxioIsnonlicensedentitycomplaints;
            Timezoneruleversionnumber = timezoneruleversionnumber;
            AdoxioIsvideo = adoxioIsvideo;
            AdoxioOtherinvolving = adoxioOtherinvolving;
            AdoxioReferraldate = adoxioReferraldate;
            AdoxioInvestigationmanagersdecision = adoxioInvestigationmanagersdecision;
            this._owninguserValue = _owninguserValue;
            AdoxioIssubmitted = adoxioIssubmitted;
            AdoxioCompliancehistoryconcerns = adoxioCompliancehistoryconcerns;
            AdoxioIsestablishmentrecords = adoxioIsestablishmentrecords;
            AdoxioInvestigationoutcome = adoxioInvestigationoutcome;
            AdoxioEstablishmentstreet = adoxioEstablishmentstreet;
            this._owningbusinessunitValue = _owningbusinessunitValue;
            AdoxioPhone = adoxioPhone;
            AdoxioEstablishmentcity = adoxioEstablishmentcity;
            this._modifiedbyValue = _modifiedbyValue;
            AdoxioIsdeathharm = adoxioIsdeathharm;
            AdoxioEmail = adoxioEmail;
            this._createdonbehalfbyValue = _createdonbehalfbyValue;
            AdoxioCommunityconcerns = adoxioCommunityconcerns;
            this._adoxioComplaintidValue = _adoxioComplaintidValue;
            Versionnumber = versionnumber;
            AdoxioIscrossregional = adoxioIscrossregional;
            this._adoxioAreaidValue = _adoxioAreaidValue;
            AdoxioIsmanufacturing = adoxioIsmanufacturing;
            AdoxioContactnumber = adoxioContactnumber;
            this._adoxioApplicationidValue = _adoxioApplicationidValue;
            this._owneridValue = _owneridValue;
            this._createdbyValue = _createdbyValue;
            AdoxioType = adoxioType;
            AdoxioIsaudits = adoxioIsaudits;
            AdoxioIsobservational = adoxioIsobservational;
            AdoxioLeadershipdiscussion = adoxioLeadershipdiscussion;
            Importsequencenumber = importsequencenumber;
            this._modifiedonbehalfbyValue = _modifiedonbehalfbyValue;
            Createdon = createdon;
            this._adoxioEstablishmentidValue = _adoxioEstablishmentidValue;
            AdoxioEstablishmentnametext = adoxioEstablishmentnametext;
            Modifiedon = modifiedon;
            this._adoxioAccountidValue = _adoxioAccountidValue;
            AdoxioEstablishmentpostalcode = adoxioEstablishmentpostalcode;
            this._adoxioRegionidValue = _adoxioRegionidValue;
            Statecode = statecode;
            Createdby = createdby;
            Createdonbehalfby = createdonbehalfby;
            Modifiedby = modifiedby;
            Modifiedonbehalfby = modifiedonbehalfby;
            Owninguser = owninguser;
            Owningteam = owningteam;
            Ownerid = ownerid;
            Owningbusinessunit = owningbusinessunit;
            AdoxioInvestigationrequestSyncErrors = adoxioInvestigationrequestSyncErrors;
            AdoxioInvestigationrequestDuplicateMatchingRecord = adoxioInvestigationrequestDuplicateMatchingRecord;
            AdoxioInvestigationrequestDuplicateBaseRecord = adoxioInvestigationrequestDuplicateBaseRecord;
            AdoxioInvestigationrequestAsyncOperations = adoxioInvestigationrequestAsyncOperations;
            AdoxioInvestigationrequestMailboxTrackingFolders = adoxioInvestigationrequestMailboxTrackingFolders;
            AdoxioInvestigationrequestProcessSession = adoxioInvestigationrequestProcessSession;
            AdoxioInvestigationrequestBulkDeleteFailures = adoxioInvestigationrequestBulkDeleteFailures;
            AdoxioInvestigationrequestPrincipalObjectAttributeAccesses = adoxioInvestigationrequestPrincipalObjectAttributeAccesses;
            AdoxioInvestigationrequestInvestigations = adoxioInvestigationrequestInvestigations;
            AdoxioApplicationId = adoxioApplicationId;
            AdoxioAccountId = adoxioAccountId;
            AdoxioLicenceId = adoxioLicenceId;
            AdoxioEstablishmentId = adoxioEstablishmentId;
            AdoxioOfficeId = adoxioOfficeId;
            AdoxioComplaintId = adoxioComplaintId;
            AdoxioInvestigationrequestSharePointDocumentLocations = adoxioInvestigationrequestSharePointDocumentLocations;
            AdoxioAreaId = adoxioAreaId;
            AdoxioRegionId = adoxioRegionId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_additionalinformation")]
        public string AdoxioAdditionalinformation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isproduct")]
        public bool? AdoxioIsproduct { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "overriddencreatedon")]
        public System.DateTimeOffset? Overriddencreatedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_financialintegrityconcerns")]
        public string AdoxioFinancialintegrityconcerns { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isidentityissues")]
        public bool? AdoxioIsidentityissues { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isoverservice")]
        public bool? AdoxioIsoverservice { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_name")]
        public string AdoxioName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_dgmdecision")]
        public int? AdoxioDgmdecision { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_dgmdecisionreason")]
        public string AdoxioDgmdecisionreason { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_referraldatelongformdate")]
        public string AdoxioReferraldatelongformdate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequestid")]
        public string AdoxioInvestigationrequestid { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationsummary")]
        public string AdoxioInvestigationsummary { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_licenceid_value")]
        public string _adoxioLicenceidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_submitteddate")]
        public System.DateTimeOffset? AdoxioSubmitteddate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_licenseereputation")]
        public string AdoxioLicenseereputation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_requestreasoning")]
        public string AdoxioRequestreasoning { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_officeid_value")]
        public string _adoxioOfficeidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_iswitnesses")]
        public bool? AdoxioIswitnesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isopensource")]
        public bool? AdoxioIsopensource { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningteam_value")]
        public string _owningteamValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isother")]
        public bool? AdoxioIsother { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "utcconversiontimezonecode")]
        public int? Utcconversiontimezonecode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "statuscode")]
        public int? Statuscode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isnonlicensedentitycomplaints")]
        public bool? AdoxioIsnonlicensedentitycomplaints { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "timezoneruleversionnumber")]
        public int? Timezoneruleversionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isvideo")]
        public bool? AdoxioIsvideo { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_otherinvolving")]
        public string AdoxioOtherinvolving { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_referraldate")]
        public System.DateTimeOffset? AdoxioReferraldate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationmanagersdecision")]
        public int? AdoxioInvestigationmanagersdecision { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owninguser_value")]
        public string _owninguserValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_issubmitted")]
        public bool? AdoxioIssubmitted { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_compliancehistoryconcerns")]
        public string AdoxioCompliancehistoryconcerns { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isestablishmentrecords")]
        public bool? AdoxioIsestablishmentrecords { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationoutcome")]
        public int? AdoxioInvestigationoutcome { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_establishmentstreet")]
        public string AdoxioEstablishmentstreet { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_owningbusinessunit_value")]
        public string _owningbusinessunitValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_phone")]
        public string AdoxioPhone { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_establishmentcity")]
        public string AdoxioEstablishmentcity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedby_value")]
        public string _modifiedbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isdeathharm")]
        public bool? AdoxioIsdeathharm { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_email")]
        public string AdoxioEmail { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdonbehalfby_value")]
        public string _createdonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_communityconcerns")]
        public string AdoxioCommunityconcerns { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_complaintid_value")]
        public string _adoxioComplaintidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "versionnumber")]
        public string Versionnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_iscrossregional")]
        public bool? AdoxioIscrossregional { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_areaid_value")]
        public string _adoxioAreaidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_ismanufacturing")]
        public bool? AdoxioIsmanufacturing { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_contactnumber")]
        public string AdoxioContactnumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_applicationid_value")]
        public string _adoxioApplicationidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_ownerid_value")]
        public string _owneridValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_createdby_value")]
        public string _createdbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_type")]
        public int? AdoxioType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isaudits")]
        public bool? AdoxioIsaudits { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_isobservational")]
        public bool? AdoxioIsobservational { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_leadershipdiscussion")]
        public string AdoxioLeadershipdiscussion { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "importsequencenumber")]
        public int? Importsequencenumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_modifiedonbehalfby_value")]
        public string _modifiedonbehalfbyValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdon")]
        public System.DateTimeOffset? Createdon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_establishmentid_value")]
        public string _adoxioEstablishmentidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_establishmentnametext")]
        public string AdoxioEstablishmentnametext { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedon")]
        public System.DateTimeOffset? Modifiedon { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_accountid_value")]
        public string _adoxioAccountidValue { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_establishmentpostalcode")]
        public string AdoxioEstablishmentpostalcode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "_adoxio_regionid_value")]
        public string _adoxioRegionidValue { get; set; }

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
        [JsonProperty(PropertyName = "adoxio_investigationrequest_SyncErrors")]
        public IList<MicrosoftDynamicsCRMsyncerror> AdoxioInvestigationrequestSyncErrors { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_DuplicateMatchingRecord")]
        public IList<MicrosoftDynamicsCRMduplicaterecord> AdoxioInvestigationrequestDuplicateMatchingRecord { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_DuplicateBaseRecord")]
        public IList<MicrosoftDynamicsCRMduplicaterecord> AdoxioInvestigationrequestDuplicateBaseRecord { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_AsyncOperations")]
        public IList<MicrosoftDynamicsCRMasyncoperation> AdoxioInvestigationrequestAsyncOperations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_MailboxTrackingFolders")]
        public IList<MicrosoftDynamicsCRMmailboxtrackingfolder> AdoxioInvestigationrequestMailboxTrackingFolders { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_ProcessSession")]
        public IList<MicrosoftDynamicsCRMprocesssession> AdoxioInvestigationrequestProcessSession { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_BulkDeleteFailures")]
        public IList<MicrosoftDynamicsCRMbulkdeletefailure> AdoxioInvestigationrequestBulkDeleteFailures { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_PrincipalObjectAttributeAccesses")]
        public IList<MicrosoftDynamicsCRMprincipalobjectattributeaccess> AdoxioInvestigationrequestPrincipalObjectAttributeAccesses { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_investigations")]
        public IList<MicrosoftDynamicsCRMadoxioInvestigation> AdoxioInvestigationrequestInvestigations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_ApplicationId")]
        public MicrosoftDynamicsCRMadoxioApplication AdoxioApplicationId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_AccountId")]
        public MicrosoftDynamicsCRMaccount AdoxioAccountId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_LicenceId")]
        public MicrosoftDynamicsCRMadoxioLicences AdoxioLicenceId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_EstablishmentId")]
        public MicrosoftDynamicsCRMadoxioEstablishment AdoxioEstablishmentId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_OfficeId")]
        public MicrosoftDynamicsCRMadoxioTerritory AdoxioOfficeId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_ComplaintId")]
        public MicrosoftDynamicsCRMadoxioComplaint AdoxioComplaintId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_investigationrequest_SharePointDocumentLocations")]
        public IList<MicrosoftDynamicsCRMsharepointdocumentlocation> AdoxioInvestigationrequestSharePointDocumentLocations { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_AreaId")]
        public MicrosoftDynamicsCRMadoxioArea AdoxioAreaId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "adoxio_RegionId")]
        public MicrosoftDynamicsCRMadoxioRegion AdoxioRegionId { get; set; }

    }
}
