using EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
using EliteDangerousChecker.JournalFile.Shared;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.FSD;
internal static class FSDJump
{
    public static async Task HandleFSDJump(string journalLine)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<FSDJumpModel>(journalLine);
        if (parsed == null)
        {
            Console.WriteLine("Failed to parse FSDJump journal entry");
            return;
        }

        var systemData = new StarSystemInfoWriter.SolarSystemDataModel(parsed.SystemAddress!.Value, parsed.StarSystem, null);

        Console.WriteLine("JUMP INTO:");
        await StarSystemInfoWriter.WriteSystemInfo([systemData]);
        Console.WriteLine(ScanFormatter.GetHeader());
    }
}
