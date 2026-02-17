using Dapper;
using EliteDangerousChecker.Database.FromJournal.Models;

namespace EliteDangerousChecker.Database.FromJournal;
public static class GetSolarSystemNameAndBodyCount
{
    public static async Task<SystemNameAndBodyCount> Execute(long solarSytemId)
    {
        using var connection = DbAccess.GetOpenConnection();
        var sql = @"
select
	nf.Name,
    case when ss.BodyCount = 0 then null else ss.BodyCount end BodyCount
from
	SolarSystem ss
	cross apply GetSectorPrefixName(ss.Id) nf
where
	ss.Id = @solarSystemId";
        var result = await connection.QuerySingleAsync<SystemNameAndBodyCount>(sql, new { solarSystemId = solarSytemId });

        return result;
    }
}