using Microsoft.Extensions.DependencyInjection;

namespace Portal.Integrations.Documents;

public static class DocumentsModule
{
    public static IServiceCollection AddDocumentsIntegration(this IServiceCollection services) => services;
}
