using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.Accounts;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
