using EliteDangerousChecker.JournalFile;

namespace EliteDangerousChecker.Api;

public class JournalWatcherBackgroundService : BackgroundService
{
    private JournalFolderWatcher? watcher;

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        watcher = new JournalFolderWatcher();
        watcher.StartWatching(cancellationToken);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        watcher?.Dispose();
        base.Dispose();
    }
}