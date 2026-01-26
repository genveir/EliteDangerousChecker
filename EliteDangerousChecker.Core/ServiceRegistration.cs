using EliteDangerousChecker.Core.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace EliteDangerousChecker.Core;
public static class ServiceRegistration
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddSingleton<IGetPricePoints, GetPricePoints>();

        return services;
    }
}
