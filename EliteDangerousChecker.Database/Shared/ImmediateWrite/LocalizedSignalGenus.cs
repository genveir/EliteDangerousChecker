using Dapper;

namespace EliteDangerousChecker.Database.Shared.ImmediateWrite;
public record LocalizedSignalGenus(long Id, string Name, string Localized);

public static class LocalizedSignalGenusAccess
{
    private static Dictionary<string, LocalizedSignalGenus>? Items { get; set; }

    private static async Task<LocalizedSignalGenus?> Get(string Name)
    {
        if (Items == null)
            await RefreshItems();

        return Items!.GetValueOrDefault(Name);
    }

    private static async Task RefreshItems()
    {
        using var connection = DbAccess.GetOpenConnection();

        var sql = @"select Id, Name, Localized from SignalGenus";

        var data = await connection.QueryAsync<LocalizedSignalGenus>(sql);

        Items = data.ToDictionary(x => x.Name, x => x);
    }

    public static async Task<long> GetId(string name, string localized)
    {
        var existing = await Get(name);

        if (existing == null)
        {
            return await AddItem(name, localized);
        }

        if (existing.Localized == null)
        {
            using var connection = DbAccess.GetOpenConnection();

            var sql = @"update SignalGenus set Localized = @localized where Id = @id";

            await connection.ExecuteAsync(sql, new { id = existing.Id, localized });

            Items![name] = existing with { Localized = localized };
        }

        return existing.Id;
    }

    private static async Task<long> AddItem(string name, string localized)
    {
        var existing = await Get(name);

        if (existing != null)
        {
            Console.WriteLine($"Cannot add SignalGenus '{name}' because it already exists");
            return existing.Id;
        }

        using var connection = DbAccess.GetOpenConnection();

        var id = Items!.Select(x => x.Value.Id).DefaultIfEmpty(0).Max() + 1;

        var sql = @"insert into SignalGenus (Id, Name, Localized) values (@id, @name, @localized)";

        await connection.ExecuteAsync(sql, new { id, name, localized });

        Items![name] = new LocalizedSignalGenus(id, name, localized);

        return id;
    }
}
