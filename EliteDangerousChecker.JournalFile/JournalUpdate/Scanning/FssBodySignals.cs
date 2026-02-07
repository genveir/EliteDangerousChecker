using System.Text.Json;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class FssBodySignals
{
    public static Task HandleFssBodySignals(string line)
    {
        var model = JsonSerializer.Deserialize<FSSBodySignalsModel>(line);
        if (model == null || model.Signals == null)
            return Task.CompletedTask;

        foreach (var signal in model.Signals)
        {
            if (string.Equals(signal.TypeLocalised, "Biological", StringComparison.OrdinalIgnoreCase))
            {
                if (model.BodyId == null)
                {
                    Console.WriteLine($"FSSBodySignals event: missing BodyId for {model.BodyName ?? "(unknown)"}.");
                    continue;
                }

                var scanData = new ScanData(model.BodyId.Value, model.BodyName, signal.Count ?? -1);

                ScanCache.AddOrUpdate(model.BodyId.Value, scanData);
            }
        }

        return Task.CompletedTask;
    }
}