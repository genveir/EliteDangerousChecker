using EliteDangerousChecker.JournalFile.NavRouteUpdate;

namespace EliteDangerousChecker.JournalFile;
internal sealed class FilesChangedHandler : IDisposable
{
    private record CurrentJournalUpdater(string FileName, JournalUpdate.JournalUpdater Updater);

    private CurrentJournalUpdater? currentJournalUpdater;

    public async Task HandleUpdate(string[] changedFiles)
    {
        if (changedFiles == null) return;

        var marketFileChanged = changedFiles.Any(f => f.EndsWith("Market.json", StringComparison.OrdinalIgnoreCase));

        if (marketFileChanged)
        {
            var marketUpdater = new MarketUpdate.MarketUpdater();
            await marketUpdater.UpdateMarket();
        }

        var changedJournalFiles = changedFiles.Where(f => Path.GetFileName(f).StartsWith("Journal.", StringComparison.OrdinalIgnoreCase)
            && f.EndsWith(".log", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .ToArray();

        for (int n = 0; n < changedJournalFiles.Length; n++)
        {
            if (currentJournalUpdater != null)
            {
                if (currentJournalUpdater.FileName != changedJournalFiles[n])
                {
                    currentJournalUpdater.Updater.Dispose();
                    currentJournalUpdater = new(changedJournalFiles[n], new JournalUpdate.JournalUpdater(changedJournalFiles[n]));
                }
            }
            else
            {
                currentJournalUpdater = new(changedJournalFiles[n], new JournalUpdate.JournalUpdater(changedJournalFiles[n]));
            }

            await currentJournalUpdater.Updater.UpdateFromJournal();
        }

        var navRouteFileChanged = changedFiles.Any(f => f.EndsWith("NavRoute.json", StringComparison.OrdinalIgnoreCase));

        if (navRouteFileChanged)
        {
            await NavRouteUpdater.UpdateNavRoute(printRoute: true);
        }
    }

    public void Dispose()
    {
        if (currentJournalUpdater != null)
        {
            currentJournalUpdater.Updater.Dispose();
            currentJournalUpdater = null;
        }
    }
}
