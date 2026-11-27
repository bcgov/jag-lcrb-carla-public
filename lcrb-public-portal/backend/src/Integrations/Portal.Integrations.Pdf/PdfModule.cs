using Microsoft.Extensions.DependencyInjection;

namespace Portal.Integrations.Pdf;

public static class PdfModule
{
    public static IServiceCollection AddPdfIntegration(this IServiceCollection services) => services;
}
