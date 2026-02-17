using EliteDangerousChecker.JournalFile.JournalUpdate.FSD;
using EliteDangerousChecker.JournalFile.JournalUpdate.Market;
using EliteDangerousChecker.JournalFile.JournalUpdate.Planetary;
using EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;
internal sealed class JournalUpdater : IDisposable
{
    const string JournalFilePath = @"C:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous\";
    private static readonly string LastUpdateTimeFile = Path.Combine(JournalFilePath, "lastupdatetime.txt");

    private readonly StreamReader reader;
    private readonly ISystemChangeTracker tracker;
    private readonly string fileName;

    public JournalUpdater(ISystemChangeTracker tracker, string fileName)
    {
        reader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        this.tracker = tracker;
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

                await HandleLine(tracker, entryTime, line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing journal file {fileName}: {ex}");
        }
    }

    internal static async Task HandleLine(ISystemChangeTracker tracker, DateTime entryTime, string line)
    {
        var splitLine = line.Split([',', '"'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var eventType = splitLine[6];

        bool lineWasHandled = true;
        switch (eventType)
        {
            case "MarketSell":
                await MarketSell.HandleMarketSell(line);
                break;
            case "SellOrganicData":
                await SellOrganicData.HandleSellOrganicData(line);
                break;
            case "FSDJump":
                await FSDJump.HandleFSDJump(tracker, line);
                break;
            case "Scan":
                await Scan.HandleScan(tracker, line);
                break;
            case "FSSBodySignals":
                await FssBodySignals.HandleFssBodySignals(line);
                break;
            case "SAASignalsFound":
                await SAASignalsFound.HandleSAASignalsFound(tracker, line);
                break;
            case "SAAScanComplete":
                await SAAScanComplete.HandleSAAScanComplete(tracker, line);
                break;
            case "Disembark":
                await Disembark.HandleDisembark(tracker, line);
                break;
            case "ScanOrganic":
                await ScanOrganic.HandleScanOrganic(tracker, line);
                break;
            case "FSSDiscoveryScan":
                await FSSDiscoveryScan.HandleFSSDiscoveryScan(tracker, line);
                break;
            default:
                lineWasHandled = false;
                break;
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

    internal static async Task UpdateLastUpdateTime(DateTime newTime)
    {
        var text = newTime.ToString("O");
        await File.WriteAllTextAsync(LastUpdateTimeFile, text);
    }

    public void Dispose()
    {
        reader.Dispose();
    }
}
