using EliteDangerousChecker.JournalFile.JournalUpdate;

namespace EliteDangerousChecker.JournalFile;
public class OfflineTimeUpdater
{
    const string JournalFolderPath = @"c:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous";

    public static async Task PrintLogForCurrentSystem(ISystemChangeTracker tracker)
    {
        var journalFiles = Directory.GetFiles(JournalFolderPath, "Journal.*.log")
            .OrderByDescending(f => f)
            .ToArray();

        Stack<string> fileStack = [];

        foreach (var file in journalFiles)
        {
            var found = await UpdateSinceJumpBeforeLastExit(file);

            fileStack.Push(file);

            if (found)
            {
                break;
            }
        }

        foreach (var file in fileStack)
        {
            using var updater = new JournalUpdater(tracker, file);

            await updater.UpdateFromJournal();
        }
    }

    private static async Task<bool> UpdateSinceJumpBeforeLastExit(string fileName)
    {
        var reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        var lastUpdate = await JournalUpdater.GetLastUpdateTime();

        var lines = (await reader.ReadToEndAsync()).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i];

            var timestamp = line.Split('"', StringSplitOptions.RemoveEmptyEntries)[3].Replace("Z", "");

            var entryTime = DateTime.Parse(timestamp);

            if (entryTime > lastUpdate)
                continue;

            if (line.Contains("FSDJump") || line.Contains("CarrierJump") || line.Contains("Location"))
            {
                var dateTime = entryTime.AddSeconds(-1);

                await JournalUpdater.MoveLastUpdateTimeBackwardsOrKeep(dateTime);

                return true;
            }
        }

        return false;
    }
}
