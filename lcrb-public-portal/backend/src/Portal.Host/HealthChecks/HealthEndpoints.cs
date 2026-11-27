using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Portal.Host.HealthChecks;

internal static class HealthEndpoints
{
    public const string ReadyTag = "ready";

    public static IServiceCollection AddPortalHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), [ReadyTag]);
        return services;
    }

    public static IEndpointRouteBuilder MapPortalHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/healthz", Options(_ => true));
        endpoints.MapHealthChecks("/healthz/live", Options(_ => false));
        endpoints.MapHealthChecks("/healthz/ready", Options(check => check.Tags.Contains(ReadyTag)));
        return endpoints;
    }

    private static HealthCheckOptions Options(Func<HealthCheckRegistration, bool> predicate) => new()
    {
        Predicate = predicate,
        ResponseWriter = HealthResponseWriter.WriteAsync,
    };
}
