using EliteDangerousChecker.JournalFile;
using EliteDangerousChecker.JournalFile.NavRouteUpdate;

namespace EliteDangerousChecker.Api;

public class JournalWatcherBackgroundService : BackgroundService
{
    private JournalFolderWatcher? watcher;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await NavRouteUpdater.UpdateNavRoute(printRoute: false);
        await SystemLogPrinter.PrintLogForCurrentSystem();

        watcher = new JournalFolderWatcher();
        await watcher.StartWatching(cancellationToken);
    }

    public override void Dispose()
    {
        watcher?.Dispose();
        base.Dispose();
    }
}