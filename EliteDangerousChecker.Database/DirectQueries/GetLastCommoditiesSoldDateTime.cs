using Dapper;

namespace EliteDangerousChecker.Database.DirectQueries;
public static class GetLastCommoditiesSoldDateTime
{
    public static async Task<DateTime> Execute()
    {
        using var connection = DbAccess.GetOpenConnection();
        var sql = @$"
            select 
                coalesce(max(Timestamp), 0)
            from 
                dbo.CommoditiesSold
        ";
        var result = await connection.QuerySingleAsync<long>(sql);

        return DbHelper.UnixToDateTime(result);
    }
}
