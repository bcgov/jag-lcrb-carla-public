using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class SharepointHealthCheck : IHealthCheck
    // reference https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2
    {
        private readonly SharePointFileManager _sharepoint;
        public SharepointHealthCheck(SharePointFileManager sharepoint)
        {
            _sharepoint = sharepoint;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
        {
            // Try and get the Account document library
            bool healthCheckResultHealthy;
            try
            {
                var result = _sharepoint.GetDocumentLibrary("Account").GetAwaiter().GetResult();

                healthCheckResultHealthy = (result != null);
            }
            catch (Exception)
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Sharepoint is healthy."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Sharepoint is unhealthy."));
        }
    }
}
