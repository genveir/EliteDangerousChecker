using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateBodySignalType
{
    public static async Task Execute(long systemAddress, long bodyId, string? bodyName, int signalCount)
    {
        await EnsureBodyExists.Execute(systemAddress, bodyId, bodyName);

        using var connection = DbAccess.GetOpenConnection();

        var signalTypeId = await SignalTypeAccess.GetId("$SAA_SignalType_Biological;");

        var existsSignalSql = "select case when exists (select 1 from BodySignalType where SolarSystemId = @systemAddress and BodyId = @bodyId and SignalTypeId = @signalTypeId) then 1 else 0 end";
        var signalExists = await connection.ExecuteScalarAsync<bool>(existsSignalSql, new { systemAddress, bodyId, signalTypeId });

        if (signalExists)
        {
            var updateSignalSql = @"
update BodySignalType
set Number = @signalCount
where SolarSystemId = @systemAddress and BodyId = @bodyId";
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
