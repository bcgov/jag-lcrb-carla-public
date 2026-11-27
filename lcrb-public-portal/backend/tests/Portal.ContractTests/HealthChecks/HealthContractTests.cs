using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Portal.ContractTests.HealthChecks;

public class HealthContractTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly string[] ReportFields = ["entries", "status", "totalDuration"];

    private static readonly string[] EntryFields = ["data", "description", "duration", "status", "tags"];

    [Fact]
    public async Task Health_report_keeps_its_field_names()
    {
        var token = TestContext.Current.CancellationToken;
        using var document = JsonDocument.Parse(await factory.CreateClient().GetStringAsync("/healthz", token));
        var report = document.RootElement;

        Assert.Equal(ReportFields, Names(report));
        Assert.Equal(EntryFields, Names(report.GetProperty("entries").GetProperty("self")));
    }

    private static string[] Names(JsonElement element) =>
        [.. element.EnumerateObject().Select(property => property.Name).Order(StringComparer.Ordinal)];
}
