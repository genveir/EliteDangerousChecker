using Dapper;
using EliteDangerousChecker.Database.FromJournal.Models;

namespace EliteDangerousChecker.Database.FromJournal;
public static class GetTripScanValues
{
    public static async Task<ScanValues> ExecuteForActiveTrip()
    {
        return await Execute("t.EndSystemId is null");
    }

    public static async Task<ScanValues> ExecuteForCompletedTrips(long tripId)
    {
        return await Execute($"t.Id = {tripId}");
    }

    private static async Task<ScanValues> Execute(string tripClause)
    {
        using var connection = DbAccess.GetOpenConnection();
        var sql = @$"
with UglyJoin as (
	select
		sum(ssv.ScanValue) TripScanValue, 0 TripUncertainty, convert(bigint, 0) TripBioValue
	from
		Trip t
		cross apply DistinctTripSystems(t.Id) dts
		cross apply SystemScanValue(dts.SolarSystemId) ssv
	where
		t.EndSystemId is null
	union all
	select
		0 TripScanValue, sum(sbv.Uncertainty) TripUncertainty, sum(sbv.Value) TripBioValue
	from
		Trip t
		cross apply DistinctTripSystems(t.Id) dts
		cross apply SystemBioValue(dts.SolarSystemId) sbv
	where
		{tripClause}
)
select
	sum(TripScanValue) ScanValue, sum(TripUncertainty) Uncertainty, sum(TripBioValue) BioValue
from
	UglyJoin";
        return await connection.QuerySingleAsync<ScanValues>(sql);
    }
}
