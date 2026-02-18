using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class AddToTrip
{
    public static async Task Execute(long solarSystemId)
    {
        using var connection = DbAccess.GetOpenConnection();
        var sql = @"
insert into TripSystem (TripId, SolarSystemId, Sequence)
select 
    Trip.Id, 
    @solarSystemId,
    (select count(1) from TripSystem where TripId = Trip.Id) + 1
from Trip
    where Trip.EndSystemId is null;";
        await connection.ExecuteAsync(sql, new { solarSystemId });
    }
}
