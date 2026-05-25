using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Interfaces;

/// <summary>
/// Wraps Microsoft.PowerPlatform.Dataverse.Client.ServiceClient.
/// Replaces the AutoRest-generated DynamicsClient.
/// Authentication is configured in LCSD-8529.
/// </summary>
public class DataverseClient : IDataverseClient
{
    protected readonly ServiceClient _serviceClient;

    public DataverseClient(IConfiguration configuration)
    {
        // Full auth setup is implemented in LCSD-8529
        throw new NotImplementedException("Complete authentication setup in LCSD-8529");
    }
}
