using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.LegalEntity;

public static class LegalEntityModule
{
    public static IServiceCollection AddLegalEntityModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapLegalEntityEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
