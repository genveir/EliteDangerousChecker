using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class GetExploreData
{
    public static async Task<KnownData[]> GetExplorationDataOn(long[] solarSystemIds)
    {
        using var connection = DbAccess.GetOpenConnection();
        var planetData = await connection.QueryAsync<BodyData>(
            @"
select
	SolarSystemId, BodySubTypeId, TerraformingStateId
from
	Body b
where
	SolarSystemId in @SolarSystemIds",
            new { SolarSystemIds = solarSystemIds });

        var solarSystems = await connection.QueryAsync<long>(
            @"
select
    Id
from
    SolarSystem
where
    Id in @SolarSystemIds",
            new { SolarSystemIds = solarSystemIds });

        return solarSystemIds.Select(ssId =>
        {
            var planets = planetData.Where(p => p.SolarSystemId == ssId).ToList();
            var earthLikePlanets = planets.Count(p => p.BodySubTypeId == 32);
            var waterWorlds = planets.Count(p => p.BodySubTypeId == 19);
            var ammoniaWorlds = planets.Count(p => p.BodySubTypeId == 26);
            var terraformablePlanets = planets.Count(p => p.TerraformingStateId == 2 && IsNotANotableSubType(p.BodySubTypeId));

            var known = solarSystems.Any(s => s == ssId);
            var bodyCount = planetData.Where(p => p.SolarSystemId == ssId).Count();

            return new KnownData(
                SolarSystemId: ssId,
                Known: known,
                BodyCount: bodyCount,
                EarthLikePlanets: earthLikePlanets,
                WaterWorlds: waterWorlds,
                AmmoniaWorlds: ammoniaWorlds,
                TerraformablePlanets: terraformablePlanets);
        }).ToArray();
    }

    private static bool IsNotANotableSubType(int bodySubTypeId)
    {
        return bodySubTypeId != 32 && bodySubTypeId != 19 && bodySubTypeId != 26;
    }

    private class BodyData
    {
        public long SolarSystemId { get; set; }
        public int BodySubTypeId { get; set; }
        public int TerraformingStateId { get; set; }
    }
}

public record KnownData(
    long SolarSystemId,
    bool Known,
    int BodyCount,
    int EarthLikePlanets,
    int WaterWorlds,
    int AmmoniaWorlds,
    int TerraformablePlanets);
