using EliteDangerousChecker.JournalFile.JournalUpdate;
using EliteDangerousChecker.JournalFile.PublicAbstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EliteDangerousChecker.JournalFile;
public static class JournalFileServiceRegistration
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<FilesChangedHandler>();
        services.AddSingleton<ISystemChangeTracker, SystemChangeTracker>();

        services.AddSingleton<IJournalFolderWatcher, JournalFolderWatcher>();
        services.AddSingleton<ISystemChangeTrackingService, SystemChangeTracker>();
    }

    public static FullScanHandler[] FullScanHandlersToRun()
    {
        return Array.Empty<FullScanHandler>();
    }
}
