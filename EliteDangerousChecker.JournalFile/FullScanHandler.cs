using EliteDangerousChecker.JournalFile.JournalUpdate;

namespace EliteDangerousChecker.JournalFile;
public class FullScanHandler
{
    const string JournalFolderPath = @"c:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous";

    private readonly string[] eventTypes;

    public FullScanHandler(string[] eventTypes)
    {
        this.eventTypes = eventTypes;
    }

    public async Task DoFullScan()
    {
        var tracker = new NopTracker();

        var journalFiles = Directory.GetFiles(JournalFolderPath, "Journal.*.log")
            .OrderBy(f => f)
            .ToArray();

        foreach (var file in journalFiles)
        {
            Console.Write($"Scanning file {file} for events: {string.Join(", ", eventTypes)} .. ");

            var reader = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var splitLine = line.Split([',', '"'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var eventType = splitLine[6];
                var entryTime = DateTime.Parse(line.Substring(15, 19));

                if (eventTypes.Contains(eventType))
                {
                    await JournalUpdater.HandleLine(tracker, entryTime, line);
                }
            }

            Console.WriteLine("Done!");
        }
    }

    private class NopTracker : ISystemChangeTracker
    {
        public void MarkBodyChange(int bodyId) { }
        public void MarkGeneralChange() { }
        public void MarkNavRouteChange() { }
        public void MarkSystemChange(long newSystemAddress) { }
    }
}
