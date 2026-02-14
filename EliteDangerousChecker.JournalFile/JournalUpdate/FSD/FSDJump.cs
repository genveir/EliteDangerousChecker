using EliteDangerousChecker.Database.FromJournal;

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

        if (parsed.SystemAddress == null)
        {
            Console.WriteLine("FSDJump journal entry is missing SystemAddress");
            return;
        }

        if (parsed.StarPos == null || parsed.StarPos.Length != 3)
        {
            Console.WriteLine("FSDJump journal entry has invalid StarPos");
            return;
        }

        double x = parsed.StarPos[0];
        double y = parsed.StarPos[1];
        double z = parsed.StarPos[2];

        if (parsed.StarSystem == null)
        {
            Console.WriteLine("FSDJump journal entry is missing StarSystem");
            return;
        }

        var systemExists = await ExistsSolarSystem.Execute(parsed.SystemAddress.Value);
        if (!systemExists)
        {
            await InsertSystemOnJump.Execute(parsed.SystemAddress.Value, x, y, z, parsed.StarSystem);
        }

        SystemChangeTracker.MarkSystemChange(parsed.SystemAddress.Value);
    }
}
