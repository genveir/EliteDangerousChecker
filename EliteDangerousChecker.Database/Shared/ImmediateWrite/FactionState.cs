namespace EliteDangerousChecker.Database.Shared.ImmediateWrite;
public record FactionState(long Id, string Name) : IIdAndNameTableEntity { }

public static class FactionStateAccess
{
    private static Dictionary<string, long>? Items { get; set; }

    public static async Task<long?> GetId(string? name) => await IdAndNameTableAccess.GetId(
        name,
        getItemsTask: GetItems,
        addItemTask: AddItem);

    private static async Task<Dictionary<string, long>> GetItems()
    {
        if (Items == null)
            await RefreshItems();

        return Items!;
    }

    private static async Task RefreshItems()
    {
        Items = await IdAndNameTableAccess.GetItems<FactionState>(tableName: "FactionState");
    }

    private static async Task AddItem(long id, string value) => await IdAndNameTableAccess.AddItem<FactionState>(tableName: "FactionState", id, value);
}
