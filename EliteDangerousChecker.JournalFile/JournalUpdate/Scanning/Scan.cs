using System.Text.Json;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class Scan
{
    public static Task HandleScan(string line)
    {
        var model = JsonSerializer.Deserialize<ScanModel>(line);
        if (model == null)
        {
            Console.WriteLine("Scan event: failed to deserialize.");
            return Task.CompletedTask;
        }

        if (model.BodyId == null)
        {
            Console.WriteLine($"Scan event: missing BodyId for {model.BodyName ?? "(unknown)"}.");
        }

        ScanData? scanData;
        if (model.BodyId != null && ScanCache.TryGet(model.BodyId.Value, out scanData))
        {
            if (scanData!.wasPrinted)
            {
                return Task.CompletedTask;
            }
            scanData.UpdateFromScan(model.ScanType, model.DistanceFromArrivalLS, model.WasDiscovered, model.WasMapped, model.WasFootfalled, model.TerraformState, model.PlanetClass);
        }
        else
        {
            scanData = new ScanData(model.ScanType, model.DistanceFromArrivalLS, model.BodyName, model.WasDiscovered, model.WasMapped, model.WasFootfalled, model.TerraformState, model.PlanetClass);
        }

        if (scanData.IsNotable())
        {
            var scanMessage = ScanFormatter.Format(scanData);

            Console.WriteLine(scanMessage);
            scanData.wasPrinted = true;
        }

        if (model.BodyId != null)
        {
            ScanCache.AddOrUpdate(model.BodyId.Value, scanData);
        }

        return Task.CompletedTask;
    }
}