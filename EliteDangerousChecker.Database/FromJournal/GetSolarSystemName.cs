using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class GetSolarSystemName
{
    public static async Task<string> Execute(long systemAddress)
    {
        using var connection = DbAccess.GetOpenConnection();

        var sql = "select * from GetSectorPrefixName(@systemAddress)";
        return await connection.ExecuteScalarAsync<string>(sql, new { systemAddress }) ?? "Unknown System";
    }
}
