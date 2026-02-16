using EliteDangerousChecker.Database.FromJournal;
using EliteDangerousChecker.Output.Models;

namespace EliteDangerousChecker.JournalFile.NavRouteUpdate;
internal static class NavRouteAccess
{
    const string NavRouteLocation = @"C:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous\NavRoute.json";

    public static async Task<NavData[]> GetRoute()
    {
        using var reader = new StreamReader(new FileStream(NavRouteLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        var json = await reader.ReadToEndAsync();

        var model = System.Text.Json.JsonSerializer.Deserialize<NavRouteModel>(json);

        if (model == null)
        {
            Console.WriteLine($"failed to deserialize nav route json");
            return Array.Empty<NavData>();
        }

        if (model.Route == null)
        {
            return Array.Empty<NavData>();
        }

        foreach (var hop in model.Route)
        {
            if (hop.StarSystem == null)
            {
                Console.WriteLine($"Nav route entry is missing star system name");
                hop.StarSystem = "Unknown";
                continue;
            }

            if (hop.SystemAddress == null)
            {
                Console.WriteLine($"Nav route entry for {hop.StarSystem} is missing system address. Aborting");
                return Array.Empty<NavData>();
            }

            if (hop.StarClass == null)
            {
                Console.WriteLine($"Nav route entry for {hop.StarSystem} is missing star class");
                hop.StarClass = "?";
                continue;
            }
        }

        var solarSystemIds = model.Route?
            .Where(hop => hop.SystemAddress != null)
            .Select(hop => hop.SystemAddress!.Value)
            .ToArray() ?? Array.Empty<long>();

        var discoveryData = await GetDiscovered.Execute(solarSystemIds);

        return model.Route!
            .Where(hop => hop.SystemAddress != null)
            .Select(hop =>
            {
                var databaseData = discoveryData.FirstOrDefault(d => d.solarSystemId == hop.SystemAddress!.Value);

                string discovered;
                if (databaseData == default)
                {
                    discovered = "No";
                }
                else
                {
                    discovered = databaseData.discovered ?? "Yes";
                }

                return new NavData(
                    SystemName: hop.StarSystem!,
                    SolarSystemId: hop.SystemAddress!.Value,
                    StarType: hop.StarClass!,
                    Discovered: discovered);
            })
            .ToArray();
    }
}
