using EliteDangerousChecker.Database.FromJournal;
using System.Text.Json;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class FSSDiscoveryScan
{
    public static async Task HandleFSSDiscoveryScan(ISystemChangeTracker tracker, string line)
    {
        var parsed = JsonSerializer.Deserialize<FSSDiscoveryScanModel>(line);

        if (parsed == null)
        {
            Console.WriteLine($"Failed to parse FSSDiscoveryScan: {line}");
            return;
        }

        if (parsed.SystemAddress == null)
        {
            Console.WriteLine($"FSSDiscoveryScan missing SystemAddress: {line}");
            return;
        }

        if (parsed.BodyCount == null)
        {
            Console.WriteLine($"FSSDiscoveryScan missing BodyCount: {line}");
            return;
        }

        await UpdateSolarSystemBodyCount.Execute(parsed.SystemAddress.Value, parsed.BodyCount.Value);

        tracker.MarkTotalBodyCountChange();
    }
}
