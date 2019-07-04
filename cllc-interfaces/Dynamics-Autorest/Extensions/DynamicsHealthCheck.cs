using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class DynamicsHealthCheck : IHealthCheck
    // follows pattern on https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2
    {
        private readonly IDynamicsClient _dynamics;
        public DynamicsHealthCheck(IDynamicsClient dynamics)
        {
            _dynamics = dynamics;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
        {
            // Execute health check logic here. This example sets a dummy
            // variable to true.
            bool healthCheckResultHealthy;
            try
            {
                var result = _dynamics.Accounts.Get(top: 1).Value;
                healthCheckResultHealthy = true;
            }
            catch (Exception)
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Dynamics is healthy."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Dynamics is unhealthy."));
        }
    }
}
