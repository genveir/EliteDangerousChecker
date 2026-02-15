using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateBodySignalGenus
{
    public static async Task Execute(long systemAddress, long bodyId, string? bodyName, GenusWithLocalization[] genuses)
    {
        await EnsureBodyExists.Execute(systemAddress, bodyId, bodyName);

        using var connection = DbAccess.GetOpenConnection();

        foreach (var localizedGenus in genuses)
        {
            var genusId = await LocalizedSignalGenusAccess
                .GetId(localizedGenus.Genus, localizedGenus.LocalizedName);

            var bodySignalGenusExistsSql = "select case when exists (select 1 from BodySignalGenus where SolarSystemId = @systemAddress and BodyId = @bodyId and SignalGenusId = @genusId) then 1 else 0 end";
            var bodySignalGenusExists = await connection.ExecuteScalarAsync<bool>(bodySignalGenusExistsSql, new { systemAddress, bodyId, genusId });

            if (!bodySignalGenusExists)
            {
                var insertSql = @"
insert into BodySignalGenus (SolarSystemId, BodyId, SignalGenusId) values
(@systemAddress, @bodyId, @genusId)";
                await connection.ExecuteAsync(insertSql, new { systemAddress, bodyId, genusId });
            }
        }
    }

    public record GenusWithLocalization(string Genus, string LocalizedName);
}
