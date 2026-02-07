using EliteDangerousChecker.JournalFile.Shared;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.FSD;
internal static class FSDTarget
{
    public static async Task HandleFSDTarget(string journalLine)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<FSDTargetModel>(journalLine);
        if (parsed == null)
        {
            Console.WriteLine("Failed to parse FSDTarget journal entry");
            return;
        }

        if (FSDCache.Get(parsed.SystemAddress) != null)
        {
            return;
        }

        var systemData = new StarSystemInfoWriter.SolarSystemDataModel(parsed.SystemAddress!.Value, parsed.Name, parsed.StarClass);

        Console.WriteLine("NEW FSD TARGET:");
        await StarSystemInfoWriter.WriteSystemInfo([systemData]);

        FSDCache.AddOrUpdate(parsed);
    }
}
