using EliteDangerousChecker.JournalFile.Shared;

namespace EliteDangerousChecker.JournalFile.NavRouteUpdate;
internal class NavRouteUpdater
{
    const string NavRouteLocation = @"C:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous\NavRoute.json";

    public async Task UpdateNavRoute()
    {
        using var reader = new StreamReader(new FileStream(NavRouteLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        var json = await reader.ReadToEndAsync();

        var model = System.Text.Json.JsonSerializer.Deserialize<NavRouteModel>(json);

        if (model == null)
        {
            Console.WriteLine($"failed to deserialize nav route json");
            return;
        }

        var systemData = model.Route!.Select(hop => new StarSystemInfoWriter.SolarSystemDataModel(hop.SystemAddress!.Value, hop.StarSystem, hop.StarClass)).ToArray();

        Console.WriteLine("NEW NAV ROUTE:");
        await StarSystemInfoWriter.WriteSystemInfo(systemData);
    }
}
