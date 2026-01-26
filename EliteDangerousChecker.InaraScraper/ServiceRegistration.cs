using EliteDangerousChecker.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace EliteDangerousChecker.InaraScraper;
public static class ServiceRegistration
{
    public static IServiceCollection AddInaraScraper(this IServiceCollection services)
    {
        services.AddSingleton<IInaraClient, InaraClient>();

        return services;
    }
}
