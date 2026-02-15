using EliteDangerousChecker.JournalFile.JournalUpdate;
using EliteDangerousChecker.JournalFile.PublicAbstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EliteDangerousChecker.JournalFile;
public static class JournalFileServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<FilesChangedHandler>();
        services.AddSingleton<SystemChangeTracker>();

        services.AddSingleton<IJournalFolderWatcher, JournalFolderWatcher>();

        services.AddSingleton<ISystemChangeTracker>(svc => svc.GetRequiredService<SystemChangeTracker>());
        services.AddSingleton<ISystemChangeTrackingService>(svc => svc.GetRequiredService<SystemChangeTracker>());
    }

    public static FullScanHandler[] FullScanHandlersToRun()
    {
        return Array.Empty<FullScanHandler>();
    }
}
