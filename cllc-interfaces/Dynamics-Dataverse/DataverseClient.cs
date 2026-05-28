using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Gov.Lclb.Cllb.Interfaces;

public class DataverseClient : IDataverseClient, IHealthCheck
{
    protected readonly ServiceClient _serviceClient;

    public DataverseClient(IConfiguration configuration)
    {
        var url = configuration["DYNAMICS_ODATA_URI"]
            ?? throw new InvalidOperationException("DYNAMICS_ODATA_URI is not configured.");

        // Strip /api/data/vX.X/ suffix if present — ServiceClient needs the base org URL
        var orgUrl = ExtractOrgUrl(url);

        var tenantId = configuration["DYNAMICS_AAD_TENANT_ID"]
            ?? throw new InvalidOperationException("DYNAMICS_AAD_TENANT_ID is not configured.");
        var clientId = configuration["DYNAMICS_APP_REG_CLIENT_ID"]
            ?? throw new InvalidOperationException("DYNAMICS_APP_REG_CLIENT_ID is not configured.");
        var clientSecret = configuration["DYNAMICS_APP_REG_CLIENT_KEY"]
            ?? throw new InvalidOperationException("DYNAMICS_APP_REG_CLIENT_KEY is not configured.");

        var connectionString =
            $"AuthType=ClientSecret;" +
            $"Url={orgUrl};" +
            $"ClientId={clientId};" +
            $"ClientSecret={clientSecret};" +
            $"TenantId={tenantId};";

        _serviceClient = new ServiceClient(connectionString);
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _serviceClient.IsReady
                ? HealthCheckResult.Healthy("Dataverse connection is ready.")
                : HealthCheckResult.Unhealthy("Dataverse ServiceClient is not ready."));
    }

    private static string ExtractOrgUrl(string odataUri)
    {
        var uri = new Uri(odataUri);
        return $"{uri.Scheme}://{uri.Host}";
    }

    // -------------------------------------------------------------------------
    // Account
    // -------------------------------------------------------------------------
    public Task<Account?> GetAccountByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Account?> GetAccountByIdWithChildrenAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Account?> GetAccountByNameAsync(string name, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IList<Account>> GetAccountsAsync(string? filter = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateAccountAsync(Account account, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAccountAsync(Account account, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Contact
    // -------------------------------------------------------------------------
    public Task<Contact?> GetContactByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateContactAsync(Contact contact, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateContactAsync(Contact contact, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Application
    // -------------------------------------------------------------------------
    public Task<adoxio_application?> GetApplicationByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<adoxio_application?> GetApplicationByIdWithChildrenAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IList<adoxio_application>> GetApplicationsByAccountIdAsync(string accountId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateApplicationAsync(adoxio_application application, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateApplicationAsync(adoxio_application application, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteApplicationAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Licence
    // -------------------------------------------------------------------------
    public Task<adoxio_licences?> GetLicenceByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<adoxio_licences?> GetLicenceByIdWithChildrenAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<adoxio_licences?> GetLicenceByNumberAsync(string licenceNumber, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IList<adoxio_licences>> GetLicencesByAccountIdAsync(string accountId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateLicenceAsync(adoxio_licences licence, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Worker
    // -------------------------------------------------------------------------
    public Task<adoxio_worker?> GetWorkerByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<adoxio_worker?> GetWorkerByIdWithChildrenAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateWorkerAsync(adoxio_worker worker, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateWorkerAsync(adoxio_worker worker, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Establishment
    // -------------------------------------------------------------------------
    public Task<adoxio_establishment?> GetEstablishmentByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IList<adoxio_establishment>> GetEstablishmentsByAccountIdAsync(string accountId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateEstablishmentAsync(adoxio_establishment establishment, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Legal Entity
    // -------------------------------------------------------------------------
    public Task<adoxio_legalentity?> GetLegalEntityByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IList<adoxio_legalentity>> GetLegalEntitiesByAccountIdAsync(string accountId, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Tied House Connection
    // -------------------------------------------------------------------------
    public Task<IList<adoxio_tiedhouseconnection>> GetTiedHouseConnectionsByAccountIdAsync(string accountId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateTiedHouseConnectionAsync(adoxio_tiedhouseconnection connection, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteTiedHouseConnectionAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Special Event
    // -------------------------------------------------------------------------
    public Task<adoxio_specialevent?> GetSpecialEventByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<adoxio_specialevent?> GetSpecialEventByIdWithChildrenAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<adoxio_specialevent?> GetSpecialEventByLicenceNumberAsync(string licenceNumber, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Event
    // -------------------------------------------------------------------------
    public Task<adoxio_event?> GetEventByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<adoxio_event?> GetEventByIdWithChildrenAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IList<adoxio_eventschedule>> GetEventSchedulesByEventIdAsync(string eventId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<IList<adoxio_eventlocation>> GetEventLocationsByEventIdAsync(string eventId, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Annotation
    // -------------------------------------------------------------------------
    public Task<IList<Annotation>> GetAnnotationsByObjectIdAsync(string objectId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Annotation?> GetAnnotationByIdAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateAnnotationAsync(Annotation annotation, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAnnotationAsync(Annotation annotation, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task DeleteAnnotationAsync(string id, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // SharePoint document location
    // -------------------------------------------------------------------------
    public Task<SharePointDocumentLocation?> GetSharePointDocLocByObjectIdAsync(string objectId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<Guid> CreateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default)
        => throw new NotImplementedException();

    // -------------------------------------------------------------------------
    // Pagination
    // -------------------------------------------------------------------------
    public Task<(IList<T> Results, string? NextPagingCookie)> RetrievePagedAsync<T>(
        QueryExpression query,
        int pageSize = 5000,
        string? pagingCookie = null,
        CancellationToken ct = default) where T : Entity
        => throw new NotImplementedException();
}
