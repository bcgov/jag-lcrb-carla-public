using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Portal.Host.HealthChecks;

internal static class HealthResponseWriter
{
    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        var body = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            entries = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    duration = entry.Value.Duration.ToString(),
                    description = entry.Value.Description,
                    data = entry.Value.Data,
                    tags = entry.Value.Tags,
                }),
        };
        return context.Response.WriteAsJsonAsync(body);
    }
}
