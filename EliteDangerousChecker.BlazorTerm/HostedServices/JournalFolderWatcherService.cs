
using EliteDangerousChecker.JournalFile;

namespace EliteDangerousChecker.BlazorTerm.HostedServices;

public class JournalFolderWatcherService : IHostedService
{
    private readonly JournalFolderWatcher journalFolderWatcher;

    public JournalFolderWatcherService(JournalFolderWatcher journalFolderWatcher)
    {
        this.journalFolderWatcher = journalFolderWatcher;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        journalFolderWatcher.StartWatching(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
