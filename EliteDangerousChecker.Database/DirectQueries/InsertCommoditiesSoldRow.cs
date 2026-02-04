using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.DirectQueries;
public static class InsertCommoditiesSoldRow
{
    public static async Task Execute(DateTime timestamp, long stationId, string commodity, int count, int sellPrice)
    {
        var commodityId = await CommodityAccess.GetId(commodity, null);
        var unixTimestamp = DbHelper.DateTimeToUnix(timestamp);

        using var connection = DbAccess.GetOpenConnection();

        var sql = @$"insert into CommoditiesSold (Timestamp, StationId, CommodityId, Count, SellPrice) values (@Timestamp, @StationId, @CommodityId, @Count, @SellPrice)";

        await connection.ExecuteAsync(sql, new { Timestamp = unixTimestamp, StationId = stationId, CommodityId = commodityId, Count = count, SellPrice = sellPrice });
    }
}
