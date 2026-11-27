using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.Licences;

public static class LicencesModule
{
    public static IServiceCollection AddLicencesModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapLicencesEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
