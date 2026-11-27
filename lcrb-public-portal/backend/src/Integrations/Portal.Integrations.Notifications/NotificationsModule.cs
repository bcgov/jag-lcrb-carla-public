using Microsoft.Extensions.DependencyInjection;

namespace Portal.Integrations.Notifications;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsIntegration(this IServiceCollection services) => services;
}
