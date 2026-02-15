
using EliteDangerousChecker.JournalFile.PublicAbstractions;

namespace EliteDangerousChecker.BlazorTerm.HostedServices;

public class JournalFolderWatcherService : BackgroundService
{
    private readonly IJournalFolderWatcher journalFolderWatcher;

    public JournalFolderWatcherService(IJournalFolderWatcher journalFolderWatcher)
    {
        this.journalFolderWatcher = journalFolderWatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await journalFolderWatcher.StartWatching(stoppingToken);
    }
}
