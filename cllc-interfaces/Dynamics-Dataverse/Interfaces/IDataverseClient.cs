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

    // -------------------------------------------------------------------------
    // Contact
    // -------------------------------------------------------------------------
    Task<Contact?> GetContactByIdAsync(string id, CancellationToken ct = default);
    Task<Guid> CreateContactAsync(Contact contact, CancellationToken ct = default);
    Task UpdateContactAsync(Contact contact, CancellationToken ct = default);

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

    // -------------------------------------------------------------------------
    // Licence (adoxio_licences)
    // -------------------------------------------------------------------------
    Task<adoxio_licences?> GetLicenceByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_licences?> GetLicenceByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<adoxio_licences?> GetLicenceByNumberAsync(string licenceNumber, CancellationToken ct = default);
    Task<IList<adoxio_licences>> GetLicencesByAccountIdAsync(string accountId, CancellationToken ct = default);
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
    Task<Guid> CreateWorkerAsync(adoxio_worker worker, CancellationToken ct = default);
    Task UpdateWorkerAsync(adoxio_worker worker, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Establishment (adoxio_establishment)
    // -------------------------------------------------------------------------
    Task<adoxio_establishment?> GetEstablishmentByIdAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_establishment>> GetEstablishmentsByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task UpdateEstablishmentAsync(adoxio_establishment establishment, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Legal Entity (adoxio_legalentity)
    // -------------------------------------------------------------------------
    Task<adoxio_legalentity?> GetLegalEntityByIdAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_legalentity>> GetLegalEntitiesByAccountIdAsync(string accountId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Tied House Connection (adoxio_tiedhouseconnection)
    // -------------------------------------------------------------------------
    Task<IList<adoxio_tiedhouseconnection>> GetTiedHouseConnectionsByAccountIdAsync(string accountId, CancellationToken ct = default);
    Task<Guid> CreateTiedHouseConnectionAsync(adoxio_tiedhouseconnection connection, CancellationToken ct = default);
    Task DeleteTiedHouseConnectionAsync(string id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Special Event (adoxio_specialevent)
    // -------------------------------------------------------------------------
    Task<adoxio_specialevent?> GetSpecialEventByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_specialevent?> GetSpecialEventByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<adoxio_specialevent?> GetSpecialEventByLicenceNumberAsync(string licenceNumber, CancellationToken ct = default);
    Task<Guid> CreateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default);
    Task UpdateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Event (adoxio_event)
    // -------------------------------------------------------------------------
    Task<adoxio_event?> GetEventByIdAsync(string id, CancellationToken ct = default);
    Task<adoxio_event?> GetEventByIdWithChildrenAsync(string id, CancellationToken ct = default);
    Task<IList<adoxio_eventschedule>> GetEventSchedulesByEventIdAsync(string eventId, CancellationToken ct = default);
    Task<IList<adoxio_eventlocation>> GetEventLocationsByEventIdAsync(string eventId, CancellationToken ct = default);

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
    Task<SharePointDocumentLocation?> GetSharePointDocLocByObjectIdAsync(string objectId, CancellationToken ct = default);
    Task<Guid> CreateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default);
    Task UpdateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Pagination — generic query with paging cookie
    // -------------------------------------------------------------------------
    Task<(IList<T> Results, string? NextPagingCookie)> RetrievePagedAsync<T>(
        Microsoft.Xrm.Sdk.Query.QueryExpression query,
        int pageSize = 5000,
        string? pagingCookie = null,
        CancellationToken ct = default) where T : Entity;
}
