using Dapper;

namespace EliteDangerousChecker.Database.Update.ImmediateWrite;
public record Commodity(long Id, string Name) : IIdAndNameTableEntity { }

public static class CommodityAccess
{
    private static Dictionary<string, long>? Items { get; set; }

    public static async Task<long?> GetId(string? name, string? categoryName)
    {
        if (name == null)
            return null;

        var items = await GetItems();

        if (!items.TryGetValue(name, out long _))
        {
            long newId = items.Count == 0
               ? 1
               : items.Values.Max() + 1;

            await AddItem(newId, categoryName ?? "Unknown", name);
            items[name] = newId;
        }

        return items[name];
    }

    private static async Task<Dictionary<string, long>> GetItems()
    {
        if (Items == null)
            await RefreshItems();

        return Items!;
    }

    private static async Task RefreshItems()
    {
        Items = await IdAndNameTableAccess.GetItems<Commodity>(tableName: "Commodity");
    }

    private static async Task AddItem(long id, string? category, string value)
    {
        var categoryId = await CommodityCategoryAccess.GetId(category);

        using var connection = DbAccess.GetOpenConnection();

        // vol injectbaar. Is okee, tableName wordt alleen geset binnen de applicatie.
        var sql = @$"insert into Commodity (Id, CommodityCategoryId, Name) values (@Id, @CategoryId, @Name)";

        await connection.ExecuteAsync(sql, new { Id = id, CategoryId = categoryId, Name = value });
    }
}
