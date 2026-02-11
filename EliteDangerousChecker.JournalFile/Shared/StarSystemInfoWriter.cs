using EliteDangerousChecker.Database.FromJournal;
using EliteDangerousChecker.JournalFile.NavRouteUpdate;

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

            if (data.Name == null)
            {
                Console.WriteLine("System with null name, skipping");
                continue;
            }

            var nameMsg = $"{data.Name,-40}";

            double distanceToSol = -1.0d;
            double distanceToEnd = -1.0d;
            if (NavRouteCache.TryGet(data.Name, out var navData))
            {
                if (navData?.StarPos == null || navData.StarPos.Length != 3)
                {
                    Console.WriteLine($"Nav route data for {data.Name} has invalid or null StarPos, skipping distance calculation");
                }
                else
                {
                    distanceToSol = Math.Sqrt(Math.Pow(navData.StarPos[0], 2) + Math.Pow(navData.StarPos[1], 2) + Math.Pow(navData.StarPos[2], 2));

                    var endPos = NavRouteCache.Last?.StarPos;

                    if (endPos != null)
                    {
                        distanceToEnd = Math.Sqrt(Math.Pow(navData.StarPos[0] - endPos[0], 2) + Math.Pow(navData.StarPos[1] - endPos[1], 2) + Math.Pow(navData.StarPos[2] - endPos[2], 2));
                    }
                }
            }

            string distanceToSolMsg = "";
            if (distanceToSol >= 0)
            {
                distanceToSolMsg = $"ToSol: {distanceToSol,-9:N2}";
            }

            string distanceToEndMsg = "";
            if (distanceToEnd >= 0)
            {
                distanceToEndMsg = $"ToEnd: {distanceToEnd,-9:N2}";
            }


            var systemMsg = $"{nameMsg}{distanceToSolMsg} {distanceToEndMsg}";

            switch (dbData.Known)
            {
                case true:
                    Console.WriteLine($"  Known: {systemMsg}");
                    Console.WriteLine($"  - {dbData.BodyCount} bodies: {dbData.EarthLikePlanets}E, {dbData.WaterWorlds}W, {dbData.AmmoniaWorlds}A, {dbData.TerraformablePlanets}T");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"UNKNOWN: ");
                    Console.ResetColor();
                    Console.WriteLine(systemMsg);
                    break;
            }
        }
    }

    public record SolarSystemDataModel(
        long Id,
        string? Name,
        string? StarClass);
}
