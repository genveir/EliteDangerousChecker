using EliteDangerousChecker.Database.FromJournal;

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

        var hopModels = model.Route ?? [];

        var explorationData = await GetExploreData.GetExplorationDataOn(hopModels.Select(m => m.SystemAddress!.Value).ToArray());

        Console.WriteLine("NEW NAV ROUTE:");
        foreach (var hop in explorationData)
        {
            var hopModel = hopModels.First(m => m.SystemAddress == hop.SolarSystemId);

            if (hopModel.StarClass != null)
            {
                Console.Write($"{hopModel.StarClass.PadRight(4)}");
                if ("OBAFGKM".Contains(hopModel.StarClass))
                {
                    Console.Write("[FUEL] ");
                }
                else
                {
                    Console.Write("       ");
                }
            }

            switch (hop.Known)
            {
                case true:
                    Console.WriteLine("Known system: " + hopModel.StarSystem);
                    if (hop.EarthLikePlanets > 0 || hop.WaterWorlds > 0 || hop.AmmoniaWorlds > 0 || hop.TerraformablePlanets > 0)
                    {
                        Console.WriteLine($"  - {hop.EarthLikePlanets} Earthlike, {hop.WaterWorlds} Water, {hop.AmmoniaWorlds} Ammonia, {hop.TerraformablePlanets} Terraformable");
                    }
                    break;
                default:
                    Console.WriteLine("UNKNOWN SYSTEM: " + hopModel.StarSystem);
                    break;
            }
        }
    }
}
