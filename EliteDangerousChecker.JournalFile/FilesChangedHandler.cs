namespace EliteDangerousChecker.JournalFile;
internal class FilesChangedHandler
{
    public static async Task HandleUpdate(List<string> changedFiles)
    {
        var marketFileChanged = changedFiles.Any(f => f.EndsWith("Market.json", StringComparison.OrdinalIgnoreCase));

        if (marketFileChanged)
        {
            var marketUpdater = new MarketUpdate.MarketUpdater();
            await marketUpdater.UpdateMarket();
        }

        var journalFilesChanged = changedFiles.Any(f => Path.GetFileName(f).StartsWith("Journal.", StringComparison.OrdinalIgnoreCase)
            && f.EndsWith(".log", StringComparison.OrdinalIgnoreCase));

        if (journalFilesChanged)
        {
            var journalUpdater = new JournalUpdate.JournalUpdater();
            await journalUpdater.UpdateFromJournal();
        }

        var navRouteFileChanged = changedFiles.Any(f => f.EndsWith("NavRoute.json", StringComparison.OrdinalIgnoreCase));

        if (navRouteFileChanged)
        {
            var navRouteUpdater = new NavRouteUpdate.NavRouteUpdater();
            await navRouteUpdater.UpdateNavRoute();
        }
    }
}
