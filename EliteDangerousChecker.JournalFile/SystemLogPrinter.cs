using EliteDangerousChecker.JournalFile.JournalUpdate;

namespace EliteDangerousChecker.JournalFile;
public class SystemLogPrinter
{
    const string JournalFolderPath = @"c:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous";

    public static async Task PrintLogForCurrentSystem()
    {
        await Task.Delay(100);

        var journalFiles = Directory.GetFiles(JournalFolderPath, "Journal.*.log")
            .OrderByDescending(f => f)
            .ToArray();

        Stack<string> fileStack = [];

        foreach (var file in journalFiles)
        {
            var found = await FindJumpAndUpdateTime(file);

            fileStack.Push(file);

            if (found)
            {
                break;
            }
        }

        foreach (var file in fileStack)
        {
            using var updater = new JournalUpdater(file);

            await updater.UpdateFromJournal();
        }
    }

    private static async Task<bool> FindJumpAndUpdateTime(string fileName)
    {
        var lines = await File.ReadAllLinesAsync(fileName);
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i];

            if (line.Contains("FSDJump", StringComparison.OrdinalIgnoreCase))
            {
                var timestamp = line.Split('"', StringSplitOptions.RemoveEmptyEntries)[3].Replace("Z", "");

                var dateTime = DateTime.Parse(timestamp).AddMilliseconds(-1);

                await JournalUpdater.UpdateLastUpdateTime(dateTime);

                return true;
            }
        }

        return false;
    }
}
