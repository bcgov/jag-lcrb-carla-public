using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.Assistant;

public static class AssistantModule
{
    public static IServiceCollection AddAssistantModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapAssistantEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
