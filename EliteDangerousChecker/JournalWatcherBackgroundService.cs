using EliteDangerousChecker.JournalFile;

namespace EliteDangerousChecker.Api;

public class JournalWatcherBackgroundService : BackgroundService
{
    private JournalFolderWatcher? watcher;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
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