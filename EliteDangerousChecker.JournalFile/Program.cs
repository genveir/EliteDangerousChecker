using EliteDangerousChecker.JournalFile.JournalUpdate;
using EliteDangerousChecker.JournalFile.NavRouteUpdate;

namespace EliteDangerousChecker.JournalFile;
public static class Program
{
    public static async Task Main(string[] args)
    {
        var tokenSource = new CancellationTokenSource();

        var cancellationToken = tokenSource.Token;

        try
        {
            Task.Run(() => SystemChangeTracker.StartOutputLoop(), cancellationToken);

            await NavRouteUpdater.UpdateNavRoute(printRoute: false);
            await SystemLogPrinter.PrintLogForCurrentSystem();

            var watcher = new JournalFolderWatcher();
            await watcher.StartWatching(cancellationToken);
        }
        finally
        {
            tokenSource.Cancel();
        }
    }
}
