using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class SAAScanComplete
{
    public static async Task HandleSAAScanComplete(SystemChangeTracker tracker, string journalLine)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<SAAScanCompleteModel>(journalLine);
        if (parsed == null)
        {
            Console.WriteLine("Failed to parse SAAScanComplete journal entry");
            return;
        }

        if (parsed?.SystemAddress == null)
        {
            Console.WriteLine("SAAScanComplete journal entry is missing SystemAddress");
            return;
        }

        if (parsed.BodyId == null)
        {
            Console.WriteLine("SAAScanComplete journal entry is missing BodyId");
            return;
        }

        await SetBodyMappedByCommander.Execute(parsed.SystemAddress.Value, parsed.BodyId.Value);

        tracker.MarkBodyChange(parsed.BodyId.Value);
    }
}
