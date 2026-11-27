using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Portal.Host.HealthChecks;

namespace Portal.UnitTests.HealthChecks;

public class HealthResponseWriterTests
{
    [Fact]
    public async Task Writes_overall_status_and_each_entry_as_json()
    {
        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["dataverse"] = new(HealthStatus.Unhealthy, "timeout", TimeSpan.FromMilliseconds(250), null,
                    new Dictionary<string, object> { ["attempts"] = 3 }, ["ready"]),
                ["self"] = new(HealthStatus.Healthy, null, TimeSpan.Zero, null, null),
            },
            TimeSpan.FromMilliseconds(300));
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await HealthResponseWriter.WriteAsync(context, report);

        Assert.Equal("application/json; charset=utf-8", context.Response.ContentType);
        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
        var body = document.RootElement;
        Assert.Equal("Unhealthy", body.GetProperty("status").GetString());
        Assert.Equal("00:00:00.3000000", body.GetProperty("totalDuration").GetString());
        var dataverse = body.GetProperty("entries").GetProperty("dataverse");
        Assert.Equal("Unhealthy", dataverse.GetProperty("status").GetString());
        Assert.Equal("00:00:00.2500000", dataverse.GetProperty("duration").GetString());
        Assert.Equal("timeout", dataverse.GetProperty("description").GetString());
        Assert.Equal(3, dataverse.GetProperty("data").GetProperty("attempts").GetInt32());
        Assert.Equal("ready", dataverse.GetProperty("tags")[0].GetString());
        Assert.Equal("Healthy", body.GetProperty("entries").GetProperty("self").GetProperty("status").GetString());
    }
}
