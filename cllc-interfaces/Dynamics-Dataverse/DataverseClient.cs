using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.PowerPlatform.Dataverse.Client;

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
}
