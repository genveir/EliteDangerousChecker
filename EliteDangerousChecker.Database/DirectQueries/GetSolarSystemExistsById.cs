using Dapper;

namespace EliteDangerousChecker.Database.DirectQueries;
public static class GetSolarSystemExistsById
{
    public static async Task<bool> Execute(long solarSystemId)
    {
        using var connection = DbAccess.GetOpenConnection();

        var result = await connection.QueryAsync<int>(
            "select 1 from SolarSystem where Id = @Id64",
            new { Id64 = solarSystemId });

        return result.Any();
    }
}
