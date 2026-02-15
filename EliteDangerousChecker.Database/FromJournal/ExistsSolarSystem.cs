using Dapper;

namespace EliteDangerousChecker.Database.FromJournal;
public static class ExistsSolarSystem
{
    public static async Task<bool> Execute(long systemAddress)
    {
        using var connection = DbAccess.GetOpenConnection();
        var sql = "select count(1) from SolarSystem where Id = @Id";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Id = systemAddress });
        return count > 0;
    }
}
