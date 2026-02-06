using Dapper;

namespace EliteDangerousChecker.Database.Shared.ImmediateWrite;

public record SolarSystemRegion(long Id, int XSector, int YSector, int ZSector);

public static class SolarSystemRegionAccess
{
    public static Dictionary<(int x, int y, int z), long> Ids { get; set; } = [];

    public static async Task<long> GetId(double? x, double? y, double? z)
    {
        if (x == null || y == null || z == null)
            return -1;

        var xSector = (int)Math.Floor(x.Value / 100.0d);
        var ySector = (int)Math.Floor(y.Value / 100.0d);
        var zSector = (int)Math.Floor(z.Value / 100.0d);

        if (Ids.Count == 0)
            await RefreshItems();

        if (Ids.TryGetValue((xSector, ySector, zSector), out var existingId))
            return existingId;

        return await AddItem(xSector, ySector, zSector);
    }

    private static async Task RefreshItems()
    {
        using var connection = DbAccess.GetOpenConnection();

        Ids = (await connection.QueryAsync<SolarSystemRegion>("SELECT Id, XSector, YSector, ZSector FROM SolarSystemRegion"))
            .ToDictionary(items => (items.XSector, items.YSector, items.ZSector), items => items.Id);
    }

    private static async Task<long> AddItem(int xSector, int ySector, int zSector)
    {
        using var connection = DbAccess.GetOpenConnection();

        var newId = await connection.ExecuteScalarAsync<long>(
            @"INSERT INTO SolarSystemRegion (XSector, YSector, ZSector) VALUES (@XSector, @YSector, @ZSector);
              SELECT CAST(SCOPE_IDENTITY() AS bigint);",
            new { XSector = xSector, YSector = ySector, ZSector = zSector });

        Ids.Add((xSector, ySector, zSector), newId);

        return newId;
    }
}
