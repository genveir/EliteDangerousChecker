using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class GetDiscovered
{
    public static async Task<(long solarSystemId, string discovered)[]> Execute(long[] solarSystemIds)
    {
        var querySql = @"
select
    s.Id SolarSystemId,
    es.Name Discovered
from
    SolarSystem s
    left join Body b on b.SolarSystemId = s.Id and b.MainStar = 1
    left join ExplorationStatus es on es.Id = b.Discovered
where
    s.Id in @solarSystemIds";

        using var connection = DbAccess.GetOpenConnection();
        var data = await connection.QueryAsync<(long SolarSystemId, string Discovered)>(querySql, new { solarSystemIds });

        return data.ToArray();
    }
}
