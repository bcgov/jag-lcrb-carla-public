using Microsoft.Extensions.DependencyInjection;

namespace Portal.Integrations.Payments;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsIntegration(this IServiceCollection services) => services;
}
