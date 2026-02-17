using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateSolarSystemBodyCount
{
    public static async Task Execute(long solarSystemId, int bodyCount)
    {
        using var connection = DbAccess.GetOpenConnection();
        var sql = @"
update SolarSystem
set BodyCount = @bodyCount
where Id = @solarSystemId";
        await connection.ExecuteAsync(sql, new { solarSystemId, bodyCount });
    }
}
