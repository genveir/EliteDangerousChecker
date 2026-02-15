using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Planetary;
internal static class ScanOrganic
{
    public static async Task HandleScanOrganic(ISystemChangeTracker tracker, string line)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<ScanOrganicModel>(line);

        if (parsed == null)
        {
            Console.WriteLine("Failed to parse ScanOrganic journal entry");
            return;
        }

        if (parsed.SystemAddress == null)
        {
            Console.WriteLine("ScanOrganic journal entry is missing SystemAddress");
            return;
        }

        if (parsed.BodyID == null)
        {
            Console.WriteLine("ScanOrganic journal entry is missing BodyID");
            return;
        }

        if (string.IsNullOrEmpty(parsed.Genus))
        {
            Console.WriteLine("ScanOrganic journal entry is missing Genus");
            return;
        }

        if (string.IsNullOrEmpty(parsed.SpeciesLocalised))
        {
            Console.WriteLine("ScanOrganic journal entry is missing SpeciesLocalised");
            return;
        }

        var finalScan = parsed.ScanType == "Analyse";

        await UpdateBodySignalGenusFromOrganicScan.Execute(parsed.SystemAddress.Value, parsed.BodyID.Value, parsed.Genus, parsed.SpeciesLocalised, parsed.WasLogged, finalScan);

        tracker.MarkBodyChange(parsed.BodyID.Value);
    }
}
