using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Portal.IntegrationTests.HealthChecks;

public class HealthEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("/healthz", true)]
    [InlineData("/healthz/ready", true)]
    [InlineData("/healthz/live", false)]
    public async Task Healthy_service_returns_200_with_json_report(string path, bool runsSelfCheck)
    {
        var (status, body) = await Get(factory.CreateClient(), path);

        Assert.Equal(HttpStatusCode.OK, status);
        Assert.Equal("Healthy", body.GetProperty("status").GetString());
        Assert.True(TimeSpan.TryParse(body.GetProperty("totalDuration").GetString(), out _));
        var entries = body.GetProperty("entries");
        Assert.Equal(runsSelfCheck, entries.TryGetProperty("self", out var self));
        if (runsSelfCheck)
        {
            Assert.Equal("Healthy", self.GetProperty("status").GetString());
        }
    }

    [Fact]
    public async Task Failing_ready_check_returns_503_while_live_stays_200()
    {
        var client = factory.WithWebHostBuilder(host => host.ConfigureServices(services =>
            services.AddHealthChecks().AddCheck("failing", () => HealthCheckResult.Unhealthy("down"), ["ready"])))
            .CreateClient();

        var (readyStatus, ready) = await Get(client, "/healthz/ready");
        var (allStatus, _) = await Get(client, "/healthz");
        var (liveStatus, _) = await Get(client, "/healthz/live");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, readyStatus);
        Assert.Equal("Unhealthy", ready.GetProperty("status").GetString());
        Assert.Equal("down", ready.GetProperty("entries").GetProperty("failing").GetProperty("description").GetString());
        Assert.Equal(HttpStatusCode.ServiceUnavailable, allStatus);
        Assert.Equal(HttpStatusCode.OK, liveStatus);
    }

    private static async Task<(HttpStatusCode Status, JsonElement Body)> Get(HttpClient client, string path)
    {
        var token = TestContext.Current.CancellationToken;
        var response = await client.GetAsync(path, token);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));
        return (response.StatusCode, document.RootElement.Clone());
    }
}
