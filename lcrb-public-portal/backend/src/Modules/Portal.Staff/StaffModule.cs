using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Portal.Staff;

public static class StaffModule
{
    public static IServiceCollection AddStaffModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapStaffEndpoints(this IEndpointRouteBuilder endpoints) => endpoints;
}
