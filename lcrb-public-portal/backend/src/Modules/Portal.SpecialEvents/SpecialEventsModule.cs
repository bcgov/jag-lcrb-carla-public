using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.SpecialEvents;

public static class SpecialEventsModule
{
    public static IServiceCollection AddSpecialEventsModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapSpecialEventsEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
