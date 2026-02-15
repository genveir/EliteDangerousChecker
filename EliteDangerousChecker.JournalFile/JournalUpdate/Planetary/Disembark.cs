using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Planetary;
internal static class Disembark
{
    public static async Task HandleDisembark(ISystemChangeTracker tracker, string journalLine)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<DisembarkModel>(journalLine);
        if (parsed == null)
        {
            Console.WriteLine("Failed to parse Disembark journal entry");
            return;
        }

        if (parsed.OnPlanet != true)
        {
            return;
        }

        if (parsed.SRV == true)
        {
            return;
        }


        if (parsed.SystemAddress == null)
        {
            Console.WriteLine("Disembark journal entry is missing SystemAddress");
            return;
        }

        if (parsed.BodyId == null)
        {
            Console.WriteLine("Disembark journal entry is missing BodyId");
            return;
        }

        await SetBodyLandedByCommander.Execute(parsed.SystemAddress.Value, parsed.BodyId.Value);

        tracker.MarkBodyChange(parsed.BodyId.Value);
    }
}
