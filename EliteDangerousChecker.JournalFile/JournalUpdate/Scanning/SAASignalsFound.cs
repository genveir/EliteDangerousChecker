using EliteDangerousChecker.Database.FromJournal;
using EliteDangerousChecker.Database.FromJournal.Models;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class SAASignalsFound
{
    public static async Task HandleSAASignalsFound(ISystemChangeTracker tracker, string journalLine)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<SAASignalsFoundModel>(journalLine);
        if (parsed == null)
        {
            Console.WriteLine("Failed to parse SAASignalsFound journal entry");
            return;
        }

        if (parsed?.SystemAddress == null)
        {
            Console.WriteLine("SAASignalsFound journal entry is missing SystemAddress");
            return;
        }

        if (parsed.BodyId == null)
        {
            Console.WriteLine("SAASignalsFound journal entry is missing BodyId");
            return;
        }

        if (parsed.Genuses != null)
        {
            List<GenusWithLocalization> localizedGenuses = [];
            foreach (var genus in parsed.Genuses)
            {
                localizedGenuses.Add(new GenusWithLocalization(genus.GenusName ?? "", genus.GenusLocalised ?? ""));
            }

            await UpdateBodySignalGenus.Execute(parsed.SystemAddress.Value, parsed.BodyId.Value, parsed.BodyName, localizedGenuses.ToArray());

            tracker.MarkBodyChange(parsed.BodyId.Value);
        }
    }
}
