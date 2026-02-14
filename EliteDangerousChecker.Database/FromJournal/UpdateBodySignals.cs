using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateBodySignals
{
    public static async Task Execute(long systemAddress, long bodyId, string? bodyName, int signalCount)
    {
        using var connection = DbAccess.GetOpenConnection();

        var signalTypeId = await SignalTypeAccess.GetId("$SAA_SignalType_Biological;");

        var existsSql = "select case when exists (select 1 from Body where SolarSystemId = @systemAddress and BodyId = @bodyId) then 1 else 0 end";
        var bodyExists = await connection.ExecuteScalarAsync<bool>(existsSql, new { systemAddress, bodyId });

        if (!bodyExists)
        {
            var insertBodySql = @"
insert into Body (SolarSystemId, BodyId, Name)
values (@systemAddress, @bodyId, @bodyName)";
            await connection.ExecuteAsync(insertBodySql, new { systemAddress, bodyId, bodyName });
        }

        var existsSignalSql = "select case when exists (select 1 from BodySignalType where SolarSystemId = @systemAddress and BodyId = @bodyId and SignalTypeId = @signalTypeId) then 1 else 0 end";
        var signalExists = await connection.ExecuteScalarAsync<bool>(existsSignalSql, new { systemAddress, bodyId, signalTypeId });

        if (signalExists)
        {
            var updateSignalSql = @"
update BodySignalType
set Number = @signalCount";
            await connection.ExecuteAsync(updateSignalSql, new { signalCount, systemAddress, bodyId, signalTypeId });
        }
        else
        {
            var insertSignalSql = @"
insert into BodySignalType (SolarSystemId, BodyId, SignalTypeId, Number)
values (@systemAddress, @bodyId, @signalTypeId, @signalCount)";
            await connection.ExecuteAsync(insertSignalSql, new { systemAddress, bodyId, signalTypeId, signalCount });
        }
    }
}
