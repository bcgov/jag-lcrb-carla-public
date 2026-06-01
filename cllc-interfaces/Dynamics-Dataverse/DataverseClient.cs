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
    public async Task<adoxio_application?> GetApplicationByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await Task.Run(() =>
                _serviceClient.Retrieve(adoxio_application.EntityLogicalName, guid, new ColumnSet(true)), ct);
            return entity?.ToEntity<adoxio_application>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<adoxio_application?> GetApplicationByIdWithChildrenAsync(string id, CancellationToken ct = default)
    {
        var application = await GetApplicationByIdAsync(id, ct);
        if (application == null) return null;

        var appId = application.Id;

        var licenceTask = application.adoxio_AssignedLicence?.Id is Guid licenceId
            ? Task.Run<Entity?>(() => _serviceClient.Retrieve(adoxio_licences.EntityLogicalName, licenceId, new ColumnSet(true)), ct)
            : Task.FromResult<Entity?>(null);

        var establishmentTask = application.adoxio_LicenceEstablishment?.Id is Guid estId
            ? Task.Run<Entity?>(() => _serviceClient.Retrieve(adoxio_establishment.EntityLogicalName, estId, new ColumnSet(true)), ct)
            : Task.FromResult<Entity?>(null);

        var leQuery = new QueryExpression(adoxio_legalentity.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        leQuery.Criteria.AddCondition("adoxio_relatedapplication", ConditionOperator.Equal, appId);
        var leTask = Task.Run(() => _serviceClient.RetrieveMultiple(leQuery), ct);

        await Task.WhenAll(licenceTask, establishmentTask, leTask);

        var licence = (await licenceTask)?.ToEntity<adoxio_licences>();
        var establishment = (await establishmentTask)?.ToEntity<adoxio_establishment>();
        var legalEntities = (await leTask).Entities.Select(e => e.ToEntity<adoxio_legalentity>()).ToList();

        if (licence != null)
            application.RelatedEntities[new Relationship("adoxio_adoxio_licences_adoxio_application_AssignedLicence")] =
                new EntityCollection(new List<Entity> { licence });
        if (establishment != null)
            application.RelatedEntities[new Relationship("adoxio_adoxio_establishment_adoxio_application_Establishment")] =
                new EntityCollection(new List<Entity> { establishment });
        if (legalEntities.Count > 0)
            application.RelatedEntities[new Relationship("adoxio_adoxio_application_adoxio_legalentity_RelatedApplication")] =
                new EntityCollection(legalEntities.Cast<Entity>().ToList());

        return application;
    }

    public async Task<IList<adoxio_application>> GetApplicationsByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicant", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<Guid> CreateApplicationAsync(adoxio_application application, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(application), ct);

    public async Task UpdateApplicationAsync(adoxio_application application, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(application), ct);

    public async Task DeleteApplicationAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await Task.Run(() => _serviceClient.Delete(adoxio_application.EntityLogicalName, guid), ct);
    }

    public async Task<Guid> CreateApplicationExtensionAsync(adoxio_applicationextension extension, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(extension), ct);

    public async Task UpdateApplicationExtensionAsync(adoxio_applicationextension extension, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(extension), ct);

    public async Task<Guid> CreateAnnualVolumeAsync(adoxio_annualvolume annualVolume, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(annualVolume), ct);

    // -------------------------------------------------------------------------
    // Licence
    // -------------------------------------------------------------------------
    public async Task<adoxio_licences?> GetLicenceByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await Task.Run(() =>
                _serviceClient.Retrieve(adoxio_licences.EntityLogicalName, guid, new ColumnSet(true)), ct);
            return entity?.ToEntity<adoxio_licences>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<adoxio_licences?> GetLicenceByIdWithChildrenAsync(string id, CancellationToken ct = default)
    {
        var licence = await GetLicenceByIdAsync(id, ct);
        if (licence == null) return null;

        var licenceId = licence.Id;

        var saQuery = new QueryExpression(adoxio_servicearea.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        saQuery.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, licenceId);
        var saTask = Task.Run(() => _serviceClient.RetrieveMultiple(saQuery), ct);

        var hosQuery = new QueryExpression(adoxio_hoursofservice.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        hosQuery.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, licenceId);
        var hosTask = Task.Run(() => _serviceClient.RetrieveMultiple(hosQuery), ct);

        var ossQuery = new QueryExpression(adoxio_offsitestorage.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        ossQuery.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, licenceId);
        var ossTask = Task.Run(() => _serviceClient.RetrieveMultiple(ossQuery), ct);

        var tclQuery = new QueryExpression(adoxio_applicationtermsconditionslimitation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        tclQuery.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, licenceId);
        var tclTask = Task.Run(() => _serviceClient.RetrieveMultiple(tclQuery), ct);

        await Task.WhenAll(saTask, hosTask, ossTask, tclTask);

        var serviceAreas = (await saTask).Entities;
        var hoursOfSale = (await hosTask).Entities;
        var offSiteStorages = (await ossTask).Entities;
        var termsConditions = (await tclTask).Entities;

        if (serviceAreas.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_licences_adoxio_servicearea")] =
                new EntityCollection(serviceAreas.ToList());
        if (hoursOfSale.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_licences_adoxio_hoursofservice")] =
                new EntityCollection(hoursOfSale.ToList());
        if (offSiteStorages.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_licences_adoxio_offsitestorage")] =
                new EntityCollection(offSiteStorages.ToList());
        if (termsConditions.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_licences_adoxio_applicationtermsconditionslimitation")] =
                new EntityCollection(termsConditions.ToList());

        return licence;
    }

    public async Task<adoxio_licences?> GetLicenceByNumberAsync(string licenceNumber, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_licences.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            TopCount = 1
        };
        query.Criteria.AddCondition("adoxio_licencenumber", ConditionOperator.Equal, licenceNumber);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_licences>();
    }

    public async Task<IList<adoxio_licences>> GetLicencesByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_licences>();
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licencee", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task UpdateLicenceAsync(adoxio_licences licence, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(licence), ct);

    // -------------------------------------------------------------------------
    // Service Area (adoxio_servicearea)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_servicearea>> GetServiceAreasByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_servicearea>();
        var query = new QueryExpression(adoxio_servicearea.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_servicearea>()).ToList();
    }

    public async Task<Guid> CreateServiceAreaAsync(adoxio_servicearea serviceArea, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(serviceArea), ct);

    public async Task UpdateServiceAreaAsync(adoxio_servicearea serviceArea, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(serviceArea), ct);

    public async Task DeleteServiceAreaAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await Task.Run(() => _serviceClient.Delete(adoxio_servicearea.EntityLogicalName, guid), ct);
    }

    // -------------------------------------------------------------------------
    // Hour of Sale (adoxio_hoursofservice)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_hoursofservice>> GetHoursOfSaleByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_hoursofservice>();
        var query = new QueryExpression(adoxio_hoursofservice.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_hoursofservice>()).ToList();
    }

    public async Task<Guid> CreateHourOfSaleAsync(adoxio_hoursofservice hourOfSale, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(hourOfSale), ct);

    public async Task UpdateHourOfSaleAsync(adoxio_hoursofservice hourOfSale, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(hourOfSale), ct);

    public async Task DeleteHourOfSaleAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await Task.Run(() => _serviceClient.Delete(adoxio_hoursofservice.EntityLogicalName, guid), ct);
    }

    // -------------------------------------------------------------------------
    // Off-Site Storage (adoxio_offsitestorage)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_offsitestorage>> GetOffSiteStorageByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_offsitestorage>();
        var query = new QueryExpression(adoxio_offsitestorage.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_offsitestorage>()).ToList();
    }

    public async Task<Guid> CreateOffSiteStorageAsync(adoxio_offsitestorage storage, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(storage), ct);

    public async Task DeleteOffSiteStorageAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await Task.Run(() => _serviceClient.Delete(adoxio_offsitestorage.EntityLogicalName, guid), ct);
    }

    // -------------------------------------------------------------------------
    // Application Terms Conditions Limitation (adoxio_applicationtermsconditionslimitation)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_applicationtermsconditionslimitation>> GetTermsConditionsByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_applicationtermsconditionslimitation>();
        var query = new QueryExpression(adoxio_applicationtermsconditionslimitation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtermsconditionslimitation>()).ToList();
    }

    public async Task<Guid> CreateTermsConditionsAsync(adoxio_applicationtermsconditionslimitation terms, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(terms), ct);

    public async Task UpdateTermsConditionsAsync(adoxio_applicationtermsconditionslimitation terms, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(terms), ct);

    // -------------------------------------------------------------------------
    // Worker (adoxio_worker)
    // -------------------------------------------------------------------------
    public async Task<adoxio_worker?> GetWorkerByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await Task.Run(() =>
                _serviceClient.Retrieve(adoxio_worker.EntityLogicalName, guid, new ColumnSet(true)), ct);
            return entity?.ToEntity<adoxio_worker>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<adoxio_worker?> GetWorkerByIdWithChildrenAsync(string id, CancellationToken ct = default)
    {
        var worker = await GetWorkerByIdAsync(id, ct);
        if (worker == null) return null;

        var phsQuery = new QueryExpression(adoxio_personalhistorysummary.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        phsQuery.Criteria.AddCondition("adoxio_workerid", ConditionOperator.Equal, worker.Id);
        var phsTask = Task.Run(() => _serviceClient.RetrieveMultiple(phsQuery), ct);

        var prevAddrQuery = new QueryExpression(adoxio_previousaddress.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        prevAddrQuery.Criteria.AddCondition("adoxio_workerid", ConditionOperator.Equal, worker.Id);
        var prevAddrTask = Task.Run(() => _serviceClient.RetrieveMultiple(prevAddrQuery), ct);

        await Task.WhenAll(phsTask, prevAddrTask);

        var phs = (await phsTask).Entities;
        var prevAddresses = (await prevAddrTask).Entities;

        if (phs.Count > 0)
            worker.RelatedEntities[new Relationship("adoxio_worker_adoxio_personalhistorysummary")] =
                new EntityCollection(phs.ToList());
        if (prevAddresses.Count > 0)
            worker.RelatedEntities[new Relationship("adoxio_previousaddress_worker")] =
                new EntityCollection(prevAddresses.ToList());

        return worker;
    }

    public async Task<Guid> CreateWorkerAsync(adoxio_worker worker, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(worker), ct);

    public async Task UpdateWorkerAsync(adoxio_worker worker, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(worker), ct);

    // -------------------------------------------------------------------------
    // Personal History Summary (adoxio_personalhistorysummary)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_personalhistorysummary>> GetPersonalHistorySummariesByWorkerIdAsync(string workerId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(workerId, out var guid)) return new List<adoxio_personalhistorysummary>();
        var query = new QueryExpression(adoxio_personalhistorysummary.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_workerid", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_personalhistorysummary>()).ToList();
    }

    public async Task<Guid> CreatePersonalHistorySummaryAsync(adoxio_personalhistorysummary summary, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(summary), ct);

    public async Task UpdatePersonalHistorySummaryAsync(adoxio_personalhistorysummary summary, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(summary), ct);

    // -------------------------------------------------------------------------
    // Previous Address (adoxio_previousaddress)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_previousaddress>> GetPreviousAddressesByWorkerIdAsync(string workerId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(workerId, out var guid)) return new List<adoxio_previousaddress>();
        var query = new QueryExpression(adoxio_previousaddress.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_workerid", ConditionOperator.Equal, guid);
        var result = await Task.Run(() => _serviceClient.RetrieveMultiple(query), ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_previousaddress>()).ToList();
    }

    public async Task<Guid> CreatePreviousAddressAsync(adoxio_previousaddress address, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Create(address), ct);

    public async Task UpdatePreviousAddressAsync(adoxio_previousaddress address, CancellationToken ct = default)
        => await Task.Run(() => _serviceClient.Update(address), ct);

    public async Task DeletePreviousAddressAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await Task.Run(() => _serviceClient.Delete(adoxio_previousaddress.EntityLogicalName, guid), ct);
    }

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
