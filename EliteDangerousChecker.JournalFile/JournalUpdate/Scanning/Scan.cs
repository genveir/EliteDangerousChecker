using EliteDangerousChecker.Database.FromJournal;
using System.Text.Json;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class Scan
{
    public static async Task HandleScan(ISystemChangeTracker tracker, string line)
    {
        var model = JsonSerializer.Deserialize<ScanModel>(line);
        if (model == null)
        {
            Console.WriteLine("Scan event: failed to deserialize.");
            return;
        }

        if (model.SystemAddress == null)
        {
            Console.WriteLine($"Scan event: missing SystemAddress for {model.BodyName ?? "(unknown)"}. Aborting scan update.");
            return;
        }

        if (model.BodyId == null)
        {
            Console.WriteLine($"Scan event: missing BodyId for {model.BodyName ?? "(unknown)"}. Aborting scan update.");
            return;
        }

        if (model.BodyName == null)
        {
            Console.WriteLine($"Scan event: missing BodyName for body with ID {model.BodyId.Value} in system with address {model.SystemAddress.Value}. Aborting scan update.");
            return;
        }

        if (model.BodyId != null)
        {
            await UpdateBodyFromScan.Execute(model.SystemAddress.Value, model.BodyId.Value, model.BodyName, model.WasDiscovered, model.WasMapped, model.WasFootfalled, model.TerraformState, model.PlanetClass, model.StarType);

            tracker.MarkGeneralChange();
        }
    }
}