using Dapper;
using EliteDangerousChecker.Database.FromJournal.Models;

namespace EliteDangerousChecker.Database.FromJournal;
internal static class GetLifeData
{
    public static async Task<Dictionary<int, LifeData[]>> Execute(long solarSystemId)
    {
        using var connection = DbAccess.GetOpenConnection();

        string getLifeDataSql = @"
select 
	bsg.BodyId,
	sg.Localized,
	s.Name,
	s.Value,
	s.Bonus,
	es.Name Scanned
from
	BodySignalGenus bsg
	left join SignalGenus sg on sg.Id = bsg.SignalGenusId
	left join Species s on s.id = bsg.SpeciesId
	left join ExplorationStatus es on es.Id = bsg.Scanned
where
	bsg.SolarSystemId = @solarSystemId";

        var data = await connection.QueryAsync<SystemLifeData>(getLifeDataSql, new { solarSystemId });

        return data.GroupBy(x => x.BodyId).ToDictionary(
            g => g.Key,
            g => g.Select(x => new LifeData(x.Localized, x.Name, x.Value, x.Bonus, x.Scanned)).ToArray());
    }

    private record SystemLifeData(int BodyId, string Localized, string Name, int Value, int Bonus, string Scanned);

    public static async Task<LifeData[]> Execute(long solarSystemId, int bodyId)
    {
        using var connection = DbAccess.GetOpenConnection();

        string getLifeDataSql = @"
select 
	sg.Localized Genus,
	s.Name Species,
	s.Value,
	s.Bonus,
	es.Name Scanned
from
	BodySignalGenus bsg
	left join SignalGenus sg on sg.Id = bsg.SignalGenusId
	left join Species s on s.id = bsg.SpeciesId
	left join ExplorationStatus es on es.Id = bsg.Scanned
where
	bsg.SolarSystemId = @solarSystemId
	and bsg.BodyId = @bodyId";
        var data = await connection.QueryAsync<LifeData>(getLifeDataSql, new { solarSystemId, bodyId });
        return data.ToArray();
    }
}
