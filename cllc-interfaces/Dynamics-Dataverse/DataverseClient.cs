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
        // Prefer DYNAMICS_NATIVE_ODATA_URI — this is the direct CRM org URL.
        // DYNAMICS_ODATA_URI points to a gateway proxy and cannot be used for ServiceClient auth.
        var url = configuration["DYNAMICS_NATIVE_ODATA_URI"];
        if (string.IsNullOrEmpty(url))
            url = configuration["DYNAMICS_ODATA_URI"];
        if (string.IsNullOrEmpty(url))
            throw new InvalidOperationException("DYNAMICS_NATIVE_ODATA_URI (or DYNAMICS_ODATA_URI) is not configured.");

        // Strip /api/data/vX.X/ suffix if present — ServiceClient needs the base org URL
        var orgUrl = ExtractOrgUrl(url);

        var tenantId = configuration["DYNAMICS_AAD_TENANT_ID"];
        if (string.IsNullOrEmpty(tenantId))
            throw new InvalidOperationException("DYNAMICS_AAD_TENANT_ID is not configured.");

        var clientId = configuration["DYNAMICS_APP_REG_CLIENT_ID"];
        if (string.IsNullOrEmpty(clientId))
            throw new InvalidOperationException("DYNAMICS_APP_REG_CLIENT_ID is not configured.");

        var clientSecret = configuration["DYNAMICS_APP_REG_CLIENT_KEY"];
        if (string.IsNullOrEmpty(clientSecret))
            throw new InvalidOperationException("DYNAMICS_APP_REG_CLIENT_KEY is not configured.");

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
    public async Task<Account?> GetAccountByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(Account.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<Account>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<Account?> GetAccountByIdWithChildrenAsync(string id, CancellationToken ct = default)
    {
        var account = await GetAccountByIdAsync(id, ct);
        if (account == null) return null;

        var accountId = account.Id;

        var estQuery = new QueryExpression(adoxio_establishment.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        estQuery.Criteria.AddCondition("adoxio_licencee", ConditionOperator.Equal, accountId);
        var estTask = _serviceClient.RetrieveMultipleAsync(estQuery, ct);

        var leQuery = new QueryExpression(adoxio_legalentity.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        leQuery.Criteria.AddCondition("adoxio_account", ConditionOperator.Equal, accountId);
        var leTask = _serviceClient.RetrieveMultipleAsync(leQuery, ct);

        var thcQuery = new QueryExpression(adoxio_tiedhouseconnection.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        thcQuery.Criteria.AddCondition("adoxio_accountid", ConditionOperator.Equal, accountId);
        var thcTask = _serviceClient.RetrieveMultipleAsync(thcQuery, ct);

        await Task.WhenAll(estTask, leTask, thcTask);

        var establishments = (await estTask).Entities;
        var legalEntities = (await leTask).Entities;
        var tiedHouseConnections = (await thcTask).Entities;

        if (establishments.Count > 0)
            account.RelatedEntities[new Relationship("adoxio_account_adoxio_establishment_Licencee")] =
                new EntityCollection(establishments.ToList());
        if (legalEntities.Count > 0)
            account.RelatedEntities[new Relationship("adoxio_account_adoxio_legalentity_Account")] =
                new EntityCollection(legalEntities.ToList());
        if (tiedHouseConnections.Count > 0)
            account.RelatedEntities[new Relationship("adoxio_account_adoxio_tiedhouseconnection_Licensee")] =
                new EntityCollection(tiedHouseConnections.ToList());

        return account;
    }

    public async Task<Account?> GetAccountByNameAsync(string name, CancellationToken ct = default)
    {
        var query = new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            TopCount = 2
        };
        query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        if (result.Entities.Count != 1)
            return null;
        return result.Entities[0].ToEntity<Account>();
    }

    public async Task<IList<Account>> GetAccountsAsync(string? filter = null, bool activeOnly = false, CancellationToken ct = default)
    {
        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        if (!string.IsNullOrEmpty(filter))
            query.Criteria.AddCondition("name", ConditionOperator.Like, filter);
        if (activeOnly)
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<Account>()).ToList();
    }

    public async Task<Guid> CreateAccountAsync(Account account, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(account, ct);

    public async Task UpdateAccountAsync(Account account, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(account, ct);

    // -------------------------------------------------------------------------
    // Contact
    // -------------------------------------------------------------------------
    public async Task<Contact?> GetContactByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(Contact.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<Contact>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<Contact?> GetContactByExternalIdAsync(string externalId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(externalId)) return null;
        // Normalize to no-hyphens uppercase — the canonical stored format for new records.
        var normalized = externalId.Replace("-", "").ToUpperInvariant();
        var query = new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        // Match either the normalized form or the standard hyphenated GUID form to handle
        // contacts whose adoxio_ExternalID was stored by the old Dynamics client with hyphens.
        var idFilter = new FilterExpression(LogicalOperator.Or);
        idFilter.AddCondition("adoxio_externalid", ConditionOperator.Equal, normalized);
        if (Guid.TryParse(normalized, out var parsedGuid))
        {
            idFilter.AddCondition("adoxio_externalid", ConditionOperator.Equal, parsedGuid.ToString("D").ToUpperInvariant());
        }
        query.Criteria.AddFilter(idFilter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<Contact>();
    }

    public async Task<Guid> CreateContactAsync(Contact contact, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(contact, ct);

    public async Task UpdateContactAsync(Contact contact, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(contact, ct);

    // -------------------------------------------------------------------------
    // Alias
    // -------------------------------------------------------------------------
    public async Task<Guid> CreateAliasAsync(adoxio_alias alias, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(alias, ct);

    public async Task UpdateAliasAsync(adoxio_alias alias, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(alias, ct);

    // -------------------------------------------------------------------------
    // Application
    // -------------------------------------------------------------------------
    public async Task<adoxio_application?> GetApplicationByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_application.EntityLogicalName, guid, new ColumnSet(true), ct);
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
            ? _serviceClient.RetrieveAsync(adoxio_licences.EntityLogicalName, licenceId, new ColumnSet(true), ct)
            : Task.FromResult<Entity?>(null);

        var establishmentTask = application.adoxio_LicenceEstablishment?.Id is Guid estId
            ? _serviceClient.RetrieveAsync(adoxio_establishment.EntityLogicalName, estId, new ColumnSet(true), ct)
            : Task.FromResult<Entity?>(null);

        var leQuery = new QueryExpression(adoxio_legalentity.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        leQuery.Criteria.AddCondition("adoxio_relatedapplication", ConditionOperator.Equal, appId);
        var leTask = _serviceClient.RetrieveMultipleAsync(leQuery, ct);

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
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<Guid> CreateApplicationAsync(adoxio_application application, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(application, ct);

    public async Task UpdateApplicationAsync(adoxio_application application, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(application, ct);

    public async Task DeleteApplicationAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_application.EntityLogicalName, guid, ct);
    }

    public async Task<Guid> CreateApplicationExtensionAsync(adoxio_applicationextension extension, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(extension, ct);

    public async Task UpdateApplicationExtensionAsync(adoxio_applicationextension extension, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(extension, ct);

    public async Task<adoxio_applicationextension?> GetApplicationExtensionByApplicationIdAsync(string applicationId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationId, out var guid)) return null;
        var query = new QueryExpression(adoxio_applicationextension.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_application", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_applicationextension>();
    }

    public async Task<adoxio_applicationextension?> GetApplicationExtensionByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_applicationextension.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_applicationextension>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<IList<adoxio_application>> GetApplicationsByApplicantExpandedAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicant", ConditionOperator.Equal, guid);
        foreach (var status in new[]
        {
            (int)adoxio_application_statuscode.Terminated,
            (int)adoxio_application_statuscode.Refused,
            (int)adoxio_application_statuscode.Cancelled,
            (int)adoxio_application_statuscode.TerminatedandRefunded
        })
            query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, status);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<IList<adoxio_application>> GetApplicationsByApplicantAndTypeAsync(
        string accountId,
        string? applicationTypeId,
        IList<int>? excludeStatuses,
        bool requireStatecode0 = false,
        string? specificApplicationId = null,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var accountGuid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicant", ConditionOperator.Equal, accountGuid);

        if (!string.IsNullOrEmpty(applicationTypeId) && Guid.TryParse(applicationTypeId, out var typeGuid))
            query.Criteria.AddCondition("adoxio_applicationtypeid", ConditionOperator.Equal, typeGuid);

        if (requireStatecode0)
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

        if (excludeStatuses != null)
            foreach (var status in excludeStatuses)
                query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, status);

        if (!string.IsNullOrEmpty(specificApplicationId) && Guid.TryParse(specificApplicationId, out var appGuid))
            query.Criteria.AddCondition("adoxio_applicationid", ConditionOperator.Equal, appGuid);

        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<Guid> CreateAnnualVolumeAsync(adoxio_annualvolume annualVolume, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(annualVolume, ct);

    // -------------------------------------------------------------------------
    // Licence
    // -------------------------------------------------------------------------
    public async Task<adoxio_licences?> GetLicenceByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_licences.EntityLogicalName, guid, new ColumnSet(true), ct);
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
        var saTask = _serviceClient.RetrieveMultipleAsync(saQuery, ct);

        var hosQuery = new QueryExpression(adoxio_hoursofservice.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        hosQuery.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, licenceId);
        var hosTask = _serviceClient.RetrieveMultipleAsync(hosQuery, ct);

        var ossQuery = new QueryExpression(adoxio_offsitestorage.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        ossQuery.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, licenceId);
        var ossTask = _serviceClient.RetrieveMultipleAsync(ossQuery, ct);

        var tclQuery = new QueryExpression(adoxio_applicationtermsconditionslimitation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        tclQuery.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, licenceId);
        var tclTask = _serviceClient.RetrieveMultipleAsync(tclQuery, ct);

        await Task.WhenAll(saTask, hosTask, ossTask, tclTask);

        var serviceAreas = (await saTask).Entities;
        var hoursOfSale = (await hosTask).Entities;
        var offSiteStorages = (await ossTask).Entities;
        var termsConditions = (await tclTask).Entities;

        if (serviceAreas.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_licence_serviceareas")] =
                new EntityCollection(serviceAreas.ToList());
        if (hoursOfSale.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_licences_adoxio_hoursofservice_Licence")] =
                new EntityCollection(hoursOfSale.ToList());
        if (offSiteStorages.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_licences_offsitestoragelocations")] =
                new EntityCollection(offSiteStorages.ToList());
        if (termsConditions.Count > 0)
            licence.RelatedEntities[new Relationship("adoxio_adoxio_licences_adoxio_applicationtermsconditionslimitation_Licence")] =
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
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_licences>();
    }

    public async Task<IList<adoxio_licences>> GetActiveLicencesByTypeIdsAsync(IList<string> licenceTypeIds, CancellationToken ct = default)
    {
        if (licenceTypeIds.Count == 0) return new List<adoxio_licences>();
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
        var typeFilter = new FilterExpression(LogicalOperator.Or);
        foreach (var typeId in licenceTypeIds)
            if (Guid.TryParse(typeId, out var tGuid))
                typeFilter.AddCondition("adoxio_licencetype", ConditionOperator.Equal, tGuid);
        query.Criteria.AddFilter(typeFilter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task<IList<adoxio_licences>> GetLicencesByIdsAsync(IList<string> ids, CancellationToken ct = default)
    {
        if (ids.Count == 0) return new List<adoxio_licences>();
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var filter = new FilterExpression(LogicalOperator.Or);
        foreach (var id in ids)
            if (Guid.TryParse(id, out var guid))
                filter.AddCondition("adoxio_licencesid", ConditionOperator.Equal, guid);
        query.Criteria.AddFilter(filter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task<IList<adoxio_licences>> GetLicencesByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_licences>();
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licencee", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task<IList<adoxio_licences>> GetLicencesByNameOrNumberAsync(string? name, string? licenceNumber, int top = 10, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = top };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        // Contains requires a full-text index, which isn't enabled on these
        // attributes in this org — Like performs the same substring match
        // via SQL LIKE without that dependency.
        var orFilter = new FilterExpression(LogicalOperator.Or);
        if (!string.IsNullOrWhiteSpace(name))
            orFilter.AddCondition("adoxio_name", ConditionOperator.Like, $"%{name}%");
        if (!string.IsNullOrWhiteSpace(licenceNumber))
            orFilter.AddCondition("adoxio_licencenumber", ConditionOperator.Like, $"%{licenceNumber}%");
        if (orFilter.Conditions.Count > 0)
            query.Criteria.AddFilter(orFilter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task UpdateLicenceAsync(adoxio_licences licence, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(licence, ct);

    // -------------------------------------------------------------------------
    // Service Area (adoxio_servicearea)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_servicearea>> GetServiceAreasByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_servicearea>();
        var query = new QueryExpression(adoxio_servicearea.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_servicearea>()).ToList();
    }

    public async Task<IList<adoxio_servicearea>> GetServiceAreasByApplicationIdAsync(string applicationId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationId, out var guid)) return new List<adoxio_servicearea>();
        var query = new QueryExpression(adoxio_servicearea.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicationid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_servicearea>()).ToList();
    }

    public async Task<Guid> CreateServiceAreaAsync(adoxio_servicearea serviceArea, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(serviceArea, ct);

    public async Task UpdateServiceAreaAsync(adoxio_servicearea serviceArea, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(serviceArea, ct);

    public async Task DeleteServiceAreaAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_servicearea.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Hour of Sale (adoxio_hoursofservice)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_hoursofservice>> GetHoursOfSaleByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_hoursofservice>();
        var query = new QueryExpression(adoxio_hoursofservice.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_hoursofservice>()).ToList();
    }

    public async Task<adoxio_hoursofservice?> GetHoursOfServiceByApplicationIdAsync(string applicationId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationId, out var guid)) return null;
        var query = new QueryExpression(adoxio_hoursofservice.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_application", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_hoursofservice>();
    }

    public async Task<Guid> CreateHourOfSaleAsync(adoxio_hoursofservice hourOfSale, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(hourOfSale, ct);

    public async Task<Guid> CreateHoursOfServiceAsync(adoxio_hoursofservice hoursOfService, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(hoursOfService, ct);

    public async Task UpdateHourOfSaleAsync(adoxio_hoursofservice hourOfSale, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(hourOfSale, ct);

    public async Task UpdateHoursOfServiceAsync(adoxio_hoursofservice hoursOfService, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(hoursOfService, ct);

    public async Task DeleteHourOfSaleAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_hoursofservice.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Off-Site Storage (adoxio_offsitestorage)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_offsitestorage>> GetOffSiteStorageByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_offsitestorage>();
        var query = new QueryExpression(adoxio_offsitestorage.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_offsitestorage>()).ToList();
    }

    public async Task<Guid> CreateOffSiteStorageAsync(adoxio_offsitestorage storage, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(storage, ct);

    public async Task UpdateOffSiteStorageAsync(adoxio_offsitestorage storage, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(storage, ct);

    public async Task DeleteOffSiteStorageAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_offsitestorage.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Application Terms Conditions Limitation (adoxio_applicationtermsconditionslimitation)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_applicationtermsconditionslimitation>> GetTermsConditionsByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_applicationtermsconditionslimitation>();
        var query = new QueryExpression(adoxio_applicationtermsconditionslimitation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtermsconditionslimitation>()).ToList();
    }

    public async Task<IList<adoxio_applicationtermsconditionslimitation>> GetTermsConditionsByLicenceIdsAsync(IEnumerable<string> licenceIds, CancellationToken ct = default)
    {
        var guids = licenceIds.Select(id => Guid.TryParse(id, out var g) ? g : (Guid?)null)
            .Where(g => g.HasValue).Select(g => g.Value).Cast<object>().ToArray();
        if (guids.Length == 0) return new List<adoxio_applicationtermsconditionslimitation>();
        var query = new QueryExpression(adoxio_applicationtermsconditionslimitation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.In, guids);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtermsconditionslimitation>()).ToList();
    }

    public async Task<Guid> CreateTermsConditionsAsync(adoxio_applicationtermsconditionslimitation terms, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(terms, ct);

    public async Task UpdateTermsConditionsAsync(adoxio_applicationtermsconditionslimitation terms, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(terms, ct);

    // -------------------------------------------------------------------------
    // Worker (adoxio_worker)
    // -------------------------------------------------------------------------
    public async Task<adoxio_worker?> GetWorkerByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_worker.EntityLogicalName, guid, new ColumnSet(true), ct);
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
        var phsTask = _serviceClient.RetrieveMultipleAsync(phsQuery, ct);

        var prevAddrQuery = new QueryExpression(adoxio_previousaddress.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        prevAddrQuery.Criteria.AddCondition("adoxio_workerid", ConditionOperator.Equal, worker.Id);
        var prevAddrTask = _serviceClient.RetrieveMultipleAsync(prevAddrQuery, ct);

        await Task.WhenAll(phsTask, prevAddrTask);

        var phs = (await phsTask).Entities;
        var prevAddresses = (await prevAddrTask).Entities;

        if (phs.Count > 0)
            worker.RelatedEntities[new Relationship("adoxio_workerregistration_personalhistorysummary")] =
                new EntityCollection(phs.ToList());
        if (prevAddresses.Count > 0)
            worker.RelatedEntities[new Relationship("adoxio_worker_previousaddresses")] =
                new EntityCollection(prevAddresses.ToList());

        return worker;
    }

    public async Task<Guid> CreateWorkerAsync(adoxio_worker worker, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(worker, ct);

    public async Task UpdateWorkerAsync(adoxio_worker worker, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(worker, ct);

    public async Task<IList<adoxio_worker>> GetWorkersByContactIdAsync(string contactId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(contactId, out var guid)) return new List<adoxio_worker>();
        var query = new QueryExpression(adoxio_worker.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_contactid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_worker>()).ToList();
    }

    public async Task DeleteWorkerAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_worker.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Personal History Summary (adoxio_personalhistorysummary)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_personalhistorysummary>> GetPersonalHistorySummariesByWorkerIdAsync(string workerId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(workerId, out var guid)) return new List<adoxio_personalhistorysummary>();
        var query = new QueryExpression(adoxio_personalhistorysummary.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_workerid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_personalhistorysummary>()).ToList();
    }

    public async Task<Guid> CreatePersonalHistorySummaryAsync(adoxio_personalhistorysummary summary, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(summary, ct);

    public async Task UpdatePersonalHistorySummaryAsync(adoxio_personalhistorysummary summary, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(summary, ct);

    // -------------------------------------------------------------------------
    // Previous Address (adoxio_previousaddress)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_previousaddress>> GetPreviousAddressesByWorkerIdAsync(string workerId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(workerId, out var guid)) return new List<adoxio_previousaddress>();
        var query = new QueryExpression(adoxio_previousaddress.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_workerid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_previousaddress>()).ToList();
    }

    public async Task<Guid> CreatePreviousAddressAsync(adoxio_previousaddress address, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(address, ct);

    public async Task UpdatePreviousAddressAsync(adoxio_previousaddress address, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(address, ct);

    public async Task DeletePreviousAddressAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_previousaddress.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Establishment
    // -------------------------------------------------------------------------
    public async Task<adoxio_establishment?> GetEstablishmentByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_establishment.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_establishment>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<IList<adoxio_establishment>> GetEstablishmentsByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_establishment>();
        var query = new QueryExpression(adoxio_establishment.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licencee", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_establishment>()).ToList();
    }

    public async Task<IList<adoxio_establishment>> GetEstablishmentsByNameAsync(string name, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_establishment.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_name", ConditionOperator.Equal, name);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_establishment>()).ToList();
    }

    public async Task<Guid> CreateEstablishmentAsync(adoxio_establishment establishment, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(establishment, ct);

    public async Task UpdateEstablishmentAsync(adoxio_establishment establishment, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(establishment, ct);

    public async Task DeleteEstablishmentAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_establishment.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Licence Type
    // -------------------------------------------------------------------------
    public async Task<adoxio_licencetype?> GetLicenceTypeByNameAsync(string name, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_licencetype.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            TopCount = 1
        };
        query.Criteria.AddCondition("adoxio_name", ConditionOperator.Equal, name);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_licencetype>();
    }

    // -------------------------------------------------------------------------
    // Local Government / Indigenous Nation
    // -------------------------------------------------------------------------
    public async Task<adoxio_localgovindigenousnation?> GetLginByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_localgovindigenousnation.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_localgovindigenousnation>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    // -------------------------------------------------------------------------
    // Legal Entity
    // -------------------------------------------------------------------------
    public async Task<adoxio_legalentity?> GetLegalEntityByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_legalentity.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_legalentity>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<IList<adoxio_legalentity>> GetLegalEntitiesByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_legalentity>();
        var query = new QueryExpression(adoxio_legalentity.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_account", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_legalentity>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Tied House Connection
    // -------------------------------------------------------------------------
    public async Task<adoxio_tiedhouseconnection?> GetTiedHouseConnectionByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_tiedhouseconnection.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_tiedhouseconnection>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<IList<adoxio_tiedhouseconnection>> GetTiedHouseConnectionsByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_tiedhouseconnection>();
        var query = new QueryExpression(adoxio_tiedhouseconnection.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_accountid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_tiedhouseconnection>()).ToList();
    }

    public async Task<IList<adoxio_tiedhouseconnection>> GetLiquorTiedHouseConnectionsByAccountAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_tiedhouseconnection>();
        var query = new QueryExpression(adoxio_tiedhouseconnection.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_accountid", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)adoxio_tiedhouseconnection_statuscode.Existing);
        query.Criteria.AddCondition("adoxio_categorytype", ConditionOperator.Equal, (int)adoxio_tiedhouseconnection_adoxio_categorytype.Liquor);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_tiedhouseconnection>()).ToList();
    }

    public async Task<adoxio_tiedhouseconnection?> GetCannabisTiedHouseConnectionByAccountAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return null;
        var query = new QueryExpression(adoxio_tiedhouseconnection.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_accountid", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)adoxio_tiedhouseconnection_statuscode.Existing);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var categoryFilter = new FilterExpression(LogicalOperator.Or);
        categoryFilter.AddCondition("adoxio_categorytype", ConditionOperator.Equal, (int)adoxio_tiedhouseconnection_adoxio_categorytype.Cannabis);
        categoryFilter.AddCondition("adoxio_categorytype", ConditionOperator.NotEqual, (int)adoxio_tiedhouseconnection_adoxio_categorytype.Liquor);
        query.Criteria.AddFilter(categoryFilter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities
            .Select(e => e.ToEntity<adoxio_tiedhouseconnection>())
            .OrderByDescending(e => e.adoxio_CategoryType == adoxio_tiedhouseconnection_adoxio_categorytype.Cannabis)
            .ThenByDescending(e => e.ModifiedOn)
            .FirstOrDefault();
    }

    public async Task<IList<adoxio_tiedhouseconnection>> GetTiedHouseConnectionsByApplicationAsync(string applicationId, string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationId, out var appGuid)) return new List<adoxio_tiedhouseconnection>();
        if (!Guid.TryParse(accountId, out var accountGuid)) return new List<adoxio_tiedhouseconnection>();

        var query = new QueryExpression(adoxio_tiedhouseconnection.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.FilterOperator = LogicalOperator.Or;

        var accountFilter = new FilterExpression(LogicalOperator.And);
        accountFilter.AddCondition("adoxio_accountid", ConditionOperator.Equal, accountGuid);
        accountFilter.AddCondition("statuscode", ConditionOperator.Equal, (int)adoxio_tiedhouseconnection_statuscode.Existing);
        accountFilter.AddCondition("adoxio_categorytype", ConditionOperator.Equal, (int)adoxio_tiedhouseconnection_adoxio_categorytype.Liquor);
        accountFilter.AddCondition("statecode", ConditionOperator.Equal, 0);

        var appFilter = new FilterExpression(LogicalOperator.And);
        appFilter.AddCondition("adoxio_application", ConditionOperator.Equal, appGuid);
        appFilter.AddCondition("adoxio_categorytype", ConditionOperator.Equal, (int)adoxio_tiedhouseconnection_adoxio_categorytype.Liquor);
        appFilter.AddCondition("statecode", ConditionOperator.Equal, 0);

        query.Criteria.Filters.Add(accountFilter);
        query.Criteria.Filters.Add(appFilter);

        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_tiedhouseconnection>()).ToList();
    }

    public async Task<IList<adoxio_licences>> GetLicencesByTiedHouseConnectionAsync(string tiedHouseId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(tiedHouseId, out var guid)) return new List<adoxio_licences>();
        var request = new Microsoft.Xrm.Sdk.Messages.RetrieveRequest
        {
            Target = new EntityReference(adoxio_tiedhouseconnection.EntityLogicalName, guid),
            ColumnSet = new ColumnSet(false),
            RelatedEntitiesQuery = new Microsoft.Xrm.Sdk.RelationshipQueryCollection
            {
                {
                    new Relationship("adoxio_adoxio_tiedhouseconnection_adoxio_licence"),
                    new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) }
                }
            }
        };
        var response = (Microsoft.Xrm.Sdk.Messages.RetrieveResponse)await _serviceClient.ExecuteAsync(request, ct);
        var rel = new Relationship("adoxio_adoxio_tiedhouseconnection_adoxio_licence");
        if (response.Entity.RelatedEntities.TryGetValue(rel, out var collection))
            return collection.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
        return new List<adoxio_licences>();
    }

    public async Task<Guid> CreateTiedHouseConnectionAsync(adoxio_tiedhouseconnection connection, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(connection, ct);

    public async Task UpdateTiedHouseConnectionAsync(adoxio_tiedhouseconnection connection, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(connection, ct);

    public async Task DeleteTiedHouseConnectionAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_tiedhouseconnection.EntityLogicalName, guid, ct);
    }

    public async Task AssociateTiedHouseConnectionToLicenceAsync(string tiedHouseId, string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(tiedHouseId, out var thcGuid)) return;
        if (!Guid.TryParse(licenceId, out var licGuid)) return;
        var relationship = new Relationship("adoxio_adoxio_tiedhouseconnection_adoxio_licence");
        var relatedEntities = new EntityReferenceCollection
        {
            new EntityReference(adoxio_licences.EntityLogicalName, licGuid)
        };
        await Task.Run(() => _serviceClient.Associate(adoxio_tiedhouseconnection.EntityLogicalName, thcGuid, relationship, relatedEntities), ct);
    }

    public async Task DisassociateTiedHouseConnectionFromLicenceAsync(string tiedHouseId, string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(tiedHouseId, out var thcGuid)) return;
        if (!Guid.TryParse(licenceId, out var licGuid)) return;
        var relationship = new Relationship("adoxio_adoxio_tiedhouseconnection_adoxio_licence");
        var relatedEntities = new EntityReferenceCollection
        {
            new EntityReference(adoxio_licences.EntityLogicalName, licGuid)
        };
        await Task.Run(() => _serviceClient.Disassociate(adoxio_tiedhouseconnection.EntityLogicalName, thcGuid, relationship, relatedEntities), ct);
    }

    // -------------------------------------------------------------------------
    // Special Event (adoxio_specialevent)
    // -------------------------------------------------------------------------
    public async Task<adoxio_specialevent?> GetSpecialEventByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_specialevent.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_specialevent>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<adoxio_specialevent?> GetSpecialEventByIdWithChildrenAsync(string id, CancellationToken ct = default)
    {
        var specialEvent = await GetSpecialEventByIdAsync(id, ct);
        if (specialEvent == null) return null;

        var forecastQuery = new QueryExpression(adoxio_sepdrinksalesforecast.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        forecastQuery.Criteria.AddCondition("adoxio_specialevent", ConditionOperator.Equal, specialEvent.Id);
        var forecasts = (await _serviceClient.RetrieveMultipleAsync(forecastQuery, ct)).Entities;

        if (forecasts.Count > 0)
            specialEvent.RelatedEntities[new Relationship("adoxio_specialevent_adoxio_sepdrinksalesforecast_SpecialEvent")] =
                new EntityCollection(forecasts.ToList());

        return specialEvent;
    }

    public async Task<adoxio_specialevent?> GetSpecialEventByLicenceNumberAsync(string licenceNumber, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_specialevent.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            TopCount = 1
        };
        query.Criteria.AddCondition("adoxio_specialeventpermitnumber", ConditionOperator.Equal, licenceNumber);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_specialevent>();
    }

    public async Task<Guid> CreateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(specialEvent, ct);

    public async Task UpdateSpecialEventAsync(adoxio_specialevent specialEvent, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(specialEvent, ct);

    // -------------------------------------------------------------------------
    // SEP City (adoxio_sepcity)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_sepcity>> GetSepCitiesAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_sepcity.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_sepcity>()).ToList();
    }

    public async Task<adoxio_sepcity?> GetSepCityByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_sepcity.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_sepcity>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    // -------------------------------------------------------------------------
    // SEP Drink Type (adoxio_sepdrinktype)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_sepdrinktype>> GetSepDrinkTypesAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_sepdrinktype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_sepdrinktype>()).ToList();
    }

    // -------------------------------------------------------------------------
    // SEP summary queries
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_specialevent>> GetSpecialEventsByApplicantAsync(string contactId, string? accountId, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_specialevent.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var orFilter = new FilterExpression(LogicalOperator.Or);
        if (Guid.TryParse(contactId, out var contactGuid))
            orFilter.AddCondition("adoxio_contactid", ConditionOperator.Equal, contactGuid);
        if (!string.IsNullOrEmpty(accountId) && Guid.TryParse(accountId, out var accountGuid))
            orFilter.AddCondition("adoxio_accountid", ConditionOperator.Equal, accountGuid);
        query.Criteria.AddFilter(orFilter);
        query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, (int)adoxio_specialevent_statuscode.Draft);
        query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, (int)adoxio_specialevent_statuscode.Cancelled);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_specialevent>()).ToList();
    }

    public async Task<IList<adoxio_specialevent>> GetSpecialEventsByJurisdictionAsync(string jurisdictionId, int[]? policeApprovals = null, int[]? excludeStatuses = null, CancellationToken ct = default)
    {
        if (!Guid.TryParse(jurisdictionId, out var jurisdGuid)) return new List<adoxio_specialevent>();
        var query = new QueryExpression(adoxio_specialevent.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_policejurisdictionid", ConditionOperator.Equal, jurisdGuid);
        if (policeApprovals?.Length > 0)
        {
            var approvalFilter = new FilterExpression(LogicalOperator.Or);
            foreach (var a in policeApprovals)
                approvalFilter.AddCondition("adoxio_policeapproval", ConditionOperator.Equal, a);
            query.Criteria.AddFilter(approvalFilter);
        }
        if (excludeStatuses?.Length > 0)
            foreach (var s in excludeStatuses)
                query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, s);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_specialevent>()).ToList();
    }

    public async Task<IList<adoxio_specialevent>> GetSpecialEventsByRepresentativeAsync(string contactId, int[]? policeApprovals = null, int[]? excludeStatuses = null, int[]? includeStatuses = null, CancellationToken ct = default)
    {
        if (!Guid.TryParse(contactId, out var contactGuid)) return new List<adoxio_specialevent>();
        var query = new QueryExpression(adoxio_specialevent.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_policerepresentativeid", ConditionOperator.Equal, contactGuid);
        if (policeApprovals?.Length > 0)
        {
            var approvalFilter = new FilterExpression(LogicalOperator.Or);
            foreach (var a in policeApprovals)
                approvalFilter.AddCondition("adoxio_policeapproval", ConditionOperator.Equal, a);
            query.Criteria.AddFilter(approvalFilter);
        }
        if (excludeStatuses?.Length > 0)
            foreach (var s in excludeStatuses)
                query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, s);
        if (includeStatuses?.Length > 0)
        {
            var statusFilter = new FilterExpression(LogicalOperator.Or);
            foreach (var s in includeStatuses)
                statusFilter.AddCondition("statuscode", ConditionOperator.Equal, s);
            query.Criteria.AddFilter(statusFilter);
        }
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_specialevent>()).ToList();
    }

    public async Task<(IList<adoxio_specialevent> Results, int TotalCount)> GetSpecialEventsByJurisdictionPagedAsync(
        string jurisdictionId, int[]? policeApprovals, int[]? excludeStatuses,
        int pageIndex, int pageSize, string? orderByField, string? sortDir, CancellationToken ct = default)
    {
        if (!Guid.TryParse(jurisdictionId, out var jurisdGuid))
            return (new List<adoxio_specialevent>(), 0);

        var query = new QueryExpression(adoxio_specialevent.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_policejurisdictionid", ConditionOperator.Equal, jurisdGuid);
        if (policeApprovals?.Length > 0)
        {
            var approvalFilter = new FilterExpression(LogicalOperator.Or);
            foreach (var a in policeApprovals)
                approvalFilter.AddCondition("adoxio_policeapproval", ConditionOperator.Equal, a);
            query.Criteria.AddFilter(approvalFilter);
        }
        if (excludeStatuses?.Length > 0)
            foreach (var s in excludeStatuses)
                query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, s);

        if (!string.IsNullOrEmpty(orderByField))
            query.Orders.Add(new OrderExpression(orderByField,
                string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase)
                    ? OrderType.Descending : OrderType.Ascending));

        query.PageInfo = new PagingInfo { Count = pageSize, PageNumber = 1, ReturnTotalRecordCount = true };
        EntityCollection? result = null;
        string? cookie = null;
        for (int i = 0; i <= pageIndex; i++)
        {
            if (cookie != null) query.PageInfo.PagingCookie = cookie;
            query.PageInfo.PageNumber = i + 1;
            result = await _serviceClient.RetrieveMultipleAsync(query, ct);
            cookie = result.MoreRecords ? result.PagingCookie : null;
            if (i < pageIndex && cookie == null) break;
        }
        var items = result?.Entities.Select(e => e.ToEntity<adoxio_specialevent>()).ToList()
                    ?? new List<adoxio_specialevent>();
        var total = result?.TotalRecordCount ?? 0;
        return (items, total);
    }

    public async Task<IList<adoxio_sepcity>> GetSepCitiesFilteredAsync(string? nameContains, bool defaultsOnly = false, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_sepcity.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 100 };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        if (defaultsOnly)
            query.Criteria.AddCondition("adoxio_ispreview", ConditionOperator.Equal, true);
        else if (!string.IsNullOrEmpty(nameContains))
            query.Criteria.AddCondition("adoxio_name", ConditionOperator.Like, $"%{nameContains}%");
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_sepcity>()).ToList();
    }

    // -------------------------------------------------------------------------
    // SEP Drink Sales Forecast (adoxio_sepdrinksalesforecast)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_sepdrinksalesforecast>> GetSepDrinkSalesForecastsByEventIdAsync(string eventId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(eventId, out var guid)) return new List<adoxio_sepdrinksalesforecast>();
        var query = new QueryExpression(adoxio_sepdrinksalesforecast.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_specialevent", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_sepdrinksalesforecast>()).ToList();
    }

    public async Task<Guid> CreateSepDrinkSalesForecastAsync(adoxio_sepdrinksalesforecast forecast, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(forecast, ct);

    public async Task UpdateSepDrinkSalesForecastAsync(adoxio_sepdrinksalesforecast forecast, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(forecast, ct);

    public async Task DeleteSepDrinkSalesForecastAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_sepdrinksalesforecast.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Special Event Location (adoxio_specialeventlocation)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_specialeventlocation>> GetSpecialEventLocationsByEventIdAsync(string eventId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(eventId, out var guid)) return new List<adoxio_specialeventlocation>();
        var query = new QueryExpression(adoxio_specialeventlocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_specialeventid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_specialeventlocation>()).ToList();
    }

    public async Task<Guid> CreateSpecialEventLocationAsync(adoxio_specialeventlocation location, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(location, ct);

    public async Task UpdateSpecialEventLocationAsync(adoxio_specialeventlocation location, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(location, ct);

    public async Task DeleteSpecialEventLocationAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_specialeventlocation.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Special Event Licenced Area (adoxio_specialeventlicencedarea)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_specialeventlicencedarea>> GetSpecialEventLicencedAreasByLocationIdAsync(string locationId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(locationId, out var guid)) return new List<adoxio_specialeventlicencedarea>();
        var query = new QueryExpression(adoxio_specialeventlicencedarea.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_specialeventlocationid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_specialeventlicencedarea>()).ToList();
    }

    public async Task<Guid> CreateSpecialEventLicencedAreaAsync(adoxio_specialeventlicencedarea area, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(area, ct);

    public async Task UpdateSpecialEventLicencedAreaAsync(adoxio_specialeventlicencedarea area, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(area, ct);

    public async Task DeleteSpecialEventLicencedAreaAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_specialeventlicencedarea.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Special Event Schedule (adoxio_specialeventschedule)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_specialeventschedule>> GetSpecialEventSchedulesByLocationIdAsync(string locationId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(locationId, out var guid)) return new List<adoxio_specialeventschedule>();
        var query = new QueryExpression(adoxio_specialeventschedule.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_specialeventlocationid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_specialeventschedule>()).ToList();
    }

    public async Task<Guid> CreateSpecialEventScheduleAsync(adoxio_specialeventschedule schedule, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(schedule, ct);

    public async Task UpdateSpecialEventScheduleAsync(adoxio_specialeventschedule schedule, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(schedule, ct);

    public async Task DeleteSpecialEventScheduleAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_specialeventschedule.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Special Event T&C (adoxio_specialeventtandc)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_specialeventtandc>> GetSpecialEventTandCsByEventIdAsync(string eventId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(eventId, out var guid)) return new List<adoxio_specialeventtandc>();
        var query = new QueryExpression(adoxio_specialeventtandc.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_specialeventid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_specialeventtandc>()).ToList();
    }

    public async Task<Guid> CreateSpecialEventTandCAsync(adoxio_specialeventtandc tandc, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(tandc, ct);

    public async Task UpdateSpecialEventTandCAsync(adoxio_specialeventtandc tandc, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(tandc, ct);

    public async Task DeleteSpecialEventTandCAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_specialeventtandc.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Event (adoxio_event)
    // -------------------------------------------------------------------------
    public async Task<adoxio_event?> GetEventByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_event.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_event>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<adoxio_event?> GetEventByIdWithChildrenAsync(string id, CancellationToken ct = default)
    {
        var ev = await GetEventByIdAsync(id, ct);
        if (ev == null) return null;

        var schedQuery = new QueryExpression(adoxio_eventschedule.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        schedQuery.Criteria.AddCondition("adoxio_eventid", ConditionOperator.Equal, ev.Id);
        var schedTask = _serviceClient.RetrieveMultipleAsync(schedQuery, ct);

        var locQuery = new QueryExpression(adoxio_eventlocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        locQuery.Criteria.AddCondition("adoxio_eventid", ConditionOperator.Equal, ev.Id);
        var locTask = _serviceClient.RetrieveMultipleAsync(locQuery, ct);

        await Task.WhenAll(schedTask, locTask);

        var schedules = (await schedTask).Entities;
        var locations = (await locTask).Entities;

        if (schedules.Count > 0)
            ev.RelatedEntities[new Relationship("adoxio_event_schedules")] =
                new EntityCollection(schedules.ToList());
        if (locations.Count > 0)
            ev.RelatedEntities[new Relationship("adoxio_event_eventlocations")] =
                new EntityCollection(locations.ToList());

        return ev;
    }

    public async Task<IList<adoxio_eventschedule>> GetEventSchedulesByEventIdAsync(string eventId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(eventId, out var guid)) return new List<adoxio_eventschedule>();
        var query = new QueryExpression(adoxio_eventschedule.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_eventid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_eventschedule>()).ToList();
    }

    public async Task<IList<adoxio_eventlocation>> GetEventLocationsByEventIdAsync(string eventId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(eventId, out var guid)) return new List<adoxio_eventlocation>();
        var query = new QueryExpression(adoxio_eventlocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_eventid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_eventlocation>()).ToList();
    }

    public async Task<IList<adoxio_event>> GetEventsByAccountAndLicenceAsync(string accountId, string licenceId, int top, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var accountGuid)) return new List<adoxio_event>();
        if (!Guid.TryParse(licenceId, out var licenceGuid)) return new List<adoxio_event>();
        var query = new QueryExpression(adoxio_event.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = top };
        query.Criteria.AddCondition("adoxio_account", ConditionOperator.Equal, accountGuid);
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, licenceGuid);
        query.AddOrder("modifiedon", OrderType.Descending);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_event>()).ToList();
    }

    public async Task<IList<adoxio_event>> GetEventsByAccountAndLicencesAsync(string accountId, IEnumerable<string> licenceIds, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var accountGuid)) return new List<adoxio_event>();
        var guids = licenceIds.Select(id => Guid.TryParse(id, out var g) ? g : (Guid?)null)
            .Where(g => g.HasValue).Select(g => g.Value).Cast<object>().ToArray();
        if (guids.Length == 0) return new List<adoxio_event>();
        var query = new QueryExpression(adoxio_event.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_account", ConditionOperator.Equal, accountGuid);
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.In, guids);
        query.AddOrder("modifiedon", OrderType.Descending);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_event>()).ToList();
    }

    public async Task<Guid> CreateEventAsync(adoxio_event evt, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(evt, ct);

    public async Task UpdateEventAsync(adoxio_event evt, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(evt, ct);

    public async Task DeleteEventAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_event.EntityLogicalName, guid, ct);
    }

    public async Task<Guid> CreateEventScheduleAsync(adoxio_eventschedule schedule, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(schedule, ct);

    public async Task DeleteEventScheduleAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_eventschedule.EntityLogicalName, guid, ct);
    }

    public async Task<Guid> CreateEventLocationAsync(adoxio_eventlocation location, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(location, ct);

    public async Task DeleteEventLocationAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_eventlocation.EntityLogicalName, guid, ct);
    }

    public async Task<IList<adoxio_applicationtermsconditionslimitation>> GetTermsConditionsByEventIdAsync(string eventId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(eventId, out var guid)) return new List<adoxio_applicationtermsconditionslimitation>();
        var query = new QueryExpression(adoxio_applicationtermsconditionslimitation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenseeevent", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtermsconditionslimitation>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Annotation
    // -------------------------------------------------------------------------
    public async Task<IList<Annotation>> GetAnnotationsByObjectIdAsync(string objectId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(objectId, out var guid)) return new List<Annotation>();
        var query = new QueryExpression(Annotation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("objectid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<Annotation>()).ToList();
    }

    public async Task<Annotation?> GetAnnotationByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(Annotation.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<Annotation>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<Guid> CreateAnnotationAsync(Annotation annotation, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(annotation, ct);

    public async Task UpdateAnnotationAsync(Annotation annotation, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(annotation, ct);

    public async Task DeleteAnnotationAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(Annotation.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // SharePoint document location
    //
    public async Task CreateWorkerSharePointDocLocAsync(string workerId, string folderName, CancellationToken ct = default)
    {
        if (!Guid.TryParse(workerId, out var workerGuid)) return;

        // Get or create the parent library location for adoxio_worker
        var libQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        libQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, "adoxio_worker");
        var libResult = await _serviceClient.RetrieveMultipleAsync(libQuery, ct);
        Guid parentId;
        var parentLib = libResult.Entities.FirstOrDefault()?.ToEntity<SharePointDocumentLocation>();
        if (parentLib != null)
        {
            parentId = parentLib.Id;
        }
        else
        {
            var newLib = new SharePointDocumentLocation { RelativeUrl = "adoxio_worker" };
            parentId = await _serviceClient.CreateAsync(newLib, ct);
        }

        // Skip if a location already exists for this relative URL linked to the worker
        var existQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        existQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, folderName);
        var existResult = await _serviceClient.RetrieveMultipleAsync(existQuery, ct);
        if (existResult.Entities.Any(e => e.GetAttributeValue<EntityReference>("regardingobjectid")?.Id == workerGuid))
            return;

        var location = new SharePointDocumentLocation
        {
            RelativeUrl = folderName,
            Name = folderName,
            Description = "Worker Files",
            ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, parentId),
            RegardingObjectId = new EntityReference(adoxio_worker.EntityLogicalName, workerGuid)
        };
        await _serviceClient.CreateAsync(location, ct);
    }

    // -------------------------------------------------------------------------
    public async Task<SharePointDocumentLocation?> GetSharePointDocLocByObjectIdAsync(string objectId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(objectId, out var guid)) return null;
        var query = new QueryExpression(SharePointDocumentLocation.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            TopCount = 1
        };
        query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<SharePointDocumentLocation>();
    }

    public async Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByObjectIdAsync(string objectId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(objectId, out var guid)) return new List<SharePointDocumentLocation>();
        var query = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<SharePointDocumentLocation>()).ToList();
    }

    public async Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByRelativeUrlAsync(string relativeUrl, CancellationToken ct = default)
    {
        var query = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, relativeUrl);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<SharePointDocumentLocation>()).ToList();
    }

    public async Task<IList<SharePointDocumentLocation>> GetSharePointDocLocsByRelativeUrlAndNameAsync(string relativeUrl, string name, CancellationToken ct = default)
    {
        var query = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, relativeUrl);
        query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<SharePointDocumentLocation>()).ToList();
    }

    public async Task<Guid> CreateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(location, ct);

    public async Task UpdateSharePointDocLocAsync(SharePointDocumentLocation location, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(location, ct);

    public async Task AssociateFederalReportExportWithDocLocAsync(string exportId, string docLocId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(exportId, out var exportGuid)) return;
        if (!Guid.TryParse(docLocId, out var docLocGuid)) return;
        var relationship = new Relationship("adoxio_federalreportexport_SharePointDocumentLocations");
        var relatedEntities = new EntityReferenceCollection
        {
            new EntityReference(SharePointDocumentLocation.EntityLogicalName, docLocGuid)
        };
        await Task.Run(() => _serviceClient.Associate(adoxio_federalreportexport.EntityLogicalName, exportGuid, relationship, relatedEntities), ct);
    }

    public async Task<string?> GetFolderNameAsync(string entityName, string entityId, CancellationToken ct = default)
    {
        var docLocs = await GetSharePointDocLocsByObjectIdAsync(entityId, ct);
        var existing = docLocs.FirstOrDefault(d => !string.IsNullOrEmpty(d.RelativeUrl));
        if (existing != null) return existing.RelativeUrl;

        // No existing document location — build the folder name from the entity's
        // primary name as "{Name}_{IdCleaned}" (matches how account creation names
        // its folder). Falls back to the cleaned id when the name is unavailable.
        // Previously only "event" was handled here, so uploads to applications,
        // accounts, contacts, workers and licences got a null folder name and threw.
        if (!Guid.TryParse(entityId, out var entityGuid)) return null;

        var (logicalName, nameAttr) = entityName.ToLower() switch
        {
            "account"     => (Account.EntityLogicalName,            "name"),
            "contact"     => (Contact.EntityLogicalName,            "fullname"),
            "application" => (adoxio_application.EntityLogicalName, "adoxio_name"),
            "worker"      => (adoxio_worker.EntityLogicalName,      "adoxio_name"),
            "licence"     => (adoxio_licences.EntityLogicalName,    "adoxio_name"),
            "event"       => (adoxio_event.EntityLogicalName,       "adoxio_name"),
            _             => (null, null)
        };
        if (logicalName == null) return null;

        var idCleaned = entityId.ToUpper().Replace("-", "");
        try
        {
            var entity = await _serviceClient.RetrieveAsync(logicalName, entityGuid, new ColumnSet(true), ct);
            var name = entity?.GetAttributeValue<string>(nameAttr);
            return string.IsNullOrEmpty(name) ? idCleaned : $"{name}_{idCleaned}";
        }
        catch
        {
            // If the entity can't be read for any reason, still return a usable
            // (non-null) folder name so the upload doesn't crash.
            return idCleaned;
        }
    }

    public async Task CreateEntitySharePointDocumentLocationAsync(string entityName, string entityId, string folderName, string name, CancellationToken ct = default)
    {
        if (!Guid.TryParse(entityId, out var entityGuid)) return;

        var (parentLibraryUrl, entityLogicalName) = entityName.ToLower() switch
        {
            "account"     => ("account",             Account.EntityLogicalName),
            "application" => ("adoxio_application",  adoxio_application.EntityLogicalName),
            "contact"     => ("contact",              Contact.EntityLogicalName),
            "worker"      => ("adoxio_worker",        adoxio_worker.EntityLogicalName),
            "event"       => ("adoxio_event",         adoxio_event.EntityLogicalName),
            "licence"     => ("adoxio_licences",      adoxio_licences.EntityLogicalName),
            _             => (null, null)
        };
        if (parentLibraryUrl == null) return;

        var parentQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        parentQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, parentLibraryUrl);
        parentQuery.Criteria.AddCondition("parentsiteorlocation", ConditionOperator.Null);
        var parentResult = await _serviceClient.RetrieveMultipleAsync(parentQuery, ct);
        var parentLib = parentResult.Entities.FirstOrDefault();
        if (parentLib == null) return;

        var checkQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        checkQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, folderName);
        checkQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, entityGuid);
        var existing = (await _serviceClient.RetrieveMultipleAsync(checkQuery, ct)).Entities.FirstOrDefault();
        if (existing != null) return;

        var location = new SharePointDocumentLocation
        {
            RegardingObjectId = new EntityReference(entityLogicalName!, entityGuid),
            ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, parentLib.Id),
            RelativeUrl = folderName,
            Name = name
        };
        await _serviceClient.CreateAsync(location, ct);
    }

    public async Task<IList<FormDocumentField>> GetFormDocumentFieldsAsync(string formId, CancellationToken ct = default)
    {
        var query = new QueryExpression("adoxio_formelementuploadfield") { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_formguid", ConditionOperator.Equal, formId);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities
            .Select(e => new FormDocumentField(
                e.GetAttributeValue<string>("adoxio_fileprefix"),
                e.GetAttributeValue<string>("adoxio_name"),
                e.GetAttributeValue<string>("adoxio_routerlink")))
            .ToList();
    }

    // -------------------------------------------------------------------------
    // Federal Report Export
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_federalreportexport>> GetPendingFederalReportExportsAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_federalreportexport.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_exportcompleted", ConditionOperator.Null);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_federalreportexport>()).ToList();
    }

    public async Task UpdateFederalReportExportAsync(adoxio_federalreportexport export, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(export, ct);

    // -------------------------------------------------------------------------
    // Cannabis Monthly Report
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_cannabismonthlyreport>> GetSubmittedCannabisMonthlyReportsAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_cannabismonthlyreport.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)adoxio_cannabismonthlyreport_statuscode.Submitted);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_cannabismonthlyreport>()).ToList();
    }

    public async Task UpdateCannabisMonthlyReportAsync(adoxio_cannabismonthlyreport report, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(report, ct);

    // -------------------------------------------------------------------------
    // Cannabis Inventory Report
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_cannabisinventoryreport>> GetInventoryReportsByMonthlyReportIdAsync(string monthlyReportId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(monthlyReportId, out var guid)) return new List<adoxio_cannabisinventoryreport>();
        var query = new QueryExpression(adoxio_cannabisinventoryreport.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_monthlyreportid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_cannabisinventoryreport>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Cannabis Product Admin
    // -------------------------------------------------------------------------
    public async Task<string?> GetCannabisProductAdminNameByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync("adoxio_cannabisproductadmin", guid, new ColumnSet("adoxio_name"), ct);
            return entity?.GetAttributeValue<string>("adoxio_name");
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    // -------------------------------------------------------------------------
    // Application Type (adoxio_applicationtype)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_applicationtype>> GetApplicationTypesAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_applicationtype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtype>()).ToList();
    }

    public async Task<adoxio_applicationtype?> GetApplicationTypeByNameAsync(string name, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_applicationtype.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("adoxio_name", ConditionOperator.Equal, name);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_applicationtype>();
    }

    public async Task<adoxio_applicationtype?> GetApplicationTypeByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        var result = await _serviceClient.RetrieveAsync(adoxio_applicationtype.EntityLogicalName, guid, new ColumnSet(true), ct);
        return result?.ToEntity<adoxio_applicationtype>();
    }

    public async Task<IList<adoxio_applicationtype>> GetApplicationTypesByIdsAsync(IList<string> ids, CancellationToken ct = default)
    {
        if (ids.Count == 0) return new List<adoxio_applicationtype>();
        var query = new QueryExpression(adoxio_applicationtype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var filter = new FilterExpression(LogicalOperator.Or);
        foreach (var id in ids)
            if (Guid.TryParse(id, out var guid))
                filter.AddCondition("adoxio_applicationtypeid", ConditionOperator.Equal, guid);
        query.Criteria.AddFilter(filter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtype>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Proposed LRS Applications
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_application>> GetApplicationsByLginAsync(
        string lginId, IList<int> includeStatuses, int? lgDecision = null, CancellationToken ct = default)
    {
        if (!Guid.TryParse(lginId, out var lginGuid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.TopCount = 50;
        query.Criteria.AddCondition("adoxio_localgovindigenousnationid", ConditionOperator.Equal, lginGuid);
        if (includeStatuses.Count > 0)
        {
            var statusGroup = new FilterExpression(LogicalOperator.Or);
            foreach (var s in includeStatuses)
                statusGroup.AddCondition("statuscode", ConditionOperator.Equal, s);
            query.Criteria.AddFilter(statusGroup);
        }
        if (lgDecision.HasValue)
            query.Criteria.AddCondition("adoxio_lgapprovaldecision", ConditionOperator.Equal, lgDecision.Value);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<(IList<adoxio_application> Results, int TotalCount)> GetApplicationsByLginPagedAsync(
        string lginId,
        IList<int> includeStatuses,
        int? lgDecision = null,
        bool? hasDecisionDate = null,
        IList<string>? includeTypeIds = null,
        IList<string>? excludeTypeIds = null,
        IList<int>? excludeStatuses = null,
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(lginId, out var lginGuid))
            return (new List<adoxio_application>(), 0);

        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_localgovindigenousnationid", ConditionOperator.Equal, lginGuid);

        if (includeStatuses.Count > 0)
        {
            var statusGroup = new FilterExpression(LogicalOperator.Or);
            foreach (var s in includeStatuses)
                statusGroup.AddCondition("statuscode", ConditionOperator.Equal, s);
            query.Criteria.AddFilter(statusGroup);
        }
        if (lgDecision.HasValue)
            query.Criteria.AddCondition("adoxio_lgapprovaldecision", ConditionOperator.Equal, lgDecision.Value);
        if (hasDecisionDate == true)
            query.Criteria.AddCondition("adoxio_lgdecisionsubmissiondate", ConditionOperator.NotNull);
        else if (hasDecisionDate == false)
            query.Criteria.AddCondition("adoxio_lgdecisionsubmissiondate", ConditionOperator.Null);
        if (excludeStatuses != null)
            foreach (var s in excludeStatuses)
                query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, s);

        if (includeTypeIds != null && includeTypeIds.Count > 0)
        {
            var typeGroup = new FilterExpression(LogicalOperator.Or);
            foreach (var tid in includeTypeIds)
                if (Guid.TryParse(tid, out var tGuid))
                    typeGroup.AddCondition("adoxio_applicationtypeid", ConditionOperator.Equal, tGuid);
            query.Criteria.AddFilter(typeGroup);
        }
        if (excludeTypeIds != null)
            foreach (var tid in excludeTypeIds)
                if (Guid.TryParse(tid, out var tGuid))
                    query.Criteria.AddCondition("adoxio_applicationtypeid", ConditionOperator.NotEqual, tGuid);

        // fetch page 1 with total count
        query.PageInfo = new PagingInfo { Count = pageSize, PageNumber = 1, ReturnTotalRecordCount = true };
        string? pagingCookie = null;
        int totalCount = 0;
        var pageEntities = new List<adoxio_application>();

        for (int page = 0; page <= pageIndex; page++)
        {
            if (page > 0)
            {
                query.PageInfo.PageNumber = page + 1;
                query.PageInfo.PagingCookie = pagingCookie;
            }
            var res = await _serviceClient.RetrieveMultipleAsync(query, ct);
            if (page == 0) totalCount = res.TotalRecordCount;
            pagingCookie = res.PagingCookie;
            if (page == pageIndex)
                pageEntities = res.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
        }

        return (pageEntities, totalCount);
    }

    public async Task<IList<adoxio_application>> GetApplicationsByJobNumberContainsAsync(
        string jobNumber, IList<int> excludeStatuses, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_jobnumber", ConditionOperator.Like, $"%{jobNumber}%");
        foreach (var s in excludeStatuses)
            query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, s);
        query.AddOrder("adoxio_jobnumber", OrderType.Ascending);
        var lnk = query.AddLink("adoxio_licences", "adoxio_assignedlicence", "adoxio_licencesid", JoinOperator.LeftOuter);
        lnk.Columns = new ColumnSet("adoxio_licencenumber", "adoxio_expirydate");
        lnk.EntityAlias = "lic";
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<IList<adoxio_applicationtype>> GetApplicationTypesByFilterAsync(
        bool? isShowLginApproval = null, bool? isLgZoningConfirmation = null, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_applicationtype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        if (isShowLginApproval.HasValue)
            query.Criteria.AddCondition("adoxio_isshowlginapproval", ConditionOperator.Equal, isShowLginApproval.Value);
        if (isLgZoningConfirmation.HasValue)
            query.Criteria.AddCondition("adoxio_islgzoningconfirmation", ConditionOperator.Equal, isLgZoningConfirmation.Value);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtype>()).ToList();
    }

    public async Task<IList<adoxio_application>> GetProposedLrsApplicationsAsync(string applicationTypeId, IList<int> excludeStatuses, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationTypeId, out var typeGuid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_checklistpsalettersent", ConditionOperator.Equal, 845280000); // Yes
        query.Criteria.AddCondition("adoxio_applicationtypeid", ConditionOperator.Equal, typeGuid);
        foreach (var status in excludeStatuses)
            query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, status);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    // -------------------------------------------------------------------------
    // System Form (systemform — standard Dataverse entity)
    // -------------------------------------------------------------------------
    public async Task<string?> GetSystemFormXmlByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync("systemform", guid, new ColumnSet("formxml"), ct);
            return entity?.GetAttributeValue<string>("formxml");
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    // -------------------------------------------------------------------------
    // Police Jurisdiction (adoxio_policejurisdiction)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_policejurisdiction>> GetPoliceJurisdictionsAsync(string? nameContains = null, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_policejurisdiction.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        if (!string.IsNullOrEmpty(nameContains))
            query.Criteria.AddCondition("adoxio_name", ConditionOperator.Like, $"%{nameContains}%");
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_policejurisdiction>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Establishment Watch Word (adoxio_establishmentwatchword)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_establishmentwatchword>> GetEstablishmentWatchWordsAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_establishmentwatchword.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_establishmentwatchword>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Application picklist metadata (for form rendering)
    // -------------------------------------------------------------------------
    public async Task<IList<DynamicsPicklistAttributeMetadata>> GetApplicationPicklistsAsync(string entityName = "adoxio_application", CancellationToken ct = default)
    {
        var request = new Microsoft.Xrm.Sdk.Messages.RetrieveEntityRequest
        {
            LogicalName = entityName,
            EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes,
            RetrieveAsIfPublished = true
        };
        var response = (Microsoft.Xrm.Sdk.Messages.RetrieveEntityResponse)await _serviceClient.ExecuteAsync(request, ct);
        return response.EntityMetadata.Attributes
            .OfType<Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata>()
            .Select(a => new DynamicsPicklistAttributeMetadata
            {
                LogicalName = a.LogicalName,
                MetadataId = a.MetadataId?.ToString(),
                OptionSet = a.OptionSet != null ? new DynamicsOptionSet
                {
                    Options = a.OptionSet.Options.Select(o => new DynamicsOption
                    {
                        Value = o.Value,
                        Label = new DynamicsLocalizedLabel
                        {
                            UserLocalizedLabel = new DynamicsLabel { Label = o.Label?.UserLocalizedLabel?.Label ?? "" }
                        }
                    }).ToList()
                } : null,
                GlobalOptionSet = a.OptionSet?.IsGlobal == true ? new DynamicsOptionSet
                {
                    Options = a.OptionSet.Options.Select(o => new DynamicsOption
                    {
                        Value = o.Value,
                        Label = new DynamicsLocalizedLabel
                        {
                            UserLocalizedLabel = new DynamicsLabel { Label = o.Label?.UserLocalizedLabel?.Label ?? "" }
                        }
                    }).ToList()
                } : null
            })
            .ToList<DynamicsPicklistAttributeMetadata>();
    }

    // -------------------------------------------------------------------------
    // LDB Order (adoxio_ldborder)
    // -------------------------------------------------------------------------
    public async Task<Guid> CreateLdbOrderAsync(adoxio_ldborder order, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(order, ct);

    // -------------------------------------------------------------------------
    // OneStop Message Item (adoxio_onestopmessageitem)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_onestopmessageitem>> GetPendingOneStopMessagesAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_onestopmessageitem.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_messagesendstatus", ConditionOperator.Equal, (int)OneStopMessageStatus.ReadyToSend);
        query.AddOrder("createdon", OrderType.Ascending);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_onestopmessageitem>()).ToList();
    }

    public async Task<IList<adoxio_onestopmessageitem>> GetOneStopMessagesByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var licenceGuid)) return new List<adoxio_onestopmessageitem>();
        var query = new QueryExpression(adoxio_onestopmessageitem.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_dateacknowledgementreceived", ConditionOperator.Null);
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, licenceGuid);
        query.AddOrder("createdon", OrderType.Ascending);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_onestopmessageitem>()).ToList();
    }

    public async Task UpdateOneStopMessageItemAsync(adoxio_onestopmessageitem item, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(item, ct);

    // -------------------------------------------------------------------------
    // Licence Type by ID
    // -------------------------------------------------------------------------
    public async Task<adoxio_licencetype?> GetLicenceTypeByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_licencetype.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_licencetype>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<IList<adoxio_licencetype>> GetAllLicenceTypesAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_licencetype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licencetype>()).ToList();
    }

    // -------------------------------------------------------------------------
    // OrgBook sync
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_licences>> GetActiveLicencesMissingOrgBookCredentialAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_orgbookcredentialresult", ConditionOperator.Null);
        query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task<IList<adoxio_licences>> GetActiveLicencesWithOrgBookCredentialPendingSyncAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_orgbookcredentialresult", ConditionOperator.Equal, (int)adoxio_licences_adoxio_orgbookcredentialresult.Pass);
        query.Criteria.AddCondition("adoxio_orgbookcredentialid", ConditionOperator.Null);
        query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task<IList<Account>> GetAccountsMissingOrgBookLinkAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("adoxio_bcincorporationnumber", "accountid")
        };
        query.Criteria.AddCondition("adoxio_orgbookorganizationlink", ConditionOperator.Null);
        query.Criteria.AddCondition("adoxio_businessregistrationnumber", ConditionOperator.Null);
        query.Criteria.AddCondition("adoxio_bcincorporationnumber", ConditionOperator.NotNull);
        query.Criteria.AddCondition("adoxio_bcincorporationnumber", ConditionOperator.NotEqual, "BC1234567");
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<Account>()).ToList();
    }

    // -------------------------------------------------------------------------
    // New Account helpers
    // -------------------------------------------------------------------------
    public async Task DeleteAccountAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(Account.EntityLogicalName, guid, ct);
    }

    public async Task<Account?> GetAccountByExternalIdAsync(string externalId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(externalId)) return null;
        var sanitized = externalId.Replace("-", "").ToUpperInvariant();
        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        query.Criteria.AddCondition("adoxio_externalid", ConditionOperator.Equal, sanitized);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<Account>();
    }

    public async Task SetContactParentAccountAsync(string contactId, string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(contactId, out var contactGuid) || !Guid.TryParse(accountId, out var accountGuid)) return;
        var patch = new Contact { Id = contactGuid };
        patch.ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountGuid);
        await _serviceClient.UpdateAsync(patch, ct);
    }

    public async Task SetAccountPrimaryContactAsync(string accountId, string contactId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var accountGuid) || !Guid.TryParse(contactId, out var contactGuid)) return;
        var patch = new Account { Id = accountGuid };
        patch.PrimaryContactId = new EntityReference(Contact.EntityLogicalName, contactGuid);
        await _serviceClient.UpdateAsync(patch, ct);
    }

    public async Task CreateAccountSharePointDocLocAsync(string accountId, string folderName, string displayName, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var accountGuid)) return;

        var parentQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        parentQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, "account");
        parentQuery.Criteria.AddCondition("parentsiteorlocation", ConditionOperator.Null);
        var parentResult = await _serviceClient.RetrieveMultipleAsync(parentQuery, ct);
        var parentLib = parentResult.Entities.FirstOrDefault();
        if (parentLib == null) return;

        var checkQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        checkQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, folderName);
        checkQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, accountGuid);
        var existing = (await _serviceClient.RetrieveMultipleAsync(checkQuery, ct)).Entities.FirstOrDefault();
        if (existing != null) return;

        var location = new SharePointDocumentLocation
        {
            RegardingObjectId = new EntityReference(Account.EntityLogicalName, accountGuid),
            ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, parentLib.Id),
            RelativeUrl = folderName,
            Description = "Account Files",
            Name = displayName
        };
        await _serviceClient.CreateAsync(location, ct);
    }

    // -------------------------------------------------------------------------
    // New Contact helpers
    // -------------------------------------------------------------------------
    public async Task<IList<Contact>> GetContactsByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<Contact>();
        var query = new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        query.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<Contact>()).ToList();
    }

    public async Task DeleteContactAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(Contact.EntityLogicalName, guid, ct);
    }

    public async Task UpdateContactBridgeLoginAsync(string contactId, string siteminderGuid, string? accountId, string? siteminderBusinessGuid, CancellationToken ct = default)
    {
        if (!Guid.TryParse(contactId, out var contactGuid)) return;
        var isServiceCard = siteminderBusinessGuid == null;
        var login = new adoxio_login
        {
            adoxio_Type = isServiceCard ? adoxio_logintype.BCServicesCard : adoxio_logintype.BusinessBCeID,
            adoxio_ExternalID = siteminderGuid,
            adoxio_Contact = new EntityReference(Contact.EntityLogicalName, contactGuid)
        };
        if (!string.IsNullOrEmpty(accountId) && Guid.TryParse(accountId, out var accountGuid))
            login.adoxio_RelatedAccount = new EntityReference(Account.EntityLogicalName, accountGuid);
        await _serviceClient.CreateAsync(login, ct);
    }

    public async Task<Contact?> GetContactByLoginAsync(bool isServicesCard, string siteminderId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(siteminderId)) return null;
        var sanitized = siteminderId.Replace("-", "").ToUpperInvariant();
        var loginType = isServicesCard ? adoxio_logintype.BCServicesCard : adoxio_logintype.BusinessBCeID;
        var query = new QueryExpression(adoxio_login.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_type", ConditionOperator.Equal, (int)loginType);
        query.Criteria.AddCondition("adoxio_externalid", ConditionOperator.Equal, sanitized);
        var logins = await _serviceClient.RetrieveMultipleAsync(query, ct);
        foreach (var entity in logins.Entities)
        {
            var login = entity.ToEntity<adoxio_login>();
            if (login.adoxio_Contact?.Id == null) continue;
            var contact = await GetContactByIdAsync(login.adoxio_Contact.Id.ToString(), ct);
            if (contact != null && contact.GetAttributeValue<OptionSetValue>("statecode")?.Value == 0)
                return contact;
        }
        return null;
    }

    public async Task<Contact?> GetContactByDetailsAsync(string? firstname, string? middlename, string? lastname, string? email, CancellationToken ct = default)
    {
        if (firstname == null && lastname == null && email == null) return null;
        var query = new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        if (!string.IsNullOrEmpty(firstname))
            query.Criteria.AddCondition("firstname", ConditionOperator.Equal, firstname.Replace("'", "''"));
        if (!string.IsNullOrEmpty(middlename))
            query.Criteria.AddCondition("middlename", ConditionOperator.Equal, middlename.Replace("'", "''"));
        if (!string.IsNullOrEmpty(lastname))
            query.Criteria.AddCondition("lastname", ConditionOperator.Equal, lastname.Replace("'", "''"));
        if (!string.IsNullOrEmpty(email))
            query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email.Replace("'", "''"));
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Count == 1 ? result.Entities[0].ToEntity<Contact>() : null;
    }

    public async Task<Contact?> GetContactByNameAndBirthdateAsync(string firstName, string lastName, string birthDate, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(birthDate)) return null;
        var query = new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("lastname", ConditionOperator.Equal, lastName);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        if (!DateTimeOffset.TryParse(birthDate, out var dob)) return null;
        return result.Entities
            .Select(e => e.ToEntity<Contact>())
            .FirstOrDefault(c =>
                c.FirstName != null && c.FirstName.Length > 0 &&
                c.FirstName[0].ToString().ToLower() == firstName[0].ToString().ToLower() &&
                c.BirthDate?.Year == dob.Year &&
                c.BirthDate?.Month == dob.Month &&
                c.BirthDate?.Day == dob.Day);
    }

    // -------------------------------------------------------------------------
    // Legal Entity helpers
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_legalentity>> GetLegalEntitiesByParentEntityIdAsync(string parentLegalEntityId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(parentLegalEntityId, out var guid)) return new List<adoxio_legalentity>();
        var query = new QueryExpression(adoxio_legalentity.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_legalentityowned", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_legalentity>()).ToList();
    }

    public async Task<Guid> CreateLegalEntityAsync(adoxio_legalentity entity, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(entity, ct);

    public async Task UpdateLegalEntityAsync(adoxio_legalentity entity, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(entity, ct);

    public async Task<adoxio_legalentity?> GetLegalEntityByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        var results = await GetLegalEntitiesByAccountIdAsync(accountId, ct);
        return results.FirstOrDefault();
    }

    public async Task DeleteLegalEntityAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_legalentity.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Licence delete
    // -------------------------------------------------------------------------
    public async Task DeleteLicenceAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_licences.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Licensee Changelog (adoxio_licenseechangelog)
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_licenseechangelog>> GetLicenseeChangelogsByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_licenseechangelog>();
        var results = new List<adoxio_licenseechangelog>();
        var fields = new[] { "adoxio_parentbusinessaccount", "adoxio_businessaccount", "adoxio_shareholderbusinessaccount" };
        var seen = new HashSet<Guid>();
        foreach (var field in fields)
        {
            var query = new QueryExpression(adoxio_licenseechangelog.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
            query.Criteria.AddCondition(field, ConditionOperator.Equal, guid);
            var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
            foreach (var entity in result.Entities)
            {
                if (seen.Add(entity.Id))
                    results.Add(entity.ToEntity<adoxio_licenseechangelog>());
            }
        }
        return results;
    }

    public async Task<IList<adoxio_licenseechangelog>> GetLicenseeChangelogsByApplicationIdAsync(string applicationId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationId, out var guid)) return new List<adoxio_licenseechangelog>();
        var query = new QueryExpression(adoxio_licenseechangelog.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_application", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licenseechangelog>()).ToList();
    }

    public async Task<IList<string>> GetLicenseeChangelogIdsByAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        var changelogs = await GetLicenseeChangelogsByAccountIdAsync(accountId, ct);
        return changelogs.Select(c => (c.adoxio_licenseechangelogId ?? c.Id).ToString()).ToList();
    }

    public async Task<Guid> CreateLicenseeChangelogAsync(adoxio_licenseechangelog changelog, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(changelog, ct);

    public async Task UpdateLicenseeChangelogAsync(adoxio_licenseechangelog changelog, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(changelog, ct);

    public async Task DeleteLicenseeChangelogAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(adoxio_licenseechangelog.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Invoice
    // -------------------------------------------------------------------------
    public async Task<Invoice?> GetInvoiceByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(Invoice.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<Invoice>();
        }
        catch
        {
            return null;
        }
    }

    public async Task UpdateInvoiceAsync(Invoice invoice, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(invoice, ct);

    // Invoice delete
    // -------------------------------------------------------------------------
    public async Task DeleteInvoiceAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(Invoice.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Generic delete
    // -------------------------------------------------------------------------
    public async Task DeleteByLogicalNameAsync(string logicalName, string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(logicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // SharePoint document location delete
    // -------------------------------------------------------------------------
    public async Task DeleteSharePointDocLocAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return;
        await _serviceClient.DeleteAsync(SharePointDocumentLocation.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Workflow execution
    // -------------------------------------------------------------------------
    public async Task ExecuteWorkflowAsync(string workflowId, string entityId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(workflowId, out var wfGuid) || !Guid.TryParse(entityId, out var entityGuid)) return;
        var request = new Microsoft.Xrm.Sdk.OrganizationRequest("ExecuteWorkflow");
        request.Parameters["EntityId"] = entityGuid;
        request.Parameters["WorkflowId"] = wfGuid;
        await _serviceClient.ExecuteAsync(request, ct);
    }

    // -------------------------------------------------------------------------
    // SPICE sync support
    // -------------------------------------------------------------------------
    public async Task<Contact?> GetContactBySpdJobIdAsync(int spdJobId, CancellationToken ct = default)
    {
        var query = new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("adoxio_spdjobid", ConditionOperator.Equal, spdJobId);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<Contact>();
    }

    public async Task<adoxio_personalhistorysummary?> GetPersonalHistorySummaryByWorkerJobNumberAsync(string jobNumber, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(jobNumber)) return null;
        var query = new QueryExpression(adoxio_personalhistorysummary.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("adoxio_workerjobnumber", ConditionOperator.Equal, jobNumber);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_personalhistorysummary>();
    }

    public async Task<IList<adoxio_alias>> GetAliasesByContactIdAsync(string contactId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(contactId, out var guid)) return new List<adoxio_alias>();
        var query = new QueryExpression(adoxio_alias.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_contactid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_alias>()).ToList();
    }

    public async Task<IList<adoxio_previousaddress>> GetPreviousAddressesByContactIdAsync(string contactId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(contactId, out var guid)) return new List<adoxio_previousaddress>();
        var query = new QueryExpression(adoxio_previousaddress.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_contactid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_previousaddress>()).ToList();
    }

    public async Task<adoxio_application?> GetApplicationByJobNumberAsync(string jobNumber, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(jobNumber)) return null;
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("adoxio_jobnumber", ConditionOperator.Equal, jobNumber);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_application>();
    }

    public async Task<IList<adoxio_leconnection>> GetActiveLeConnectionsByParentAccountIdAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_leconnection>();
        var query = new QueryExpression(adoxio_leconnection.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        query.Criteria.AddCondition("adoxio_parentaccount", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("adoxio_childprofilename", ConditionOperator.NotEqual, guid);
        query.Criteria.AddCondition("adoxio_securityscreeningrequired", ConditionOperator.Equal, true);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_leconnection>()).ToList();
    }

    public async Task<IList<adoxio_worker>> GetWorkersToSendAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_worker.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_consentvalidated", ConditionOperator.Equal, 845280000); // WorkerConsentValidated.Yes
        query.Criteria.AddCondition("adoxio_exporteddate", ConditionOperator.Null);
        query.Criteria.AddCondition("adoxio_paymentreceived", ConditionOperator.Equal, 1); // adoxio_generalyesno.Yes
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_worker>()).ToList();
    }

    public async Task<IList<adoxio_applicationtype>> GetApplicationTypesWithLeSectionAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_applicationtype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_haslesection", ConditionOperator.Equal, true);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtype>()).ToList();
    }

    public async Task<IList<adoxio_application>> GetApplicationsToSendAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_checklistsenttospd", ConditionOperator.Equal, 1); // Yes
        var statusFilter = new FilterExpression(LogicalOperator.Or);
        statusFilter.AddCondition("adoxio_checklistsecurityclearancestatus", ConditionOperator.Equal, 845280000); // NotSent
        statusFilter.AddCondition("adoxio_checklistsecurityclearancestatus", ConditionOperator.Equal, 845280007); // Sending
        query.Criteria.AddFilter(statusFilter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Licence sub-category
    // -------------------------------------------------------------------------
    public async Task<adoxio_licencesubcategory?> GetLicenceSubCategoryByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        var query = new QueryExpression(adoxio_licencesubcategory.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("adoxio_licencesubcategoryid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_licencesubcategory>();
    }

    public async Task<adoxio_licencesubcategory?> GetLicenceSubCategoryByNameAsync(string name, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_licencesubcategory.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        query.Criteria.AddCondition("adoxio_name", ConditionOperator.Equal, name);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_licencesubcategory>();
    }

    // -------------------------------------------------------------------------
    // Application types by licence type
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_applicationtype>> GetApplicationTypesByLicenceTypeIdAsync(string licenceTypeId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceTypeId, out var guid)) return new List<adoxio_applicationtype>();
        var query = new QueryExpression(adoxio_applicationtype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licencetype", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtype>()).ToList();
    }

    public async Task<IList<adoxio_applicationtype>> GetApplicationTypesByLicenceTypeIdsAsync(IList<string> licenceTypeIds, CancellationToken ct = default)
    {
        if (licenceTypeIds.Count == 0) return new List<adoxio_applicationtype>();
        var query = new QueryExpression(adoxio_applicationtype.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var typeFilter = new FilterExpression(LogicalOperator.Or);
        foreach (var id in licenceTypeIds)
            if (Guid.TryParse(id, out var guid))
                typeFilter.AddCondition("adoxio_licencetype", ConditionOperator.Equal, guid);
        query.Criteria.AddFilter(typeFilter);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtype>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Application type contents
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_applicationtypecontent>> GetApplicationTypeContentsByTypeIdAsync(string applicationTypeId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationTypeId, out var guid)) return new List<adoxio_applicationtypecontent>();
        var query = new QueryExpression(adoxio_applicationtypecontent.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicationtype", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_applicationtypecontent>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Endorsement queries
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_endorsement>> GetEndorsementsByLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_endorsement>();
        var query = new QueryExpression(adoxio_endorsement.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, (int)adoxio_endorsement_statuscode.Cancelled);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_endorsement>()).ToList();
    }

    public async Task<IList<adoxio_hoursofservice>> GetHoursOfSaleByEndorsementIdAsync(string endorsementId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(endorsementId, out var guid)) return new List<adoxio_hoursofservice>();
        var query = new QueryExpression(adoxio_hoursofservice.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_endorsement", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_hoursofservice>()).ToList();
    }

    public async Task<IList<adoxio_servicearea>> GetServiceAreasByEndorsementIdAsync(string endorsementId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(endorsementId, out var guid)) return new List<adoxio_servicearea>();
        var query = new QueryExpression(adoxio_servicearea.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_endorsement", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_servicearea>()).ToList();
    }

    public async Task<IList<adoxio_hoursofservice>> GetHoursOfSaleByLicenceIdNoEndorsementAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_hoursofservice>();
        var query = new QueryExpression(adoxio_hoursofservice.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licence", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("adoxio_endorsement", ConditionOperator.Null);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_hoursofservice>()).ToList();
    }

    public async Task<IList<adoxio_servicearea>> GetServiceAreasByLicenceIdNoEndorsementAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_servicearea>();
        var query = new QueryExpression(adoxio_servicearea.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("adoxio_endorsement", ConditionOperator.Null);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_servicearea>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Application queries for licence management
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_application>> GetApplicationsForLicenceByApplicantAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicant", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("adoxio_assignedlicence", ConditionOperator.NotNull);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var excludedStatuses = new FilterExpression(LogicalOperator.And);
        excludedStatuses.AddCondition("statuscode", ConditionOperator.NotEqual, 845280011); // Processed
        excludedStatuses.AddCondition("statuscode", ConditionOperator.NotEqual, 845280009); // Terminated
        excludedStatuses.AddCondition("statuscode", ConditionOperator.NotEqual, 2);          // Cancelled
        excludedStatuses.AddCondition("statuscode", ConditionOperator.NotEqual, 845280004); // Approved
        excludedStatuses.AddCondition("statuscode", ConditionOperator.NotEqual, 845280005); // Refused
        excludedStatuses.AddCondition("statuscode", ConditionOperator.NotEqual, 845280010); // TerminatedAndRefunded
        query.Criteria.AddFilter(excludedStatuses);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<IList<adoxio_application>> GetActiveApplicationsByAssignedLicenceIdAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_assignedlicence", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<IList<adoxio_application>> GetApplicationsByTypeAndAssignedLicenceAsync(string applicationTypeId, string licenceId, IList<int> excludeStatuses, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationTypeId, out var typeGuid)) return new List<adoxio_application>();
        if (!Guid.TryParse(licenceId, out var licGuid)) return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicationtypeid", ConditionOperator.Equal, typeGuid);
        query.Criteria.AddCondition("adoxio_assignedlicence", ConditionOperator.Equal, licGuid);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        if (excludeStatuses?.Count > 0)
            foreach (var status in excludeStatuses)
                query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, status);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }

    public async Task<IList<adoxio_licences>> GetLicencesByThirdPartyOperatorAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_licences>();
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_thirdpartyoperatorid", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    public async Task<IList<adoxio_licences>> GetLicencesByProposedOwnerAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return new List<adoxio_licences>();
        var query = new QueryExpression(adoxio_licences.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_proposedowner", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_licences>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Licence reference clear operations
    // -------------------------------------------------------------------------
    public async Task ClearLicenceProposedOwnerAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return;
        var patch = new adoxio_licences { Id = guid };
        patch.Attributes["adoxio_proposedowner"] = null;
        await _serviceClient.UpdateAsync(patch, ct);
    }

    public async Task ClearLicenceThirdPartyOperatorAsync(string licenceId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var guid)) return;
        var patch = new adoxio_licences { Id = guid };
        patch.Attributes["adoxio_thirdpartyoperatorid"] = null;
        await _serviceClient.UpdateAsync(patch, ct);
    }

    public async Task ClearAccountProposedOperatorAsync(string accountId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var guid)) return;
        var licences = await GetLicencesByThirdPartyOperatorAsync(accountId, ct);
        foreach (var licence in licences)
        {
            await ClearLicenceThirdPartyOperatorAsync(licence.Id.ToString(), ct);
        }
    }

    // -------------------------------------------------------------------------
    // Application — term association
    // -------------------------------------------------------------------------
    public async Task AssociateTermsConditionsToApplicationAsync(string applicationId, string termId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationId, out var appGuid)) return;
        if (!Guid.TryParse(termId, out var termGuid)) return;
        var relationship = new Relationship("adoxio_adoxio_application_adoxio_applicationtermsconditionslimitation_Application");
        var relatedEntities = new EntityReferenceCollection
        {
            new EntityReference(adoxio_applicationtermsconditionslimitation.EntityLogicalName, termGuid)
        };
        await Task.Run(() => _serviceClient.Associate(adoxio_application.EntityLogicalName, appGuid, relationship, relatedEntities), ct);
    }

    // -------------------------------------------------------------------------
    // Licence SharePoint document location
    // -------------------------------------------------------------------------
    public async Task CreateLicenceSharePointDocLocAsync(string licenceId, string folderName, string name, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var licenceGuid)) return;

        var parentQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        parentQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, "adoxio_licences");
        parentQuery.Criteria.AddCondition("parentsiteorlocation", ConditionOperator.Null);
        var parentResult = await _serviceClient.RetrieveMultipleAsync(parentQuery, ct);
        var parentLib = parentResult.Entities.FirstOrDefault();
        if (parentLib == null) return;

        var checkQuery = new QueryExpression(SharePointDocumentLocation.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = 1 };
        checkQuery.Criteria.AddCondition("relativeurl", ConditionOperator.Equal, folderName);
        checkQuery.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, licenceGuid);
        var existing = (await _serviceClient.RetrieveMultipleAsync(checkQuery, ct)).Entities.FirstOrDefault();
        if (existing != null) return;

        var location = new SharePointDocumentLocation
        {
            RegardingObjectId = new EntityReference(adoxio_licences.EntityLogicalName, licenceGuid),
            ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, parentLib.Id),
            RelativeUrl = folderName,
            Description = "Licence Files",
            Name = name
        };
        await _serviceClient.CreateAsync(location, ct);
    }

    // -------------------------------------------------------------------------
    // Pagination
    // -------------------------------------------------------------------------
    public async Task<(IList<T> Results, string? NextPagingCookie)> RetrievePagedAsync<T>(
        QueryExpression query,
        int pageSize = 5000,
        string? pagingCookie = null,
        CancellationToken ct = default) where T : Entity
    {
        query.PageInfo = new PagingInfo
        {
            Count = pageSize,
            PageNumber = 1,
            PagingCookie = pagingCookie,
            ReturnTotalRecordCount = false
        };
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        var items = result.Entities.Select(e => e.ToEntity<T>()).ToList();
        var nextCookie = result.MoreRecords ? result.PagingCookie : null;
        return (items, nextCookie);
    }

    // -------------------------------------------------------------------------
    // Local Government / Indigenous Nation — queries
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_localgovindigenousnation>> GetIndigenousNationsAsync(CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_localgovindigenousnation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_isindigenousnation", ConditionOperator.Equal, true);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_localgovindigenousnation>()).ToList();
    }

    public async Task<IList<adoxio_localgovindigenousnation>> GetLginsAsync(string? nameContains = null, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_localgovindigenousnation.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        if (!string.IsNullOrEmpty(nameContains))
            query.Criteria.AddCondition("adoxio_name", ConditionOperator.Like, $"%{nameContains}%");
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_localgovindigenousnation>()).ToList();
    }

    public async Task<Account?> GetAccountByLginLinkIdAsync(string lginId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(lginId, out var guid)) return null;
        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_lginlinkid", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        query.Criteria.AddCondition("websiteurl", ConditionOperator.NotNull);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<Account>();
    }

    // -------------------------------------------------------------------------
    // Terms/Conditions — single record and preset
    // -------------------------------------------------------------------------
    public async Task<adoxio_applicationtermsconditionslimitation?> GetTermsConditionsByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_applicationtermsconditionslimitation.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_applicationtermsconditionslimitation>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    public async Task<adoxio_termsconditionslimitationspreset?> GetTermsConditionsPresetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try
        {
            var entity = await _serviceClient.RetrieveAsync(adoxio_termsconditionslimitationspreset.EntityLogicalName, guid, new ColumnSet(true), ct);
            return entity?.ToEntity<adoxio_termsconditionslimitationspreset>();
        }
        catch (Exception ex) when (ex.Message.Contains("Does Not Exist"))
        {
            return null;
        }
    }

    // -------------------------------------------------------------------------
    // Account — SEP police representative check
    // -------------------------------------------------------------------------
    public async Task<bool> IsAccountSepPoliceRepresentativeAsync(string accountId, CancellationToken ct = default)
    {
        try
        {
            var account = await GetAccountByIdAsync(accountId, ct);
            return (int?)account?.adoxio_BusinessType == 845280019; // Police
        }
        catch
        {
            return false;
        }
    }

    // -------------------------------------------------------------------------
    // Policy Document
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_policydocument>> GetPolicyDocumentsAsync(string? category = null, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_policydocument.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        if (!string.IsNullOrEmpty(category))
            query.Criteria.AddCondition("adoxio_category", ConditionOperator.Equal, category);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_policydocument>()).ToList();
    }

    public async Task<adoxio_policydocument?> GetPolicyDocumentBySlugAsync(string slug, CancellationToken ct = default)
    {
        var query = new QueryExpression(adoxio_policydocument.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_slug", ConditionOperator.Equal, slug);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_policydocument>();
    }

    // -------------------------------------------------------------------------
    // Marketing List / Lead
    // -------------------------------------------------------------------------
    public async Task<List?> GetMarketingListByNameAsync(string listName, CancellationToken ct = default)
    {
        var query = new QueryExpression(List.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("listname", ConditionOperator.Equal, listName);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<List>();
    }

    public async Task<Lead?> GetLeadByEmailAsync(string email, CancellationToken ct = default)
    {
        var query = new QueryExpression(Lead.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<Lead>();
    }

    public async Task<Guid> CreateLeadAsync(Lead lead, CancellationToken ct = default)
        => await _serviceClient.CreateAsync(lead, ct);

    public async Task AddLeadToMarketingListAsync(string listId, string leadId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(listId, out var listGuid) || !Guid.TryParse(leadId, out var leadGuid)) return;
        try
        {
            var relationship = new Relationship("listlead_association");
            var related = new EntityReferenceCollection { new EntityReference(Lead.EntityLogicalName, leadGuid) };
            await Task.Run(() => _serviceClient.Associate(List.EntityLogicalName, listGuid, relationship, related), ct);
        }
        catch { /* Already a member is acceptable */ }
    }

    // -------------------------------------------------------------------------
    // LE Connection contacts (recursive)
    // -------------------------------------------------------------------------
    public async Task<IList<Contact>> GetLeConnectionContactsAsync(string accountId, IList<string>? memo = null, CancellationToken ct = default)
    {
        memo ??= new List<string>();
        var result = new List<Contact>();
        if (memo.Contains(accountId)) return result;
        memo.Add(accountId);

        if (!Guid.TryParse(accountId, out var guid)) return result;
        var query = new QueryExpression(adoxio_leconnection.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_parentaccount", ConditionOperator.Equal, guid);
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var connections = await _serviceClient.RetrieveMultipleAsync(query, ct);

        foreach (var entity in connections.Entities)
        {
            var conn = entity.ToEntity<adoxio_leconnection>();
            if (conn.adoxio_IsIndividual == true && conn.adoxio_SecurityScreeningRequired == true)
            {
                if (conn.adoxio_ChildProfileName?.LogicalName == Contact.EntityLogicalName)
                {
                    var contact = await GetContactByIdAsync(conn.adoxio_ChildProfileName.Id.ToString(), ct);
                    if (contact != null && result.All(c => c.Id != contact.Id))
                        result.Add(contact);
                }
            }
            else if (conn.adoxio_IsIndividual != true)
            {
                if (conn.adoxio_ChildProfileName?.LogicalName == Account.EntityLogicalName)
                {
                    var childContacts = await GetLeConnectionContactsAsync(conn.adoxio_ChildProfileName.Id.ToString(), memo, ct);
                    foreach (var c in childContacts)
                        if (result.All(x => x.Id != c.Id))
                            result.Add(c);
                }
            }
        }
        return result;
    }

    // -------------------------------------------------------------------------
    // Alias — additional queries
    // -------------------------------------------------------------------------
    public async Task<adoxio_alias?> GetAliasByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try { return await _serviceClient.RetrieveAsync(adoxio_alias.EntityLogicalName, guid, new ColumnSet(true)).ToEntity<adoxio_alias>(, ct); }
        catch { return null; }
    }

    public async Task DeleteAliasAsync(string id, CancellationToken ct = default)
    {
        if (Guid.TryParse(id, out var guid))
            await _serviceClient.DeleteAsync(adoxio_alias.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Annual Volume — additional queries
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_annualvolume>> GetAnnualVolumesByApplicationIdAsync(string applicationId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(applicationId, out var guid)) return new List<adoxio_annualvolume>();
        var query = new QueryExpression(adoxio_annualvolume.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_application", ConditionOperator.Equal, guid);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_annualvolume>()).ToList();
    }

    public async Task DeleteAnnualVolumeAsync(string id, CancellationToken ct = default)
    {
        if (Guid.TryParse(id, out var guid))
            await _serviceClient.DeleteAsync(adoxio_annualvolume.EntityLogicalName, guid, ct);
    }

    // -------------------------------------------------------------------------
    // Previous Address — additional queries
    // -------------------------------------------------------------------------
    public async Task<adoxio_previousaddress?> GetPreviousAddressByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var guid)) return null;
        try { return await _serviceClient.RetrieveAsync(adoxio_previousaddress.EntityLogicalName, guid, new ColumnSet(true)).ToEntity<adoxio_previousaddress>(, ct); }
        catch { return null; }
    }

    // -------------------------------------------------------------------------
    // Cannabis Monthly Report — additional queries
    // -------------------------------------------------------------------------
    public async Task<adoxio_cannabismonthlyreport?> GetCannabisMonthlyReportByIdAsync(string reportId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(reportId, out var guid)) return null;
        try { return await _serviceClient.RetrieveAsync(adoxio_cannabismonthlyreport.EntityLogicalName, guid, new ColumnSet(true)).ToEntity<adoxio_cannabismonthlyreport>(, ct); }
        catch { return null; }
    }

    public async Task<IList<adoxio_cannabismonthlyreport>> GetCannabisMonthlyReportsByLicenceAndLicenceeAsync(string licenceId, string licenceeId, string startDate, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var licGuid) || !Guid.TryParse(licenceeId, out var licenceeGuid)) return new List<adoxio_cannabismonthlyreport>();
        var query = new QueryExpression(adoxio_cannabismonthlyreport.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, licGuid);
        query.Criteria.AddCondition("adoxio_licenseeid", ConditionOperator.Equal, licenceeGuid);
        if (DateTime.TryParse(startDate, out var startDt))
            query.Criteria.AddCondition("createdon", ConditionOperator.GreaterEqual, startDt);
        query.AddOrder("modifiedon", OrderType.Descending);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_cannabismonthlyreport>()).ToList();
    }

    public async Task<adoxio_cannabismonthlyreport?> GetCannabisMonthlyReportByLicenceYearMonthAsync(string licenceId, string year, string month, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceId, out var licGuid)) return null;
        var query = new QueryExpression(adoxio_cannabismonthlyreport.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenceid", ConditionOperator.Equal, licGuid);
        query.Criteria.AddCondition("adoxio_reportingperiodyear", ConditionOperator.Equal, year);
        query.Criteria.AddCondition("adoxio_reportingperiodmonth", ConditionOperator.Equal, month);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.FirstOrDefault()?.ToEntity<adoxio_cannabismonthlyreport>();
    }

    public async Task<IList<adoxio_cannabismonthlyreport>> GetCannabisMonthlyReportsByLicenceeAsync(string licenceeId, string startDate, CancellationToken ct = default)
    {
        if (!Guid.TryParse(licenceeId, out var guid)) return new List<adoxio_cannabismonthlyreport>();
        var query = new QueryExpression(adoxio_cannabismonthlyreport.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_licenseeid", ConditionOperator.Equal, guid);
        if (DateTime.TryParse(startDate, out var startDt))
            query.Criteria.AddCondition("createdon", ConditionOperator.GreaterEqual, startDt);
        query.AddOrder("adoxio_reportingperiodyear", OrderType.Ascending);
        query.AddOrder("adoxio_reportingperiodmonth", OrderType.Ascending);
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_cannabismonthlyreport>()).ToList();
    }

    // -------------------------------------------------------------------------
    // Cannabis Inventory Report — update
    // -------------------------------------------------------------------------
    public async Task UpdateCannabisInventoryReportAsync(adoxio_cannabisinventoryreport report, CancellationToken ct = default)
        => await _serviceClient.UpdateAsync(report, ct);

    // -------------------------------------------------------------------------
    // Application filter by applicant + type + status codes
    // -------------------------------------------------------------------------
    public async Task<IList<adoxio_application>> GetApplicationsByApplicantTypeAndStatusesAsync(string accountId, string applicationTypeId, IList<int> statusCodes, CancellationToken ct = default)
    {
        if (!Guid.TryParse(accountId, out var accountGuid) || !Guid.TryParse(applicationTypeId, out var typeGuid))
            return new List<adoxio_application>();
        var query = new QueryExpression(adoxio_application.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("adoxio_applicant", ConditionOperator.Equal, accountGuid);
        query.Criteria.AddCondition("adoxio_applicationtypeid", ConditionOperator.Equal, typeGuid);
        if (statusCodes?.Count > 0)
        {
            var orFilter = new FilterExpression(LogicalOperator.Or);
            foreach (var code in statusCodes)
                orFilter.AddCondition("statuscode", ConditionOperator.Equal, code);
            query.Criteria.AddFilter(orFilter);
        }
        var result = await _serviceClient.RetrieveMultipleAsync(query, ct);
        return result.Entities.Select(e => e.ToEntity<adoxio_application>()).ToList();
    }
}
