using EliteDangerousChecker.JournalFile.JournalUpdate.FSD;
using EliteDangerousChecker.JournalFile.JournalUpdate.Market;
using EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;
internal sealed class JournalUpdater : IDisposable
{
    const string JournalFilePath = @"C:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous\";
    private static readonly string LastUpdateTimeFile = Path.Combine(JournalFilePath, "lastupdatetime.txt");

    private readonly StreamReader reader;
    private readonly string fileName;

    public JournalUpdater(string fileName)
    {
        reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        this.fileName = fileName;
    }

    public async Task UpdateFromJournal()
    {
        var lastUpdate = await GetLastUpdateTime();

        try
        {
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

                await HandleLine(entryTime, line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing journal file {fileName}: {ex.Message}");
        }
    }

    private static bool ShipHasBeenDismissed = false;
    private static bool LastHandledLineWasFSSBodySignals = false;
    private static async Task HandleLine(DateTime entryTime, string line)
    {
        var splitLine = line.Split([',', '"'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var eventType = splitLine[6];

        if (LastHandledLineWasFSSBodySignals && eventType is not ("Scan" or "CodexEntry" or "ScanBaryCentre"))
        {
            Console.WriteLine($"Unexpected: FSSBodySignals event not followed by Scan event. Timestamp: {splitLine[3]}, eventType: {eventType}");
        }

        if (eventType == "Liftoff")
        {
            if (splitLine[7] == "PlayerControlled" && splitLine[8] == ":false")
            {
                ShipHasBeenDismissed = true;
            }
        }

        if (eventType == "Touchdown")
        {
            ShipHasBeenDismissed = false;
        }

        bool lineWasHandled = true;

        if (!ShipHasBeenDismissed)
        {
            switch (eventType)
            {
                case "MarketSell":
                    await MarketSell.HandleMarketSell(line);
                    LastHandledLineWasFSSBodySignals = false;
                    break;
                case "FSDJump":
                    ScanCache.Clear();
                    FSDCache.Clear();
                    LastHandledLineWasFSSBodySignals = false;
                    await FSDJump.HandleFSDJump(line);
                    break;
                case "FSDTarget":
                    await FSDTarget.HandleFSDTarget(line);
                    LastHandledLineWasFSSBodySignals = false;
                    break;
                case "Scan":
                    await Scan.HandleScan(line);
                    LastHandledLineWasFSSBodySignals = false;
                    break;
                case "FSSBodySignals":
                    await FssBodySignals.HandleFssBodySignals(line);
                    LastHandledLineWasFSSBodySignals = true;
                    break;
                case "SAASignalsFound":
                    await SAASignalsFound.HandleSAASignalsFound(line);
                    LastHandledLineWasFSSBodySignals = false;
                    break;
                default:
                    lineWasHandled = false;
                    break;
            }
        }

        if (lineWasHandled)
        {
            await UpdateLastUpdateTime(entryTime);
        }
    }

    private static async Task<DateTime> GetLastUpdateTime()
    {
        if (!File.Exists(LastUpdateTimeFile))
            return DateTime.MinValue;

        var text = await File.ReadAllTextAsync(LastUpdateTimeFile);
        if (string.IsNullOrWhiteSpace(text))
            return DateTime.MinValue;

        if (DateTime.TryParse(text, out var dt))
            return dt;

        return DateTime.MinValue;
    }

    private static async Task UpdateLastUpdateTime(DateTime newTime)
    {
        var text = newTime.ToString("O");
        await File.WriteAllTextAsync(LastUpdateTimeFile, text);
    }

    public void Dispose()
    {
        reader.Dispose();
    }
}
