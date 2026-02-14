using EliteDangerousChecker.Database.FromJournal;
using System.Text.Json;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class FssBodySignals
{
    public static async Task HandleFssBodySignals(string line)
    {
        var model = JsonSerializer.Deserialize<FSSBodySignalsModel>(line);
        if (model == null || model.Signals == null)
            return;

        if (model.SystemAddress == null)
        {
            Console.WriteLine($"FSSBodySignals event: missing SystemAddress for {model.BodyName ?? "(unknown)"}.");
            return;
        }

        if (model.BodyId == null)
        {
            Console.WriteLine($"FSSBodySignals event: missing BodyId for {model.BodyName ?? "(unknown)"}.");
            return;
        }

        foreach (var signal in model.Signals)
        {
            if (string.Equals(signal.TypeLocalised, "Biological", StringComparison.OrdinalIgnoreCase))
            {
                if (signal.Count == null)
                {
                    Console.WriteLine($"FSSBodySignals event: missing Count for {model.BodyName ?? "(unknown)"}'s biological signals.");
                    return;
                }

                await UpdateBodySignals.Execute(model.SystemAddress.Value, model.BodyId.Value, model.BodyName, signal.Count.Value);
            }
        }

        return;
    }
}