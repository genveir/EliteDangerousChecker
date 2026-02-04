using Dapper;
using EliteDangerousChecker.Database.Shared;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EliteDangerousChecker.Database.FromJournal;
public class UpdateMarket
{
    long stationId;
    private readonly DateTime updateTime;

    private readonly DataTable StationCommodities;

    public UpdateMarket(long stationId, DateTime updateTime)
    {
        this.stationId = stationId;
        this.updateTime = updateTime;

        StationCommodities = DataTables.SetupStationCommoditiesDataTable();
    }

    public async Task AddItem(string itemName, string? categoryName, int? buyPrice, int? sellPrice, int? demand, int? supply)
    {
        var row = StationCommodities.NewRow();
        row["StationId"] = stationId;
        row["CommodityId"] = await CommodityAccess.GetId(itemName, categoryName);
        row["Demand"] = DbHelper.ValueOrDbNull(demand);
        row["Supply"] = DbHelper.ValueOrDbNull(supply);
        row["BuyPrice"] = DbHelper.ValueOrDbNull(buyPrice);
        row["SellPrice"] = DbHelper.ValueOrDbNull(sellPrice);
        StationCommodities.Rows.Add(row);
    }

    public async Task DoUpdate()
    {
        Console.WriteLine($"Starting market update for station {stationId}");

        using var connection = DbAccess.GetOpenConnection();
        //using var transaction = connection.BeginTransaction();

        await Updater.RecreateUpdateTables();

        await PushDataToDatabase(connection);//, transaction);

        await Updater.MergeStationCommoditiesAsync();

        await UpdateStation(connection);//, transaction);

        //transaction.Commit();
    }

    private async Task PushDataToDatabase(SqlConnection connection)//, SqlTransaction transaction)
    {
        using var bulkCopy = new SqlBulkCopy(connection);//, SqlBulkCopyOptions.Default, transaction);

        bulkCopy.DestinationTableName = $"upd.StationCommodities";
        await bulkCopy.WriteToServerAsync(StationCommodities);
    }

    private async Task UpdateStation(SqlConnection connection)//, SqlTransaction transaction)
    {
        var sql = @"update Station set MarketUpdateTime = @MarketUpdatedAt where Id = @Id";

        var unixUpdateTime = DbHelper.DateTimeToUnix(updateTime);

        await connection.ExecuteAsync(sql, new { MarketUpdatedAt = unixUpdateTime, Id = stationId });//, transaction);
    }
}
