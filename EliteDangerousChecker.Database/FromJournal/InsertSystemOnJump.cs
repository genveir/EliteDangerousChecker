using Dapper;
using EliteDangerousChecker.Database.Shared;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.FromJournal;
public static class InsertSystemOnJump
{
    public static async Task Execute(long systemAddress, double x, double y, double z, string systemName)
    {
        var connection = DbAccess.GetOpenConnection();

        var (prefixWords, suffix, postfix) = SolarSystemNameParser.Parse(systemName);

        var sectorSuffixId = await SectorSuffixAccess.GetId(suffix);
        var sectorPostfixId = await SectorPostfixAccess.GetId(postfix);

        var (regionId, subSector) = await SolarSystemLocationCalculator.Calculate(x, y, z);

        var sql = @"
insert into SolarSystem (Id, X, Y, Z, SectorPostFixId, SectorSuffixId, SolarSystemRegionId, SubSector)
values (@Id, @X, @Y, @Z, @SectorPostFixId, @SectorSuffixId, @SolarSystemRegionId, @SubSector)";
        await connection.ExecuteAsync(sql, new
        {
            Id = systemAddress,
            X = x,
            Y = y,
            Z = z,
            SectorPostFixId = sectorPostfixId,
            SectorSuffixId = sectorSuffixId,
            SolarSystemRegionId = regionId,
            SubSector = subSector
        });

        await SectorPrefixInserter.InsertSectorPrefix(systemAddress, prefixWords);
    }
}
