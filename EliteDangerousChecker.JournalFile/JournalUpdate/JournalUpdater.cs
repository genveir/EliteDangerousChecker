using EliteDangerousChecker.Database.DirectQueries;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;
internal class JournalUpdater
{
    const string JournalFilePath = @"C:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous\";

    public async Task UpdateFromJournal()
    {
        var lastUpdate = await GetLastCommoditiesSoldDateTime.Execute();

        var journalFiles = Directory.GetFiles(JournalFilePath, "Journal.*.log")
            .OrderBy(f => f)
            .ToList();

        for (int n = 0; n < journalFiles.Count; n++)
        {
            var currentFile = journalFiles[n];
            var nextFile = n + 1 < journalFiles.Count ? journalFiles[n + 1] : null;

            bool parseFile = true;

            if (nextFile != null)
            {
                var nextDateTimePart = Path.GetFileNameWithoutExtension(journalFiles[n + 1]).Split('.')[1];
                var nextFileDateTime = ParseFilenameDateTime(nextDateTimePart);

                if (nextFileDateTime <= lastUpdate)
                {
                    parseFile = false;
                }
            }

            if (!parseFile)
                continue;

            try
            {
                using var reader = new StreamReader(new FileStream(journalFiles[n], FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var timestampPart = line.Substring(15, 19);
                    var entryTime = DateTime.Parse(timestampPart);
                    if (entryTime <= lastUpdate)
                    {
                        continue;
                    }

                    await HandleLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing journal file {currentFile}: {ex.Message}");
            }
        }
    }

    private async Task HandleLine(string line)
    {
        var splitLine = line.Split([',', '"'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var eventType = splitLine[6];

        switch (eventType)
        {
            case "MarketSell":
                await MarketSell.HandleMarketSell(line);
                break;
        }
    }

    private DateTime ParseFilenameDateTime(string value)
    {
        var parts = value.Split(['-', 'T']);

        int year = int.Parse(parts[0]);
        int month = int.Parse(parts[1]);
        int day = int.Parse(parts[2]);

        int hour = int.Parse(parts[3].Substring(0, 2));
        int minute = int.Parse(parts[3].Substring(2, 2));
        int second = int.Parse(parts[3].Substring(4, 2));

        return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
    }
}
