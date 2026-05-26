using Microsoft.Extensions.Configuration;
using Gov.Lclb.Cllb.Interfaces;
using Xunit;

namespace Dataverse.Tests;

public class DataverseClientTests
{
    [Fact]
    public void DataverseClient_BuildsConnectionFromConfig()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DYNAMICS_ODATA_URI"] = "https://testorg.crm.dynamics.com/api/data/v9.2/",
                ["DYNAMICS_AAD_TENANT_ID"] = "test-tenant",
                ["DYNAMICS_APP_REG_CLIENT_ID"] = "test-client-id",
                ["DYNAMICS_APP_REG_CLIENT_KEY"] = "test-secret"
            })
            .Build();

        // Only verifies config is read and parsed — does not require real credentials
        var ex = Record.Exception(() => new DataverseClient(config));
        Assert.True(ex == null || ex is not InvalidOperationException);
    }

    [Theory]
    [InlineData("DYNAMICS_ODATA_URI")]
    [InlineData("DYNAMICS_AAD_TENANT_ID")]
    [InlineData("DYNAMICS_APP_REG_CLIENT_ID")]
    [InlineData("DYNAMICS_APP_REG_CLIENT_KEY")]
    public void DataverseClient_ThrowsWhenConfigKeyMissing(string missingKey)
    {
        var values = new Dictionary<string, string?>
        {
            ["DYNAMICS_ODATA_URI"] = "https://testorg.crm.dynamics.com/api/data/v9.2/",
            ["DYNAMICS_AAD_TENANT_ID"] = "test-tenant",
            ["DYNAMICS_APP_REG_CLIENT_ID"] = "test-client-id",
            ["DYNAMICS_APP_REG_CLIENT_KEY"] = "test-secret"
        };
        values.Remove(missingKey);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        Assert.Throws<InvalidOperationException>(() => new DataverseClient(config));
    }
}
