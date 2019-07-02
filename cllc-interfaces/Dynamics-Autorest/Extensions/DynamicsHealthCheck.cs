using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class DynamicsHealthCheck : IHealthCheck
    {
        private readonly IDynamicsClient _dynamics;
        public DynamicsHealthCheck(IDynamicsClient dynamics)
        {
            _dynamics = dynamics;
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
                var result = _dynamics.Accounts.Get(top: 1).Value;
                healthCheckResultHealthy = true;
            }
            catch (Exception)
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return HealthCheckResult.Healthy("Dynamics is healthy.");
            }

            return HealthCheckResult.Unhealthy("Dynamics is unhealthy.");
        }
    }
}
