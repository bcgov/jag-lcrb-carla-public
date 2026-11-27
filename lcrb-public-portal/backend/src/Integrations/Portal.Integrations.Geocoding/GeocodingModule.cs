using Microsoft.Extensions.DependencyInjection;

namespace Portal.Integrations.Geocoding;

public static class GeocodingModule
{
    public static IServiceCollection AddGeocodingIntegration(this IServiceCollection services) => services;
}
