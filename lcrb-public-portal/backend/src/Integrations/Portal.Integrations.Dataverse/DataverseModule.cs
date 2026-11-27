using Microsoft.Extensions.DependencyInjection;

namespace Portal.Integrations.Dataverse;

public static class DataverseModule
{
    public static IServiceCollection AddDataverseIntegration(this IServiceCollection services) => services;
}
