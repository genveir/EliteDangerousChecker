using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
internal static class EnsureBodyExists
{
    public static async Task Execute(long systemAddress, long bodyId, string? bodyName)
    {
        using var connection = DbAccess.GetOpenConnection();

        var existsSql = "select case when exists (select 1 from Body where SolarSystemId = @systemAddress and BodyId = @bodyId) then 1 else 0 end";
        var bodyExists = await connection.ExecuteScalarAsync<bool>(existsSql, new { systemAddress, bodyId });

        if (bodyName != null)
        {
            var solarSystemName = await GetSolarSystemName.Execute(systemAddress);

            bodyName = bodyName.Replace(solarSystemName, "");
        }

        if (!bodyExists)
        {
            var insertBodySql = @"
insert into Body (SolarSystemId, BodyId, Name)
values (@systemAddress, @bodyId, @bodyName)";
            await connection.ExecuteAsync(insertBodySql, new { systemAddress, bodyId, bodyName });
        }
    }
}
