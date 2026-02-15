using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class GetBodyData
{
    public static async Task<BodyData[]> Execute(long solarSystemId)
    {
        var querySql = @"
with signals as (
	select
		bsg.BodyId,
		bsg.Number
	from
		BodySignalType bsg
		join SignalType st on st.Id = bsg.SignalTypeId
	where
		bsg.SolarSystemId = @solarSystemId
		and st.Name = '$SAA_SignalType_Biological;'
)
select
	b.BodyId,
	b.Name,
	esD.Name Discovered,
	esM.Name Mapped,
	esL.Name Landed,
	ts.Name TerraformingState,
	bt.Name BodyType,
	bst.Name SubType,
	isnull(sig.Number, 0) BioSignals
from
	Body b
	left join ExplorationStatus esD on esD.Id = b.Discovered
	left join ExplorationStatus esM on esM.Id = b.Mapped
	left join ExplorationStatus esL on esL.Id = b.Landed
	left join TerraformingState ts on ts.Id = b.TerraformingStateId
	left join BodyType bt on bt.Id = b.BodyTypeId
	left join BodySubType bst on bst.Id = b.BodySubTypeId
	left join signals sig on sig.BodyId = b.BodyId
where
	b.SolarSystemId = @solarSystemId";

        using var connection = DbAccess.GetOpenConnection();

        var bodyData = await connection.QueryAsync<BodyData>(querySql, new { solarSystemId });

        return bodyData.ToArray();
    }

    public static async Task<BodyData?> Execute(long solarSystemId, int bodyId)
    {
        var querySql = @"
select
	b.BodyId,
	b.Name,
	esD.Name Discovered,
	esM.Name Mapped,
	esL.Name Landed,
	ts.Name TerraformingState,
	bt.Name BodyType,
	bst.Name SubType,
	isnull(sig.Number, 0) BioSignals
from
	Body b
	left join ExplorationStatus esD on esD.Id = b.Discovered
	left join ExplorationStatus esM on esM.Id = b.Mapped
	left join ExplorationStatus esL on esL.Id = b.Landed
	left join TerraformingState ts on ts.Id = b.TerraformingStateId
	left join BodyType bt on bt.Id = b.BodyTypeId
	left join BodySubType bst on bst.Id = b.BodySubTypeId
	left join BodySignalType sig on sig.SolarSystemId = b.SolarSystemId and sig.BodyId = b.BodyId
	left join SignalType st on st.Id = sig.SignalTypeId
where
	b.SolarSystemId = @solarSystemId
	and b.BodyId = @bodyId
	and st.Name = '$SAA_SignalType_Biological;'";

        using var connection = DbAccess.GetOpenConnection();

        var bodyData = await connection.QueryAsync<BodyData>(querySql, new { solarSystemId, bodyId });

        return bodyData.SingleOrDefault();
    }

    public record BodyData(int BodyId, string Name, string Discovered, string Mapped, string Landed, string TerraformingState, string BodyType, string SubType, int BioSignals)
    {
        public LifeData[] LifeData { get; set; } = Array.Empty<LifeData>();
    }

    public record LifeData(string Genus, string Species, int Value, bool Scanned, bool First);
}
