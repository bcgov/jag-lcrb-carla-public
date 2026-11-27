using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.Partners;

public static class PartnersModule
{
    public static IServiceCollection AddPartnersModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapPartnersEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
