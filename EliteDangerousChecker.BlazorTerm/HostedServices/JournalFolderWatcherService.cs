
using EliteDangerousChecker.JournalFile;

namespace EliteDangerousChecker.BlazorTerm.HostedServices;

public class JournalFolderWatcherService : BackgroundService
{
    private readonly JournalFolderWatcher journalFolderWatcher;

    public JournalFolderWatcherService(JournalFolderWatcher journalFolderWatcher)
    {
        this.journalFolderWatcher = journalFolderWatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await journalFolderWatcher.StartWatching(stoppingToken);
    }
}
