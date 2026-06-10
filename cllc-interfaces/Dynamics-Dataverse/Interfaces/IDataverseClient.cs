using Microsoft.Xrm.Sdk;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Dataverse SDK client — replaces IDynamicsClient (AutoRest).
/// All methods are async and return null when a record is not found (never throw on 404).
/// </summary>
public interface IDataverseClient
{
    // -------------------------------------------------------------------------
    // Account
    // -------------------------------------------------------------------------
    Task<Account?> GetAccountByIdAsync(string id, CancellationToken ct = default);
    Task<Account?> GetAccountByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<Account?> GetAccountByNameAsync(string name, CancellationToken ct = default);
    Task<IList<Account>> GetAccountsAsync(string? filter = null, CancellationToken ct = default);
    Task<Guid> CreateAccountAsync(Account account, CancellationToken ct = default);
    Task UpdateAccountAsync(Account account, CancellationToken ct = default);
    Task DeleteAccountAsync(string id, CancellationToken ct = default);
    Task<Account?> GetAccountByExternalIdAsync(string externalId, CancellationToken ct = default);
    Task SetContactParentAccountAsync(string contactId, string accountId, CancellationToken ct = default);
    Task SetAccountPrimaryContactAsync(string accountId, string contactId, CancellationToken ct = default);
    Task CreateAccountSharePointDocLocAsync(string accountId, string folderName, string displayName, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Contact
    // -------------------------------------------------------------------------
    Task<Contact?> GetContactByIdAsync(string id, CancellationToken ct = default);
    Task<Contact?> GetContactByExternalIdAsync(string externalId, CancellationToken ct = default);
    Task<IList<Contact>> GetContactsByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<Guid> CreateContactAsync(Contact contact, CancellationToken ct = default);
    Task UpdateContactAsync(Contact contact, CancellationToken ct = default);
    Task DeleteContactAsync(string id, CancellationToken ct = default);
    Task UpdateContactBridgeLoginAsync(string contactId, string siteminderGuid, string? accountId, string? siteminderBusinessGuid, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Alias (adoxio_alias)
    // -------------------------------------------------------------------------
    Task<Guid> CreateAliasAsync(adoxio_alias alias, CancellationToken ct = default);
    Task UpdateAliasAsync(adoxio_alias alias, CancellationToken ct = default);
    Task<adoxio_alias?> GetAliasByIdAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_alias>> GetAliasesByContactIdAsync(string contactId, CancellationToken ct = default);
    Task DeleteAliasAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Application (adoxio_application)
    // -------------------------------------------------------------------------
    Task<adoxio_application?> GetApplicationByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_application?> GetApplicationByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_application>> GetApplicationsByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<Guid> CreateApplicationAsync(adoxio_application application, CancellationToken ct = default);
    Task UpdateApplicationAsync(adoxio_application application, CancellationToken ct = default);
    Task DeleteApplicationAsync(string id, CancellationToken ct = default);
    Task<Guid> CreateApplicationExtensionAsync(adoxio_applicationextension extension, CancellationToken ct = default);
    Task UpdateApplicationExtensionAsync(adoxio_applicationextension extension, CancellationToken ct = default);
    Task<Guid> CreateAnnualVolumeAsync(adoxio_annualvolume annualVolume, CancellationToken ct = default);
    Task<IList<adoxio_annualvolume>> GetAnnualVolumesByApplicationIdAsync(string applicationId, CancellationToken ct = default);
    Task DeleteAnnualVolumeAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence (adoxio_licences)
    // -------------------------------------------------------------------------
    Task<adoxio_licences?> GetLicenceByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_licences?> GetLicenceByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<adoxio_licences?> GetLicenceByNumberAsync(string licenceNumber, CancellationToken ct = default);
    Task<IList<adoxio_licences>> GetLicencesByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_licences>> GetActiveLicencesByTypeIdsAsync(IList<string> licenceTypeIds, CancellationToken ct = default);
    Task UpdateLicenceAsync(adoxio_licences licence, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Service Area (adoxio_servicearea)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_servicearea>> GetServiceAreasByLicenceIdAsync(string licenceId, CancellationToken ct = default);
    Task<Guid> CreateServiceAreaAsync(adoxio_servicearea serviceArea, CancellationToken ct = default);
    Task UpdateServiceAreaAsync(adoxio_servicearea serviceArea, CancellationToken ct = default);
    Task DeleteServiceAreaAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Hour of Sale (adoxio_hoursofservice)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_hoursofservice>> GetHoursOfSaleByLicenceIdAsync(string licenceId, CancellationToken ct = default);
    Task<Guid> CreateHourOfSaleAsync(adoxio_hoursofservice hourOfSale, CancellationToken ct = default);
    Task UpdateHourOfSaleAsync(adoxio_hoursofservice hourOfSale, CancellationToken ct = default);
    Task DeleteHourOfSaleAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Off-Site Storage (adoxio_offsitestorage)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_offsitestorage>> GetOffSiteStorageByLicenceIdAsync(string licenceId, CancellationToken ct = default);
    Task<Guid> CreateOffSiteStorageAsync(adoxio_offsitestorage storage, CancellationToken ct = default);
    Task UpdateOffSiteStorageAsync(adoxio_offsitestorage storage, CancellationToken ct = default);
    Task DeleteOffSiteStorageAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Application Terms Conditions Limitation (adoxio_applicationtermsconditionslimitation)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_applicationtermsconditionslimitation>> GetTermsConditionsByLicenceIdAsync(string licenceId, CancellationToken ct = default);
    Task<Guid> CreateTermsConditionsAsync(adoxio_applicationtermsconditionslimitation terms, CancellationToken ct = default);
    Task UpdateTermsConditionsAsync(adoxio_applicationtermsconditionslimitation terms, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Worker (adoxio_worker)
    // -------------------------------------------------------------------------
    Task<adoxio_worker?> GetWorkerByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_worker?> GetWorkerByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_worker>> GetWorkersByContactIdAsync(string contactId, CancellationToken ct = default);
    Task<Guid> CreateWorkerAsync(adoxio_worker worker, CancellationToken ct = default);
    Task UpdateWorkerAsync(adoxio_worker worker, CancellationToken ct = default);
    Task DeleteWorkerAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Personal History Summary (adoxio_personalhistorysummary)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_personalhistorysummary>> GetPersonalHistorySummariesByWorkerIdAsync(string workerId, CancellationToken ct = default);
    Task<Guid> CreatePersonalHistorySummaryAsync(adoxio_personalhistorysummary summary, CancellationToken ct = default);
    Task UpdatePersonalHistorySummaryAsync(adoxio_personalhistorysummary summary, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Previous Address (adoxio_previousaddress)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_previousaddress>> GetPreviousAddressesByWorkerIdAsync(string workerId, CancellationToken ct = default);
    Task<adoxio_previousaddress?> GetPreviousAddressByIdAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_previousaddress>> GetPreviousAddressesByContactIdAsync(string contactId, CancellationToken ct = default);
    Task<Guid> CreatePreviousAddressAsync(adoxio_previousaddress address, CancellationToken ct = default);
    Task UpdatePreviousAddressAsync(adoxio_previousaddress address, CancellationToken ct = default);
    Task DeletePreviousAddressAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Establishment (adoxio_establishment)
    // -------------------------------------------------------------------------
    Task<adoxio_establishment?> GetEstablishmentByIdAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_establishment>> GetEstablishmentsByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_establishment>> GetEstablishmentsByNameAsync(string name, CancellationToken ct = default);
    Task<Guid> CreateEstablishmentAsync(adoxio_establishment establishment, CancellationToken ct = default);
    Task UpdateEstablishmentAsync(adoxio_establishment establishment, CancellationToken ct = default);
    Task DeleteEstablishmentAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence Type (adoxio_licencetype)
    // -------------------------------------------------------------------------
    Task<adoxio_licencetype?> GetLicenceTypeByNameAsync(string name, CancellationToken ct = default);
    Task<IList<adoxio_licencetype>> GetAllLicenceTypesAsync(CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence Sub-Category (adoxio_licencesubcategory)
    // -------------------------------------------------------------------------
    Task<adoxio_licencesubcategory?> GetLicenceSubCategoryByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Application Type — by licence type
    // -------------------------------------------------------------------------
    Task<IList<adoxio_applicationtype>> GetApplicationTypesByLicenceTypeIdAsync(string licenceTypeId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Endorsement (adoxio_endorsement)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_endorsement>> GetEndorsementsByLicenceIdAsync(string licenceId, CancellationToken ct = default);
    Task<IList<adoxio_hoursofservice>> GetHoursOfSaleByEndorsementIdAsync(string endorsementId, CancellationToken ct = default);
    Task<IList<adoxio_servicearea>> GetServiceAreasByEndorsementIdAsync(string endorsementId, CancellationToken ct = default);
    Task<IList<adoxio_hoursofservice>> GetHoursOfSaleByLicenceIdNoEndorsementAsync(string licenceId, CancellationToken ct = default);
    Task<IList<adoxio_servicearea>> GetServiceAreasByLicenceIdNoEndorsementAsync(string licenceId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence — related entity queries
    // -------------------------------------------------------------------------
    Task<IList<adoxio_application>> GetApplicationsForLicenceByApplicantAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_application>> GetActiveApplicationsByAssignedLicenceIdAsync(string licenceId, CancellationToken ct = default);
    Task<IList<adoxio_licences>> GetLicencesByThirdPartyOperatorAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_licences>> GetLicencesByProposedOwnerAsync(string accountId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence — reference clear operations
    // -------------------------------------------------------------------------
    Task ClearLicenceProposedOwnerAsync(string licenceId, CancellationToken ct = default);
    Task ClearLicenceThirdPartyOperatorAsync(string licenceId, CancellationToken ct = default);
    Task ClearAccountProposedOperatorAsync(string accountId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Application — term association
    // -------------------------------------------------------------------------
    Task AssociateTermsConditionsToApplicationAsync(string applicationId, string termId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence SharePoint document location
    // -------------------------------------------------------------------------
    Task CreateLicenceSharePointDocLocAsync(string licenceId, string folderName, string name, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Local Government / Indigenous Nation (adoxio_localgovindigenousnation)
    // -------------------------------------------------------------------------
    Task<adoxio_localgovindigenousnation?> GetLginByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Legal Entity (adoxio_legalentity)
    // -------------------------------------------------------------------------
    Task<adoxio_legalentity?> GetLegalEntityByIdAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_legalentity>> GetLegalEntitiesByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_legalentity>> GetLegalEntitiesByParentEntityIdAsync(string parentLegalEntityId, CancellationToken ct = default);
    Task<Guid> CreateLegalEntityAsync(adoxio_legalentity entity, CancellationToken ct = default);
    Task DeleteLegalEntityAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Tied House Connection (adoxio_tiedhouseconnection)
    // -------------------------------------------------------------------------
    Task<adoxio_tiedhouseconnection?> GetTiedHouseConnectionByIdAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_tiedhouseconnection>> GetTiedHouseConnectionsByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_tiedhouseconnection>> GetLiquorTiedHouseConnectionsByAccountAsync(string accountId, CancellationToken ct = default);
    Task<adoxio_tiedhouseconnection?> GetCannabisTiedHouseConnectionByAccountAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_tiedhouseconnection>> GetTiedHouseConnectionsByApplicationAsync(string applicationId, string accountId, CancellationToken ct = default);
    Task<IList<adoxio_licences>> GetLicencesByTiedHouseConnectionAsync(string tiedHouseId, CancellationToken ct = default);
    Task<Guid> CreateTiedHouseConnectionAsync(adoxio_tiedhouseconnection connection, CancellationToken ct = default);
    Task UpdateTiedHouseConnectionAsync(adoxio_tiedhouseconnection connection, CancellationToken ct = default);
    Task DeleteTiedHouseConnectionAsync(string id, CancellationToken ct = default);
    Task AssociateTiedHouseConnectionToLicenceAsync(string tiedHouseId, string licenceId, CancellationToken ct = default);
    Task DisassociateTiedHouseConnectionFromLicenceAsync(string tiedHouseId, string licenceId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Special Event (adoxio_specialevent)
    // -------------------------------------------------------------------------
    Task<adoxio_specialevent?> GetSpecialEventByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_specialevent?> GetSpecialEventByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<adoxio_specialevent?> GetSpecialEventByLicenceNumberAsync(string licenceNumber, CancellationToken ct = default);
    Task<Guid> CreateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default);
    Task UpdateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // SEP City (adoxio_sepcity)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_sepcity>> GetSepCitiesAsync(CancellationToken ct = default);
    Task<adoxio_sepcity?> GetSepCityByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // SEP Drink Type (adoxio_sepdrinktype)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_sepdrinktype>> GetSepDrinkTypesAsync(CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Event (adoxio_event)
    // -------------------------------------------------------------------------
    Task<adoxio_event?> GetEventByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_event?> GetEventByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_eventschedule>> GetEventSchedulesByEventIdAsync(string eventId, CancellationToken ct = default);
    Task<IList<adoxio_eventlocation>> GetEventLocationsByEventIdAsync(string eventId, CancellationToken ct = default);
    Task<IList<adoxio_event>> GetEventsByAccountAndLicenceAsync(string accountId, string licenceId, int top, CancellationToken ct = default);
    Task<Guid> CreateEventAsync(adoxio_event evt, CancellationToken ct = default);
    Task UpdateEventAsync(adoxio_event evt, CancellationToken ct = default);
    Task DeleteEventAsync(string id, CancellationToken ct = default);
    Task<Guid> CreateEventScheduleAsync(adoxio_eventschedule schedule, CancellationToken ct = default);
    Task DeleteEventScheduleAsync(string id, CancellationToken ct = default);
    Task<Guid> CreateEventLocationAsync(adoxio_eventlocation location, CancellationToken ct = default);
    Task DeleteEventLocationAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_applicationtermsconditionslimitation>> GetTermsConditionsByEventIdAsync(string eventId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Annotation (notes / file attachments)
    // -------------------------------------------------------------------------
    Task<IList<Annotation>> GetAnnotationsByObjectIdAsync(string objectId, CancellationToken ct = default);
    Task<Annotation?> GetAnnotationByIdAsync(string id, CancellationToken ct = default);
    Task<Guid> CreateAnnotationAsync(Annotation annotation, CancellationToken ct = default);
    Task UpdateAnnotationAsync(Annotation annotation, CancellationToken ct = default);
    Task DeleteAnnotationAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // SharePoint document location
    // -------------------------------------------------------------------------
    Task CreateWorkerSharePointDocLocAsync(string workerId, string folderName, CancellationToken ct = default);
    Task<SharePointDocumentLocation?> GetSharePointDocLocByObjectIdAsync(string objectId, CancellationToken ct = default);
    Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByObjectIdAsync(string objectId, CancellationToken ct = default);
    Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByRelativeUrlAsync(string relativeUrl, CancellationToken ct = default);
    Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByRelativeUrlAndNameAsync(string relativeUrl, string name, CancellationToken ct = default);
    Task<Guid> CreateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default);
    Task UpdateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default);
    Task DeleteSharePointDocLocAsync(string id, CancellationToken ct = default);
    Task AssociateFederalReportExportWithDocLocAsync(string exportId, string docLocId, CancellationToken ct = default);
    Task<string?> GetFolderNameAsync(string entityName, string entityId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Federal Report Export (adoxio_federalreportexport)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_federalreportexport>> GetPendingFederalReportExportsAsync(CancellationToken ct = default);
    Task UpdateFederalReportExportAsync(adoxio_federalreportexport export, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Cannabis Monthly Report (adoxio_cannabismonthlyreport)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_cannabismonthlyreport>> GetSubmittedCannabisMonthlyReportsAsync(CancellationToken ct = default);
    Task UpdateCannabisMonthlyReportAsync(adoxio_cannabismonthlyreport report, CancellationToken ct = default);
    Task<adoxio_cannabismonthlyreport?> GetCannabisMonthlyReportByIdAsync(string reportId, CancellationToken ct = default);
    Task<IList<adoxio_cannabismonthlyreport>> GetCannabisMonthlyReportsByLicenceAndLicenceeAsync(string licenceId, string licenceeId, string startDate, CancellationToken ct = default);
    Task<adoxio_cannabismonthlyreport?> GetCannabisMonthlyReportByLicenceYearMonthAsync(string licenceId, string year, string month, CancellationToken ct = default);
    Task<IList<adoxio_cannabismonthlyreport>> GetCannabisMonthlyReportsByLicenceeAsync(string licenceeId, string startDate, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Cannabis Inventory Report (adoxio_cannabisinventoryreport)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_cannabisinventoryreport>> GetInventoryReportsByMonthlyReportIdAsync(string monthlyReportId, CancellationToken ct = default);
    Task UpdateCannabisInventoryReportAsync(adoxio_cannabisinventoryreport report, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Cannabis Product Admin (adoxio_cannabisproductadmin — not in generated types)
    // -------------------------------------------------------------------------
    Task<string?> GetCannabisProductAdminNameByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Application Type (adoxio_applicationtype)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_applicationtype>> GetApplicationTypesAsync(CancellationToken ct = default);
    Task<adoxio_applicationtype?> GetApplicationTypeByNameAsync(string name, CancellationToken ct = default);
    Task<adoxio_applicationtype?> GetApplicationTypeByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Proposed LRS Applications (map data query)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_application>> GetProposedLrsApplicationsAsync(string applicationTypeId, IList<int> excludeStatuses, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // System Form (systemform — standard Dataverse entity)
    // -------------------------------------------------------------------------
    Task<string?> GetSystemFormXmlByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Application picklist metadata (for form rendering)
    // -------------------------------------------------------------------------
    Task<IList<DynamicsPicklistAttributeMetadata>> GetApplicationPicklistsAsync(string entityName = "adoxio_application", CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Police Jurisdiction (adoxio_policejurisdiction)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_policejurisdiction>> GetPoliceJurisdictionsAsync(string? nameContains = null, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Establishment Watch Word (adoxio_establishmentwatchword)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_establishmentwatchword>> GetEstablishmentWatchWordsAsync(CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Local Government / Indigenous Nation — queries
    // -------------------------------------------------------------------------
    Task<IList<adoxio_localgovindigenousnation>> GetIndigenousNationsAsync(CancellationToken ct = default);
    Task<IList<adoxio_localgovindigenousnation>> GetLginsAsync(string? nameContains = null, CancellationToken ct = default);
    Task<Account?> GetAccountByLginLinkIdAsync(string lginId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Terms/Conditions Limitations — single record and preset
    // -------------------------------------------------------------------------
    Task<adoxio_applicationtermsconditionslimitation?> GetTermsConditionsByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_termsconditionslimitationspreset?> GetTermsConditionsPresetByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Account — SEP police representative check
    // -------------------------------------------------------------------------
    Task<bool> IsAccountSepPoliceRepresentativeAsync(string accountId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Policy Document (adoxio_policydocument)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_policydocument>> GetPolicyDocumentsAsync(string? category = null, CancellationToken ct = default);
    Task<adoxio_policydocument?> GetPolicyDocumentBySlugAsync(string slug, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Marketing List / Lead (newsletter subscriptions)
    // -------------------------------------------------------------------------
    Task<List?> GetMarketingListByNameAsync(string listName, CancellationToken ct = default);
    Task<Lead?> GetLeadByEmailAsync(string email, CancellationToken ct = default);
    Task<Guid> CreateLeadAsync(Lead lead, CancellationToken ct = default);
    Task AddLeadToMarketingListAsync(string listId, string leadId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // LE Connection contacts (recursive, for security screening)
    // -------------------------------------------------------------------------
    Task<IList<Contact>> GetLeConnectionContactsAsync(string accountId, IList<string>? memo = null, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Application filter by applicant + type + status codes (for eligibility)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_application>> GetApplicationsByApplicantTypeAndStatusesAsync(string accountId, string applicationTypeId, IList<int> statusCodes, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // LDB Order (adoxio_ldborder)
    // -------------------------------------------------------------------------
    Task<Guid> CreateLdbOrderAsync(adoxio_ldborder order, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // OneStop Message Item (adoxio_onestopmessageitem)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_onestopmessageitem>> GetPendingOneStopMessagesAsync(CancellationToken ct = default);
    Task<IList<adoxio_onestopmessageitem>> GetOneStopMessagesByLicenceIdAsync(string licenceId, CancellationToken ct = default);
    Task UpdateOneStopMessageItemAsync(adoxio_onestopmessageitem item, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence Type by ID
    // -------------------------------------------------------------------------
    Task<adoxio_licencetype?> GetLicenceTypeByIdAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // OrgBook sync
    // -------------------------------------------------------------------------
    Task<IList<adoxio_licences>> GetActiveLicencesMissingOrgBookCredentialAsync(CancellationToken ct = default);
    Task<IList<adoxio_licences>> GetActiveLicencesWithOrgBookCredentialPendingSyncAsync(CancellationToken ct = default);
    Task<IList<Account>> GetAccountsMissingOrgBookLinkAsync(CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licence (delete)
    // -------------------------------------------------------------------------
    Task DeleteLicenceAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Licensee Changelog (adoxio_licenseechangelog — no generated entity type)
    // -------------------------------------------------------------------------
    Task<IList<string>> GetLicenseeChangelogIdsByAccountIdAsync(string accountId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Invoice (invoice)
    // -------------------------------------------------------------------------
    Task DeleteInvoiceAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Generic delete (for entities without a dedicated typed method)
    // -------------------------------------------------------------------------
    Task DeleteByLogicalNameAsync(string logicalName, string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Workflow execution
    // -------------------------------------------------------------------------
    Task ExecuteWorkflowAsync(string workflowId, string entityId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Pagination — generic query with paging cookie
    // -------------------------------------------------------------------------
    Task<(IList<T> Results, string? NextPagingCookie)> RetrievePagedAsync<T>(
        Microsoft.Xrm.Sdk.Query.QueryExpression query,
        int pageSize = 5000,
        string? pagingCookie = null,
        CancellationToken ct = default) where T : Entity;

    // -------------------------------------------------------------------------
    // SPICE sync support
    // -------------------------------------------------------------------------
    Task<Contact?> GetContactBySpdJobIdAsync(int spdJobId, CancellationToken ct = default);
    Task<adoxio_personalhistorysummary?> GetPersonalHistorySummaryByWorkerJobNumberAsync(string jobNumber, CancellationToken ct = default);
    Task<adoxio_application?> GetApplicationByJobNumberAsync(string jobNumber, CancellationToken ct = default);
    Task<IList<adoxio_leconnection>> GetActiveLeConnectionsByParentAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<IList<adoxio_worker>> GetWorkersToSendAsync(CancellationToken ct = default);
    Task<IList<adoxio_applicationtype>> GetApplicationTypesWithLeSectionAsync(CancellationToken ct = default);
    Task<IList<adoxio_application>> GetApplicationsToSendAsync(CancellationToken ct = default);
}
