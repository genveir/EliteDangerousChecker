using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;

namespace EliteDangerousChecker.Database.FromJournal;
public static class UpdateSpeciesFromOrganicSale
{
    public static async Task Execute(string species, int Value, int Bonus)
    {
        using var connection = DbAccess.GetOpenConnection();

        var speciesId = await SpeciesAccess.GetId(species);

        var bonusClause = Bonus == 0 ? "" : ", Bonus = @Bonus";

        var updateSql = @$"
update 
    Species
set 
    Value = @Value{bonusClause}
where 
    Id = @speciesId";

        if (Bonus > 0)
        {
            await connection.ExecuteAsync(updateSql, new
            {
                speciesId,
                Value,
                Bonus
            });
        }
        else
        {
            await connection.ExecuteAsync(updateSql, new
            {
                speciesId,
                Value
            });
        }
    }
}
