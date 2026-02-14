using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.Shared;
internal class SolarSystemLocationCalculator
{
    public static async Task<(long? regionId, int subSector)> Calculate(double? x, double? y, double? z)
    {
        var regionId = await GetRegion(x, y, z);
        var subSector = CalculateSubSector(x, y, z);
        return (regionId, subSector);
    }

    private static async Task<long?> GetRegion(double? x, double? y, double? z)
    {
        return await SolarSystemRegionAccess.GetId(x, y, z);
    }

    private static int CalculateSubSector(double? x, double? y, double? z)
    {
        var sectorX = (int)Math.Floor((x ?? 0) / 100.0d);
        var sectorY = (int)Math.Floor((y ?? 0) / 100.0d);
        var sectorZ = (int)Math.Floor((z ?? 0) / 100.0d);

        var subSectorX = (int)Math.Floor(((x ?? 0) - sectorX * 100) / 10.0d);
        var subSectorY = (int)Math.Floor(((y ?? 0) - sectorY * 100) / 10.0d);
        var subSectorZ = (int)Math.Floor(((z ?? 0) - sectorZ * 100) / 10.0d);

        return subSectorX * 100 + subSectorY * 10 + subSectorZ;
    }
}
