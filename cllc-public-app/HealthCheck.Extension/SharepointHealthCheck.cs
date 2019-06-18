using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.HealthCheck.Extension
{
    public class SharepointHealthCheck : IHealthCheck
    {
        private readonly SharePointFileManager _sharepoint;
        public SharepointHealthCheck(SharePointFileManager sharepoint)
        {
            _sharepoint = sharepoint;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
        {
            // Execute health check logic here. This example sets a dummy
            // variable to true.
            bool healthCheckResultHealthy;
            try
            {
                var result = await _sharepoint.GetDocumentLibrary("Account");

                healthCheckResultHealthy = (result != null);
            }
            catch (Exception)
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return await Task.FromResult(
                    HealthCheckResult.Healthy("Sharepoint is healthy."));
            }

            return await Task.FromResult(
                HealthCheckResult.Unhealthy("Sharepoint is unhealthy."));
        }
    }
}
