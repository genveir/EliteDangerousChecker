using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class SetBodyLandedByCommander
{
    public static async Task Execute(long systemAddress, int bodyId)
    {
        Console.WriteLine($"Setting body {bodyId} in system {systemAddress} as landed by commander.");

        using var connection = DbAccess.GetOpenConnection();
        var updateSql = @"
update Body
set Landed = 3
where SolarSystemId = @systemAddress and BodyId = @bodyId and Landed = 1";
        await connection.ExecuteAsync(updateSql, new { systemAddress, bodyId });
    }
}
