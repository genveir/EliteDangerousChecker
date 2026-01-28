using EliteDangerousChecker.Database.Update;
using Microsoft.Extensions.DependencyInjection;

namespace EliteDangerousChecker.Database;

public static class ServiceRegistration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IJsonReaderFactory, JsonReaderFactory>();

        return services;
    }
}
