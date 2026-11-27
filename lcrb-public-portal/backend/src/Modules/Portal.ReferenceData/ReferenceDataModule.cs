using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.ReferenceData;

public static class ReferenceDataModule
{
    public static IServiceCollection AddReferenceDataModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapReferenceDataEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
