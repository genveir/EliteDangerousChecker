using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class SetBodyMappedByCommander
{
    public static async Task Execute(long systemAddress, int bodyId)
    {
        Console.WriteLine($"Setting body {bodyId} in system {systemAddress} as mapped by commander.");

        using var connection = DbAccess.GetOpenConnection();
        var updateSql = @"
update Body
set Mapped = 3
where SolarSystemId = @systemAddress and BodyId = @bodyId and Mapped = 1";
        await connection.ExecuteAsync(updateSql, new { systemAddress, bodyId });
    }
}
