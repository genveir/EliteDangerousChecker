using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateSpeciesFromOrganicSale
{
    public static async Task Execute(string species, int Value, int Bonus)
    {
        using var connection = DbAccess.GetOpenConnection();

        var speciesId = await SpeciesAccess.GetId(species);

        var updateSql = @"
update 
    Species
set 
    Value = @Value, 
    Bonus = @Bonus
where 
    Id = @speciesId";

        await connection.ExecuteAsync(updateSql, new
        {
            speciesId,
            Value,
            Bonus
        });
    }
}
