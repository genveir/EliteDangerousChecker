using Microsoft.Data.SqlClient;

namespace EliteDangerousChecker.Database.Shared;
public static class Updater
{
    public static SqlConnection GetConnection() => DbAccess.GetOpenConnection();

    public static async Task UpdateDatabaseFromUpdateTables()
    {
        Console.WriteLine("Starting Update");

        await MergeFactionAsync();
        await MergeSolarSystemAsync();
        await MergeSolarSystemFactionAsync();
        await MergeSolarSystemPowerAsync();
        await MergeSolarSystemPowerConflictProgressAsync();
        await MergeBodiesAsync();
        await MergeStationsAsync();
        await MergeStationCommoditiesAsync();
        await MergeStationEconomiesAsync();
        await MergeStationServicesAsync();
        await MergeStationsMappedToPlaceholderFactionAsync();
        await MergeSectorPrefixAsync();
        await MergeRingsAsync();
        await MergeBodySignalTypesAsync();
        await MergeBodySignalGenusesAsync();
        await MergeRingSignalTypesAsync();
        await MergeRingSignalGenusesAsync();

        Console.WriteLine("Update Complete!");
    }

    public static async Task RecreateUpdateTables()
    {
        Console.WriteLine("Recreating update tables");

        var sql = @"
exec dbo.RecreateUpdateTables;";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeFactionAsync()
    {
        Console.WriteLine("Merging factions");

        var sql = @"
MERGE INTO dbo.Faction AS target
USING upd.Faction AS source
ON target.Id = source.Id
WHEN MATCHED THEN UPDATE SET
    Name = source.Name,
    AllegianceId = source.AllegianceId,
    GovernmentId = source.GovernmentId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Name, AllegianceId, GovernmentId)
    VALUES (source.Id, source.Name, source.AllegianceId, source.GovernmentId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeSolarSystemAsync()
    {
        Console.WriteLine("Merging solar systems");

        var sql = @"
MERGE INTO dbo.SolarSystem AS target
USING upd.SolarSystem AS source
ON target.Id = source.Id
WHEN MATCHED THEN UPDATE SET
    X = source.X,
    Y = source.Y,
    Z = source.Z,
    AllegianceId = source.AllegianceId,
    GovernmentId = source.GovernmentId,
    PrimaryEconomyId = source.PrimaryEconomyId,
    SecondaryEconomyId = source.SecondaryEconomyId,
    SecurityId = source.SecurityId,
    Population = source.Population,
    BodyCount = source.BodyCount,
    ControllingFactionId = source.ControllingFactionId,
    Date = source.Date,
    PowerStateTimestamp = source.PowerStateTimestamp,
    PowersTimestamp = source.PowersTimestamp,
    ControllingPowerTimestamp = source.ControllingPowerTimestamp,
    ControllingPowerId = source.ControllingPowerId,
    PowerStateId = source.PowerStateId,
    PowerStateControlProgress = source.PowerStateControlProgress,
    PowerStateReinforcement = source.PowerStateReinforcement,
    PowerStateUndermining = source.PowerStateUndermining,
    SectorPostfixId = source.SectorPostfixId,
    SectorSuffixId = source.SectorSuffixId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, X, Y, Z, AllegianceId, GovernmentId, PrimaryEconomyId, SecondaryEconomyId, SecurityId, Population, BodyCount, ControllingFactionId, Date, PowerStateTimestamp, PowersTimestamp, ControllingPowerTimestamp, ControllingPowerId, PowerStateId, PowerStateControlProgress, PowerStateReinforcement, PowerStateUndermining, SectorPostfixId, SectorSuffixId)
    VALUES (source.Id, source.X, source.Y, source.Z, source.AllegianceId, source.GovernmentId, source.PrimaryEconomyId, source.SecondaryEconomyId, source.SecurityId, source.Population, source.BodyCount, source.ControllingFactionId, source.Date, source.PowerStateTimestamp, source.PowersTimestamp, source.ControllingPowerTimestamp, source.ControllingPowerId, source.PowerStateId, source.PowerStateControlProgress, source.PowerStateReinforcement, source.PowerStateUndermining, source.SectorPostfixId, source.SectorSuffixId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeSolarSystemFactionAsync()
    {
        Console.WriteLine("Merging solar system factions");

        var sql = @"
MERGE INTO dbo.SolarSystemFaction AS target
USING upd.SolarSystemFaction AS source
ON target.SolarSystemId = source.SolarSystemId AND target.FactionId = source.FactionId
WHEN MATCHED THEN UPDATE SET
    Influence = source.Influence,
    FactionStateId = source.FactionStateId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (SolarSystemId, FactionId, Influence, FactionStateId)
    VALUES (source.SolarSystemId, source.FactionId, source.Influence, source.FactionStateId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeSolarSystemPowerAsync()
    {
        Console.WriteLine("Merging solar system powers");

        var sql = @"
MERGE INTO dbo.SolarSystemPower AS target
USING upd.SolarSystemPower AS source
ON target.SolarSystemId = source.SolarSystemId AND target.PowerId = source.PowerId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (SolarSystemId, PowerId)
    VALUES (source.SolarSystemId, source.PowerId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeSolarSystemPowerConflictProgressAsync()
    {
        Console.WriteLine("Merging solar system power conflict progress");

        var sql = @"
MERGE INTO dbo.SolarSystemPowerConflictProgress AS target
USING upd.SolarSystemPowerConflictProgress AS source
ON target.SolarSystemId = source.SolarSystemId AND target.PowerId = source.PowerId
WHEN MATCHED THEN UPDATE SET
    Progress = source.Progress
WHEN NOT MATCHED BY TARGET THEN
    INSERT (SolarSystemId, PowerId, Progress)
    VALUES (source.SolarSystemId, source.PowerId, source.Progress);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeBodiesAsync()
    {
        Console.WriteLine("Merging bodies");

        var sql = @"
MERGE INTO dbo.Body AS target
USING upd.Body AS source
ON target.Id = source.Id
WHEN MATCHED THEN UPDATE SET
    BodyId = source.BodyId,
    Name = source.Name,
    BodyTypeId = source.BodyTypeId,
    BodySubTypeId = source.BodySubTypeId,
    DistanceToArrival = source.DistanceToArrival,
    Mainstar = source.Mainstar,
    Age = source.Age,
    SpectralClassId = source.SpectralClassId,
    LuminosityId = source.LuminosityId,
    AbsoluteMagnitude = source.AbsoluteMagnitude,
    SolarMasses = source.SolarMasses,
    SurfaceTemperature = source.SurfaceTemperature,
    RotationalPeriod = source.RotationalPeriod,
    RotationalPeriodTidallyLocked = source.RotationalPeriodTidallyLocked,
    AxialTilt = source.AxialTilt,
    OrbitalPeriod = source.OrbitalPeriod,
    SemiMajorAxis = source.SemiMajorAxis,
    OrbitalEccentricity = source.OrbitalEccentricity,
    OrbitalInclination = source.OrbitalInclination,
    ArgOfPeriapsis = source.ArgOfPeriapsis,
    MeanAnomaly = source.MeanAnomaly,
    AscendingNode = source.AscendingNode,
    IsLandable = source.IsLandable,
    Gravity = source.Gravity,
    EarthMasses = source.EarthMasses,
    Radius = source.Radius,
    SurfacePressure = source.SurfacePressure,
    VolcanismTypeId = source.VolcanismTypeId,
    AtmosphereTypeId = source.AtmosphereTypeId,
    TerraformingStateId = source.TerraformingStateId,
    ReserveLevelId = source.ReserveLevelId,
    UpdateTime = source.UpdateTime,
    DistanceToArrivalTimestamp = source.DistanceToArrivalTimestamp,
    MeanAnomalyTimestamp = source.MeanAnomalyTimestamp,
    AscendingNodeTimestamp = source.AscendingNodeTimestamp,
    SolarSystemId = source.SolarSystemId,
    SolarSystemNameIsPrefix = source.SolarSystemNameIsPrefix,
    SignalsUpdateTime = source.SignalsUpdateTime,
    Carbon = source.Carbon,
    Iron = source.Iron,
    Nickel = source.Nickel,
    Niobium = source.Niobium,
    Phosphorus = source.Phosphorus,
    Sulphur = source.Sulphur,
    Tellurium = source.Tellurium,
    Tungsten = source.Tungsten,
    Vanadium = source.Vanadium,
    Zinc = source.Zinc,
    Zirconium = source.Zirconium,
    Germanium = source.Germanium,
    Manganese = source.Manganese,
    Molybdenum = source.Molybdenum,
    Selenium = source.Selenium,
    Yttrium = source.Yttrium,
    Cadmium = source.Cadmium,
    Ruthenium = source.Ruthenium,
    Arsenic = source.Arsenic,
    Antimony = source.Antimony,
    Chromium = source.Chromium,
    Tin = source.Tin,
    Mercury = source.Mercury,
    Technetium = source.Technetium,
    Polonium = source.Polonium
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, BodyId, Name, BodyTypeId, BodySubTypeId, DistanceToArrival, Mainstar, Age, SpectralClassId, LuminosityId, AbsoluteMagnitude, SolarMasses, SurfaceTemperature, RotationalPeriod, RotationalPeriodTidallyLocked, AxialTilt, OrbitalPeriod, SemiMajorAxis, OrbitalEccentricity, OrbitalInclination, ArgOfPeriapsis, MeanAnomaly, AscendingNode, IsLandable, Gravity, EarthMasses, Radius, SurfacePressure, VolcanismTypeId, AtmosphereTypeId, TerraformingStateId, ReserveLevelId, UpdateTime, DistanceToArrivalTimestamp, MeanAnomalyTimestamp, AscendingNodeTimestamp, SolarSystemId, SolarSystemNameIsPrefix, SignalsUpdateTime, Carbon, Iron, Nickel, Niobium, Phosphorus, Sulphur, Tellurium, Tungsten, Vanadium, Zinc, Zirconium, Germanium, Manganese, Molybdenum, Selenium, Yttrium, Cadmium, Ruthenium, Arsenic, Antimony, Chromium, Tin, Mercury, Technetium, Polonium)
    VALUES (source.Id, source.BodyId, source.Name, source.BodyTypeId, source.BodySubTypeId, source.DistanceToArrival, source.Mainstar, source.Age, source.SpectralClassId, source.LuminosityId, source.AbsoluteMagnitude, source.SolarMasses, source.SurfaceTemperature, source.RotationalPeriod, source.RotationalPeriodTidallyLocked, source.AxialTilt, source.OrbitalPeriod, source.SemiMajorAxis, source.OrbitalEccentricity, source.OrbitalInclination, source.ArgOfPeriapsis, source.MeanAnomaly, source.AscendingNode, source.IsLandable, source.Gravity, source.EarthMasses, source.Radius, source.SurfacePressure, source.VolcanismTypeId, source.AtmosphereTypeId, source.TerraformingStateId, source.ReserveLevelId, source.UpdateTime, source.DistanceToArrivalTimestamp, source.MeanAnomalyTimestamp, source.AscendingNodeTimestamp, source.SolarSystemId, source.SolarSystemNameIsPrefix, source.SignalsUpdateTime, source.Carbon, source.Iron, source.Nickel, source.Niobium, source.Phosphorus, source.Sulphur, source.Tellurium, source.Tungsten, source.Vanadium, source.Zinc, source.Zirconium, source.Germanium, source.Manganese, source.Molybdenum, source.Selenium, source.Yttrium, source.Cadmium, source.Ruthenium, source.Arsenic, source.Antimony, source.Chromium, source.Tin, source.Mercury, source.Technetium, source.Polonium);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 60;
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeStationsAsync()
    {
        Console.WriteLine("Merging stations");

        var sql = @"
MERGE INTO dbo.Station AS target
USING upd.Station AS source
ON target.Id = source.Id
WHEN MATCHED THEN UPDATE SET
    Name = source.Name,
    UpdateTime = source.UpdateTime,
    RealName = source.RealName,
    ControllingFactionId = source.ControllingFactionId,
    ControllingFactionStateId = source.ControllingFactionStateId,
    DistanceToArrival = source.DistanceToArrival,
    PrimaryEconomyId = source.PrimaryEconomyId,
    GovernmentId = source.GovernmentId,
    StationTypeId = source.StationTypeId,
    StateId = source.StateId,
    LargePads = source.LargePads,
    MediumPads = source.MediumPads,
    SmallPads = source.SmallPads,
    MarketUpdateTime = source.MarketUpdateTime,
    CarrierDockingAccessId = source.CarrierDockingAccessId,
    CarrierName = source.CarrierName,
    ShipyardUpdateTime = source.ShipyardUpdateTime,
    OutfittingUpdateTime = source.OutfittingUpdateTime,
    AllegianceId = source.AllegianceId,
    Latitude = source.Latitude,
    Longitude = source.Longitude,
    BodyId = source.BodyId,
    SolarSystemId = source.SolarSystemId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Name, UpdateTime, RealName, ControllingFactionId, ControllingFactionStateId, DistanceToArrival, PrimaryEconomyId, GovernmentId, StationTypeId, StateId, LargePads, MediumPads, SmallPads, MarketUpdateTime, CarrierDockingAccessId, CarrierName, ShipyardUpdateTime, OutfittingUpdateTime, AllegianceId, Latitude, Longitude, BodyId, SolarSystemId)
    VALUES (source.Id, source.Name, source.UpdateTime, source.RealName, source.ControllingFactionId, source.ControllingFactionStateId, source.DistanceToArrival, source.PrimaryEconomyId, source.GovernmentId, source.StationTypeId, source.StateId, source.LargePads, source.MediumPads, source.SmallPads, source.MarketUpdateTime, source.CarrierDockingAccessId, source.CarrierName, source.ShipyardUpdateTime, source.OutfittingUpdateTime, source.AllegianceId, source.Latitude, source.Longitude, source.BodyId, source.SolarSystemId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeStationCommoditiesAsync()
    {
        Console.WriteLine("Merging station commodities");

        var sql = @"
MERGE INTO dbo.StationCommodities AS target
USING upd.StationCommodities AS source
ON target.StationId = source.StationId AND target.CommodityId = source.CommodityId
WHEN MATCHED THEN UPDATE SET
    Demand = source.Demand,
    Supply = source.Supply,
    BuyPrice = source.BuyPrice,
    SellPrice = source.SellPrice
WHEN NOT MATCHED BY TARGET THEN
    INSERT (StationId, CommodityId, Demand, Supply, BuyPrice, SellPrice)
    VALUES (source.StationId, source.CommodityId, source.Demand, source.Supply, source.BuyPrice, source.SellPrice);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 60;
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeStationEconomiesAsync()
    {
        Console.WriteLine("Merging station economies");

        var sql = @"
MERGE INTO dbo.StationEconomies AS target
USING upd.StationEconomies AS source
ON target.StationId = source.StationId AND target.EconomyId = source.EconomyId
WHEN MATCHED THEN UPDATE SET
    Proportion = source.Proportion
WHEN NOT MATCHED BY TARGET THEN
    INSERT (StationId, EconomyId, Proportion)
    VALUES (source.StationId, source.EconomyId, source.Proportion);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeStationServicesAsync()
    {
        Console.WriteLine("Merging station services");

        var sql = @"
MERGE INTO dbo.StationServices AS target
USING upd.StationServices AS source
ON target.StationId = source.StationId AND target.ServiceId = source.ServiceId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (StationId, ServiceId)
    VALUES (source.StationId, source.ServiceId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeStationsMappedToPlaceholderFactionAsync()
    {
        Console.WriteLine("Merging stations mapped to placeholder faction");

        var sql = @"
MERGE INTO dbo.StationsMappedToPlaceholderFaction AS target
USING upd.StationsMappedToPlaceholderFaction AS source
ON target.StationId = source.StationId AND target.FactionName = source.FactionName
WHEN NOT MATCHED BY TARGET THEN
    INSERT (StationId, FactionName)
    VALUES (source.StationId, source.FactionName);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeSectorPrefixAsync()
    {
        Console.WriteLine("Merging sector prefixes");

        var sql = @"
MERGE INTO dbo.SectorPrefix AS target
USING upd.SectorPrefix AS source
ON target.SolarSystemId = source.SolarSystemId AND target.Sequence = source.Sequence
WHEN MATCHED THEN UPDATE SET
    SectorPrefixWordId = source.SectorPrefixWordId,
    SectorPrefixNumber = source.SectorPrefixNumber,
    StartWithDash = source.StartWithDash,
    StartWithJ = source.StartWithJ
WHEN NOT MATCHED BY TARGET THEN
    INSERT (SolarSystemId, Sequence, SectorPrefixWordId, SectorPrefixNumber, StartWithDash, StartWithJ)
    VALUES (source.SolarSystemId, source.Sequence, source.SectorPrefixWordId, source.SectorPrefixNumber, source.StartWithDash, source.StartWithJ);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeRingsAsync()
    {
        Console.WriteLine("Merging rings");

        var sql = @"
MERGE INTO dbo.Ring AS target
USING upd.Ring AS source
ON target.Id = source.Id
WHEN MATCHED THEN UPDATE SET
    Name = source.Name,
    BodyNameIsPrefix = source.BodyNameIsPrefix,
    BodyId = source.BodyId,
    RingTypeId = source.RingTypeId,
    Mass = source.Mass,
    InnerRadius = source.InnerRadius,
    OuterRadius = source.OuterRadius
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Name, BodyNameIsPrefix, BodyId, RingTypeId, Mass, InnerRadius, OuterRadius)
    VALUES (source.Id, source.Name, source.BodyNameIsPrefix, source.BodyId, source.RingTypeId, source.Mass, source.InnerRadius, source.OuterRadius);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeBodySignalTypesAsync()
    {
        Console.WriteLine("Merging body signal types");

        var sql = @"
MERGE INTO dbo.BodySignalType AS target
USING upd.BodySignalType AS source
ON target.BodyId = source.BodyId AND target.SignalTypeId = source.SignalTypeId
WHEN MATCHED THEN UPDATE SET
    Number = source.Number
WHEN NOT MATCHED BY TARGET THEN
    INSERT (BodyId, SignalTypeId, Number)
    VALUES (source.BodyId, source.SignalTypeId, source.Number);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeBodySignalGenusesAsync()
    {
        Console.WriteLine("Merging body signal genuses");

        var sql = @"
MERGE INTO dbo.BodySignalGenus AS target
USING upd.BodySignalGenus AS source
ON target.BodyId = source.BodyId AND target.SignalGenusId = source.SignalGenusId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (BodyId, SignalGenusId)
    VALUES (source.BodyId, source.SignalGenusId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeRingSignalTypesAsync()
    {
        Console.WriteLine("Merging ring signal types");

        var sql = @"
MERGE INTO dbo.RingSignalType AS target
USING upd.RingSignalType AS source
ON target.RingId = source.RingId AND target.SignalTypeId = source.SignalTypeId
WHEN MATCHED THEN UPDATE SET
    Number = source.Number
WHEN NOT MATCHED BY TARGET THEN
    INSERT (RingId, SignalTypeId, Number)
    VALUES (source.RingId, source.SignalTypeId, source.Number);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task MergeRingSignalGenusesAsync()
    {
        Console.WriteLine("Merging ring signal genuses");

        var sql = @"
MERGE INTO dbo.RingSignalGenus AS target
USING upd.RingSignalGenus AS source
ON target.RingId = source.RingId AND target.SignalGenusId = source.SignalGenusId
WHEN NOT MATCHED BY TARGET THEN
    INSERT (RingId, SignalGenusId)
    VALUES (source.RingId, source.SignalGenusId);";
        using var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
}