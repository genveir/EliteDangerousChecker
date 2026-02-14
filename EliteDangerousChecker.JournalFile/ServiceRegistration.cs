using EliteDangerousChecker.JournalFile.JournalUpdate;
using Microsoft.Extensions.DependencyInjection;

namespace EliteDangerousChecker.JournalFile;
public static class JournalFileServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<FilesChangedHandler>();
        services.AddSingleton<JournalFolderWatcher>();
        services.AddSingleton<SystemChangeTracker>();
        services.AddSingleton<SystemWriter>();
    }
}
