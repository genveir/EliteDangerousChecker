using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateBodySignalGenusFromOrganicScan
{
    public static async Task Execute(long systemAddress, long bodyId, string genus, string species, bool scanned, bool finalScan)
    {
        Console.WriteLine($"Updating BodySignalGenus from Organic Scan");

        using var connection = DbAccess.GetOpenConnection();

        var signalGenusId = await SignalGenusAccess.GetId(genus);
        var speciesId = await SpeciesAccess.GetId(species);

        var currentScannedId = await connection.QueryFirstOrDefaultAsync<int?>(@"
select Scanned from BodySignalGenus where SolarSystemId = @systemAddress and BodyId = @bodyId and SignalGenusId = @signalGenusId",
            new { systemAddress, bodyId, signalGenusId });

        int scannedId = ResolveScannedId(currentScannedId, scanned, finalScan);

        var bodySignalGenusUpdateSql = @"
update
    BodySignalGenus
set
    SpeciesId = @speciesId,
    Scanned = @scannedId
where
    SolarSystemId = @systemAddress
    and BodyId = @bodyId
    and SignalGenusId = @signalGenusId";

        await connection.ExecuteAsync(bodySignalGenusUpdateSql, new
        {
            systemAddress,
            bodyId,
            signalGenusId,
            speciesId,
            scannedId
        });
    }

    private static int ResolveScannedId(int? currentScannedId, bool scanned, bool finalScan)
    {
        int result = 0;

        if (currentScannedId == null)
        {
            result = scanned ? 2 : 1;
        }
        else
        {
            result = currentScannedId.Value;
        }

        if (finalScan && result == 1)
        {
            result = 3;
        }

        return result;
    }
}
