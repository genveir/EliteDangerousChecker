namespace EliteDangerousChecker.Database.Shared.ImmediateWrite;

public record BodySubType(long Id, string Name) : IIdAndNameTableEntity { }

public static class BodySubTypeAccess
{
    private static Dictionary<string, long>? Items { get; set; }

    public static async Task<long?> GetId(string? name) => await IdAndNameTableAccess.GetId(
        name,
        getItemsTask: GetItems,
        addItemTask: AddItem);

    public static async Task<string?> GetName(long id)
    {
        var items = await GetItems();

        var item = items.FirstOrDefault(kv => kv.Value == id);

        return item.Key;
    }

    private static async Task<Dictionary<string, long>> GetItems()
    {
        if (Items == null)
            await RefreshItems();

        return Items!;
    }

    private static async Task RefreshItems()
    {
        Items = await IdAndNameTableAccess.GetItems<BodySubType>(tableName: "BodySubType");
    }

    private static async Task AddItem(long id, string value) => await IdAndNameTableAccess.AddItem<BodySubType>(tableName: "BodySubType", id, value);
}
