using EliteDangerousChecker.Output.Writers;
using Microsoft.Extensions.DependencyInjection;

namespace EliteDangerousChecker.Output;
public static class OutputServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<SystemWriter>();
    }
}
