using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.Applications;

public static class ApplicationsModule
{
    public static IServiceCollection AddApplicationsModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapApplicationsEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
