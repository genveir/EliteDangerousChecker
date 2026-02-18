using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateSolarSystemBodyCount
{
    public static async Task Execute(long solarSystemId, int bodyCount)
    {
        Console.WriteLine($"Updating body count for solar system {solarSystemId} to {bodyCount}.");

        using var connection = DbAccess.GetOpenConnection();
        var sql = @"
update SolarSystem
set BodyCount = @bodyCount
where Id = @solarSystemId";
        await connection.ExecuteAsync(sql, new { solarSystemId, bodyCount });
    }
}
