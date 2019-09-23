using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class GeocoderHealthCheck : IHealthCheck
    // reference https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2
    {
        private readonly IConfiguration _configuration;

        public GeocoderHealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
        {
        GeocoderClient geocoder = new GeocoderClient(_configuration);
        // Try and get the Account document library
        bool healthCheckResultHealthy;
            try
            {
                healthCheckResultHealthy = geocoder.TestAuthentication().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Geocoder is healthy."));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Geocoder is unhealthy."));
            }            
        }
    }
}
