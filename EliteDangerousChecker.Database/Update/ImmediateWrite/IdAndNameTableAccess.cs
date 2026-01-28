using Dapper;

namespace EliteDangerousChecker.Database.Update.ImmediateWrite;
internal static class IdAndNameTableAccess
{
    private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public static async Task<long?> GetId(
        string? name,
        Func<Task<Dictionary<string, long>>> getItemsTask,
        Func<long, string, Task> addItemTask)
    {
        if (name == null)
            return null;

        var items = await getItemsTask();

        if (!items.TryGetValue(name, out long _))
        {
            {
                long newId = items.Count == 0
                ? 1
                : items.Values.Max() + 1;

                await addItemTask(newId, name);

                items.Add(name, newId);
            }
        }

        return items[name];
    }

    public static async Task<Dictionary<string, long>> GetItems<TObjectType>(string tableName)
        where TObjectType : IIdAndNameTableEntity
    {
        using var connection = DbAccess.GetOpenConnection();

        // vol injectbaar. Is okee, tableName wordt alleen geset binnen de applicatie.
        var sql = $"select Id, Name from {tableName}";

        var data = await connection.QueryAsync<TObjectType>(sql);

        return data.ToDictionary(k => k.Name, v => v.Id);
    }

    public static async Task AddItem<TObjectType>(string tableName, long id, string value)
        where TObjectType : IIdAndNameTableEntity
    {
        using var connection = DbAccess.GetOpenConnection();

        // vol injectbaar. Is okee, tableName wordt alleen geset binnen de applicatie.
        var sql = @$"insert into {tableName} (Id, Name) values (@Id, @Name)";

        await connection.ExecuteAsync(sql, new { Id = id, Name = value });
    }
}
