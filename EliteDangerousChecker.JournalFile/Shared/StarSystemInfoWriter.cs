using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.JournalFile.Shared;
internal static class StarSystemInfoWriter
{
    public static async Task WriteSystemInfo(SolarSystemDataModel[] solarSystems)
    {
        var explorationData = await GetExploreData.GetExplorationDataOn(solarSystems.Select(ss => ss.Id).ToArray());

        foreach (var dbData in explorationData)
        {
            var data = solarSystems.Single(ss => ss.Id == dbData.SolarSystemId);

            if (data.StarClass != null)
            {
                Console.Write($"{data.StarClass.PadRight(4)}");
                if ("OBAFGKM".Contains(data.StarClass))
                {
                    Console.Write("[FUEL] ");
                }
                else
                {
                    Console.Write("       ");
                }
            }

            switch (dbData.Known)
            {
                case true:
                    Console.WriteLine("Known system: " + data.Name);
                    Console.WriteLine($"  - {dbData.BodyCount} bodies: {dbData.EarthLikePlanets}E, {dbData.WaterWorlds}W, {dbData.AmmoniaWorlds}A, {dbData.TerraformablePlanets}T");
                    break;
                default:
                    Console.WriteLine("UNKNOWN SYSTEM: " + data.Name);
                    break;
            }
        }
    }

    public record SolarSystemDataModel(
        long Id,
        string? Name,
        string? StarClass);
}
