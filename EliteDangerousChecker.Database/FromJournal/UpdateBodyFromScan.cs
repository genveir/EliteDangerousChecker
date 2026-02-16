namespace EliteDangerousChecker.Database.FromJournal;

using Dapper;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;
using Microsoft.Data.SqlClient;

public static class UpdateBodyFromScan
{
    public static async Task Execute(long systemAddress, int bodyId, string name, bool? wasDiscovered, bool? wasMapped, bool? wasFootfalled, string? terraformState, string? planetClass, string? starType)
    {
        Console.WriteLine($"Updating body from scan for systemAddress {systemAddress} and bodyId {bodyId}");

        if (planetClass != null && starType != null)
        {
            Console.WriteLine($"Scan for systemAddress {systemAddress} and bodyId {bodyId} has both planetclass and starType. Aborting update.");
            return;
        }

        if (planetClass == null && starType == null)
        {
            return;
        }

        string bodyTypeName = starType != null ? "Star" : "Planet";

        long? discoveredValue = wasDiscovered.HasValue ? (wasDiscovered.Value ? 2 : 3) : null;
        long? mappedValue = wasMapped.HasValue ? (wasMapped.Value ? 2 : 1) : null;
        long? footfalledValue = wasFootfalled.HasValue ? (wasFootfalled.Value ? 2 : 1) : null;

        long? terraFormStateId = await TerraformingStateAccess.GetId(terraformState);
        long? bodySubTypeId = await BodySubTypeAccess.GetId(ConvertPlanetClass(planetClass));
        long bodyTypeId = (await BodyTypeAccess.GetId(bodyTypeName)).Value;

        bool mainStar = false;

        var solarSystemName = await GetSolarSystemName.Execute(systemAddress);
        name = name.Replace(solarSystemName, "");

        if (starType != null)
        {
            bodySubTypeId = await BodySubTypeAccess.GetId(starType);

            if (name.Trim() == "A" || name.Trim() == "")
            {
                mainStar = true;
            }
        }

        using var connection = DbAccess.GetOpenConnection();

        var existsSql = "select case when exists (select 1 from Body where SolarSystemId = @systemAddress and BodyId = @bodyId) then 1 else 0 end";

        var bodyExists = await connection.ExecuteScalarAsync<bool>(existsSql, new { systemAddress, bodyId });

        if (bodyExists)
        {
            var explorationDataSql = "select Discovered, Mapped, Landed from Body where SolarSystemId = @systemAddress and BodyId = @bodyId";
            var explorationData = await connection.QuerySingleAsync<ExplorationData>(explorationDataSql, new { systemAddress, bodyId });

            await Update(
                connection: connection,
                systemAddress: systemAddress,
                bodyId: bodyId,
                discoveredId: explorationData.Discovered ?? discoveredValue,
                mappedId: explorationData.Mapped ?? mappedValue,
                footfalledId: explorationData.Landed ?? footfalledValue,
                terraformStateId: terraFormStateId,
                bodyTypeId: bodyTypeId,
                bodySubTypeId: bodySubTypeId);
        }
        else
        {
            await Insert(
                connection: connection,
                systemAddress: systemAddress,
                bodyId: bodyId,
                name: name,
                discoveredId: discoveredValue,
                mappedId: mappedValue,
                footfalledId: footfalledValue,
                terraformStateId: terraFormStateId,
                bodyTypeId: bodyTypeId,
                bodySubTypeId: bodySubTypeId,
                mainStar: mainStar);
        }
    }

    private class ExplorationData
    {
        public long? Discovered { get; set; }
        public long? Mapped { get; set; }
        public long? Landed { get; set; }
    }

    private static async Task Update(
        SqlConnection connection,
        long systemAddress,
        int bodyId,
        long? discoveredId,
        long? mappedId,
        long? footfalledId,
        long? terraformStateId,
        long bodyTypeId,
        long? bodySubTypeId)
    {
        var updateSql = @"
            update Body
            set Discovered = @discoveredId,
                Mapped = @mappedId,
                Landed = @footfalledId,
                TerraformingStateId = @terraformStateId,
                BodyTypeId = @bodyTypeId,
                BodySubTypeId = @bodySubTypeId
            where SolarSystemId = @systemAddress and BodyId = @bodyId";

        await connection.ExecuteAsync(updateSql, new { systemAddress, bodyId, discoveredId, mappedId, footfalledId, terraformStateId, bodyTypeId, bodySubTypeId });
    }

    private static async Task Insert(
        SqlConnection connection,
        long systemAddress,
        int bodyId,
        string name,
        long? discoveredId,
        long? mappedId,
        long? footfalledId,
        long? terraformStateId,
        long bodyTypeId,
        long? bodySubTypeId,
        bool mainStar)
    {
        var insertSql = @"
            insert into Body (SolarSystemId, BodyId, Name, Discovered, Mapped, Landed, TerraformingStateId, BodyTypeId, BodySubTypeId, Mainstar) values
            (@systemAddress, @bodyId, @name, @discoveredId, @mappedId, @footfalledId, @terraformStateId, @bodyTypeId, @bodySubTypeId, @mainStar)";

        await connection.ExecuteAsync(insertSql, new { systemAddress, bodyId, name, discoveredId, mappedId, footfalledId, terraformStateId, bodyTypeId, bodySubTypeId, mainStar });
    }

    private static string? ConvertPlanetClass(string? planetClass)
    {
        if (string.IsNullOrEmpty(planetClass))
            return null;

        // Helium-rich gas giant and Helium gas giant have not been encountered yet. Possibly other types are unknown as well.
        return planetClass switch
        {
            "High metal content body" => "High metal content world",
            "Sudarsky class III gas giant" => "Class III gas giant",
            "Icy body" => "Icy body",
            "Water world" => "Water world",
            "Sudarsky class I gas giant" => "Class I gas giant",
            "Rocky ice body" => "Rocky Ice world",
            "Metal rich body" => "Metal-rich body",
            "Rocky body" => "Rocky body",
            "Gas giant with water based life" => "Gas giant with water-based life",
            "Sudarsky class II gas giant" => "Class II gas giant",
            "Earthlike body" => "Earth-like world",
            "Sudarsky class V gas giant" => "Class V gas giant",
            "Sudarsky class IV gas giant" => "Class IV gas giant",
            "Ammonia world" => "Ammonia world",
            "Gas giant with ammonia based life" => "Gas giant with ammonia-based life",
            "Water giant" => "Water giant",
            _ => UnknownPlanetClass(planetClass)
        };
    }

    private static string? UnknownPlanetClass(string planetClass)
    {
        Console.WriteLine($"UpdateBodyFromScan -> Unknown planet class: {planetClass}");
        return "Unknown";
    }
}
