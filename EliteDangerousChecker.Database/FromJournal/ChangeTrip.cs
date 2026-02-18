using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class ChangeTrip
{
    public static async Task Execute(long stationId)
    {
        using var connection = DbAccess.GetOpenConnection();

        var sql = @"
declare @solarSystemId bigint = (select SolarSystemId from Station where Id = @stationId);

if not exists (select 1 from Trip where StartSystemId = @solarSystemId and EndSystemId is null)
begin

    update 
        Trip
    set 
        Trip.EndSystemId = @solarSystemId
    where
        Trip.EndSystemId is null;

    insert into Trip (StartSystemId) values (@solarSystemId);
end
";
        await connection.ExecuteAsync(sql, new { stationId });
    }
}
