using Dapper;
using EliteDangerousChecker.Database.Update.DumpModel;
using EliteDangerousChecker.Database.Update.ImmediateWrite;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EliteDangerousChecker.Database.Update;
public class BulkWriter
{
    DataTable SolarSystems { get; set; }

    DataTable Factions { get; set; }
    long nextFactionId { get; set; }
    Dictionary<string, long> FactionNameToId = new();

    DataTable SolarSystemFactions { get; set; }

    DataTable Bodies { get; set; }

    DataTable Stations { get; set; }
    DataTable StationEconomies { get; set; }
    DataTable StationServices { get; set; }
    DataTable StationsMappedToPlaceholderFaction { get; set; }

    public int RowCount => SolarSystems.Rows.Count;
    public bool Writing = false;

    public BulkWriter()
    {
        Reset();
    }

    private void Reset()
    {
        SolarSystems = SetupSolarSystemDataTable();
        Factions = SetupFactionDataTable();
        SolarSystemFactions = SetupSolarSystemFactionDataTable();
        Bodies = SetupBodiesDataTable();
        Stations = SetupStationsDataTable();
        StationEconomies = SetupStationEconomiesDataTable();
        StationServices = SetupStationServicesDataTable();
        StationsMappedToPlaceholderFaction = SetupStationsMappedToPlaceholderFactionDataTable();
    }

    public async Task Initialize()
    {
        using var connection = DbAccess.GetOpenConnection();

        var existingFactions = await connection.QueryAsync<ExistingFaction>("select Id, Name from Faction");
        foreach (var faction in existingFactions)
        {
            FactionNameToId[faction.Name] = faction.Id;
        }
        nextFactionId = existingFactions.Any()
            ? existingFactions.Max(f => f.Id) + 1
            : 1;

        // story NPC corp, no systems
        var brewerCorporation = new Faction()
        {
            Name = "Brewer Corporation",
            Allegiance = "Independent",
            Government = "Corporate"
        };
        await AddFaction(brewerCorporation);

        // fleet carriers belong to the FleetCarrier faction
        var fleetCarrierFaction = new Faction()
        {
            Name = "FleetCarrier",
            Allegiance = "Independent",
            Government = "Corporate"
        };
        await AddFaction(fleetCarrierFaction);

        // placeholder corp
        var placeHolderCorporation = new Faction()
        {
            Name = "PlaceHolder",
            Allegiance = "Independent",
            Government = "Corporate"
        };
        await AddFaction(placeHolderCorporation);
    }
    private record ExistingFaction(long Id, string Name);

    public async Task AddSystem(SolarSystem? solarSystem)
    {
        if (solarSystem == null)
            return;


        if (solarSystem.ControllingFaction != null)
        {
            await AddFaction(solarSystem.ControllingFaction);
        }

        if (solarSystem.Factions != null)
        {
            foreach (var faction in solarSystem.Factions)
            {
                await AddFaction(faction);
                await AddSolarSystemFaction(solarSystem, faction);
            }
        }

        await AddSolarSystemToDataTable(solarSystem);

        if (solarSystem.Bodies != null)
        {
            foreach (var body in solarSystem.Bodies)
            {
                await AddBody(body, solarSystem.Id64);
            }
        }

        if (solarSystem.Stations != null)
        {
            foreach (var station in solarSystem.Stations)
            {
                await AddStationToDataTable(station, solarSystem);
            }
        }
    }

    private async Task AddBody(Body body, long solarSystemId)
    {
        await AddBodyToDataTable(body, solarSystemId);

        if (body.Stations != null)
        {
            foreach (var station in body.Stations)
            {
                await AddStationToDataTable(station, body);
                await AddStationEconomies(station);
                await AddStationServices(station);
            }
        }
    }

    SemaphoreSlim solarSystemMutex = new SemaphoreSlim(1, 1);
    private async Task AddSolarSystemToDataTable(SolarSystem solarSystem)
    {
        var row = SolarSystems.NewRow();
        row["Id"] = solarSystem.Id64;
        row["Name"] = ValueOrDbNull(solarSystem.Name);
        row["X"] = ValueOrDbNull(solarSystem.Coordinates?.X);
        row["Y"] = ValueOrDbNull(solarSystem.Coordinates?.Y);
        row["Z"] = ValueOrDbNull(solarSystem.Coordinates?.Z);
        row["AllegianceId"] = ValueOrDbNull(await AllegianceAccess.GetId(solarSystem.Allegiance));
        row["GovernmentId"] = ValueOrDbNull(await GovernmentAccess.GetId(solarSystem.Government));
        row["PrimaryEconomyId"] = ValueOrDbNull(await EconomyAccess.GetId(solarSystem.PrimaryEconomy));
        row["SecondaryEconomyId"] = ValueOrDbNull(await EconomyAccess.GetId(solarSystem.SecondaryEconomy));
        row["SecurityId"] = ValueOrDbNull(await SecurityAccess.GetId(solarSystem.Security));
        row["Population"] = solarSystem.Population ?? 0;
        row["BodyCount"] = solarSystem.BodyCount ?? 0;
        row["ControllingFactionId"] = ValueOrDbNull(solarSystem.ControllingFaction != null ? FactionNameToId[solarSystem.ControllingFaction.Name!] : null);
        row["Date"] = ValueOrDbNull(solarSystem.Date);
        row["PowerStateTimestamp"] = ValueOrDbNull(solarSystem.Timestamps?.PowerState);
        row["PowersTimestamp"] = ValueOrDbNull(solarSystem.Timestamps?.Powers);
        row["ControllingPowerTimestamp"] = ValueOrDbNull(solarSystem.Timestamps?.ControllingPower);
        row["ControllingPowerId"] = ValueOrDbNull(await PowerAccess.GetId(solarSystem.ControllingPower));
        row["PowerStateId"] = ValueOrDbNull(await PowerStateAccess.GetId(solarSystem.PowerState));
        row["PowerStateControlProgress"] = ValueOrDbNull(solarSystem.PowerStateControlProgress ?? 0);
        row["PowerStateReinforcement"] = ValueOrDbNull(solarSystem.PowerStateReinforcement ?? 0);
        row["PowerStateUndermining"] = ValueOrDbNull(solarSystem.PowerStateUndermining ?? 0);
        SolarSystems.Rows.Add(row);
    }

    private async Task AddBodyToDataTable(Body body, long solarSystemId)
    {
        var row = Bodies.NewRow();
        row["Id"] = body.Id64;
        row["BodyId"] = body.BodyId;
        row["Name"] = ValueOrDbNull(body.Name);
        row["BodyTypeId"] = ValueOrDbNull(await BodyTypeAccess.GetId(body.Type));
        row["BodySubTypeId"] = ValueOrDbNull(await BodySubTypeAccess.GetId(body.SubType));
        row["DistanceToArrival"] = ValueOrDbNull(body.DistanceToArrival);
        row["Mainstar"] = body.MainStar ?? false;
        row["Age"] = ValueOrDbNull(body.Age);
        row["SpectralClassId"] = ValueOrDbNull(await SpectralClassAccess.GetId(body.SpectralClass));
        row["LuminosityId"] = ValueOrDbNull(await LuminosityAccess.GetId(body.Luminosity));
        row["AbsoluteMagnitude"] = ValueOrDbNull(body.AbsoluteMagnitude);
        row["SolarMasses"] = ValueOrDbNull(body.SolarMasses);
        row["SurfaceTemperature"] = ValueOrDbNull(body.SurfaceTemperature);
        row["RotationalPeriod"] = ValueOrDbNull(body.RotationalPeriod);
        row["RotationalPeriodTidallyLocked"] = body.RotationalPeriodTidallyLocked ?? false;
        row["AxialTilt"] = ValueOrDbNull(body.AxialTilt);
        row["OrbitalPeriod"] = ValueOrDbNull(body.OrbitalPeriod);
        row["SemiMajorAxis"] = ValueOrDbNull(body.SemiMajorAxis);
        row["OrbitalEccentricity"] = ValueOrDbNull(body.OrbitalEccentricity);
        row["OrbitalInclination"] = ValueOrDbNull(body.OrbitalInclination);
        row["ArgOfPeriapsis"] = ValueOrDbNull(body.ArgOfPeriapsis);
        row["MeanAnomaly"] = ValueOrDbNull(body.MeanAnomaly);
        row["AscendingNode"] = ValueOrDbNull(body.AscendingNode);
        row["IsLandable"] = body.IsLandable ?? false;
        row["Gravity"] = ValueOrDbNull(body.Gravity);
        row["EarthMasses"] = ValueOrDbNull(body.EarthMasses);
        row["Radius"] = ValueOrDbNull(body.Radius);
        row["SurfacePressure"] = ValueOrDbNull(body.SurfacePressure);
        row["VolcanismTypeId"] = ValueOrDbNull(await VolcanismTypeAccess.GetId(body.VolcanismType));
        row["AtmosphereTypeId"] = ValueOrDbNull(await AtmosphereTypeAccess.GetId(body.AtmosphereType));
        row["TerraformingStateId"] = ValueOrDbNull(await TerraformingStateAccess.GetId(body.TerraformingState));
        row["ReserveLevelId"] = ValueOrDbNull(await ReserveLevelAccess.GetId(body.ReserveLevel));
        row["UpdateTime"] = ValueOrDbNull(body.UpdateTime);
        row["DistanceToArrivalTimestamp"] = ValueOrDbNull(body.Timestamps?.DistanceToArrival);
        row["MeanAnomalyTimestamp"] = ValueOrDbNull(body.Timestamps?.MeanAnomaly);
        row["AscendingNodeTimestamp"] = ValueOrDbNull(body.Timestamps?.AscendingNode);
        row["SolarSystemId"] = solarSystemId;
        Bodies.Rows.Add(row);
    }

    private async Task AddStationToDataTable(Station station, SolarSystem solarSystem)
    {
        var row = Stations.NewRow();
        await FillStationRows(station, row);
        row["solarSystemId"] = solarSystem.Id64;
        Stations.Rows.Add(row);
    }

    private async Task AddStationToDataTable(Station station, Body body)
    {
        var row = Stations.NewRow();
        await FillStationRows(station, row);
        row["bodyId"] = body.Id64;
        Stations.Rows.Add(row);
    }

    private async Task FillStationRows(Station station, DataRow row)
    {
        if (station.ControllingFaction != null && !FactionNameToId.ContainsKey(station.ControllingFaction))
        {
            // station's faction is unknown, add it to the placeholder faction
            await AddStationsMappedToPlaceholderFaction(station);
            station.ControllingFaction = "PlaceHolder";
        }

        row["Id"] = station.Id;
        row["Name"] = ValueOrDbNull(station.Name);
        row["UpdateTime"] = ValueOrDbNull(station.UpdateTime);
        row["RealName"] = ValueOrDbNull(station.RealName);
        row["ControllingFactionId"] = ValueOrDbNull(station.ControllingFaction != null ? FactionNameToId[station.ControllingFaction] : null);
        row["ControllingFactionStateId"] = ValueOrDbNull(await FactionStateAccess.GetId(station.ControllingFactionState));
        row["DistanceToArrival"] = ValueOrDbNull(station.DistanceToArrival);
        row["PrimaryEconomyId"] = ValueOrDbNull(await EconomyAccess.GetId(station.PrimaryEconomy));
        row["GovernmentId"] = ValueOrDbNull(await GovernmentAccess.GetId(station.Government));
        row["StationTypeId"] = ValueOrDbNull(await StationTypeAccess.GetId(station.Type));
        row["StateId"] = ValueOrDbNull(await StationStateAccess.GetId(station.State));
        row["LargePads"] = ValueOrDbNull(station.LandingPads?.Large ?? 0);
        row["MediumPads"] = ValueOrDbNull(station.LandingPads?.Medium ?? 0);
        row["SmallPads"] = ValueOrDbNull(station.LandingPads?.Small ?? 0);
        row["MarketUpdateTime"] = ValueOrDbNull(station.Market?.UpdateTime);
        row["CarrierDockingAccessId"] = ValueOrDbNull(await CarrierDockingAccessAccess.GetId(station.CarrierDockingAccess));
        row["CarrierName"] = ValueOrDbNull(station.CarrierName);
        row["ShipyardUpdateTime"] = ValueOrDbNull(station.Shipyard?.UpdateTime);
        row["OutfittingUpdateTime"] = ValueOrDbNull(station.Outfitting?.UpdateTime);
        row["AllegianceId"] = ValueOrDbNull(await AllegianceAccess.GetId(station.Allegiance));
        row["Latitude"] = ValueOrDbNull(station.Latitude);
        row["Longitude"] = ValueOrDbNull(station.Longitude);
    }

    private async Task AddStationsMappedToPlaceholderFaction(Station station)
    {
        var row = StationsMappedToPlaceholderFaction.NewRow();
        row["StationId"] = station.Id;
        row["FactionName"] = station.ControllingFaction;
        StationsMappedToPlaceholderFaction.Rows.Add(row);

        await Task.CompletedTask;
    }

    private async Task AddStationEconomies(Station station)
    {
        if (station.Economies != null)
        {
            foreach (var economy in station.Economies)
            {
                var row = StationEconomies.NewRow();
                row["StationId"] = station.Id;
                row["EconomyId"] = await EconomyAccess.GetId(economy.Key);
                row["Proportion"] = economy.Value;
                StationEconomies.Rows.Add(row);
            }
        }
    }

    private async Task AddStationServices(Station station)
    {
        if (station.Services != null)
        {
            foreach (var service in station.Services)
            {
                var row = StationServices.NewRow();
                row["StationId"] = station.Id;
                row["ServiceId"] = await ServiceAccess.GetId(service);
                StationServices.Rows.Add(row);
            }
        }
    }

    private async Task AddFaction(Faction faction)
    {
        if (faction.Name == null || FactionNameToId.ContainsKey(faction.Name))
            return;

        var row = Factions.NewRow();
        row["Id"] = nextFactionId;
        row["Name"] = faction.Name;
        row["Allegiance"] = ValueOrDbNull(await AllegianceAccess.GetId(faction.Allegiance));
        row["Government"] = ValueOrDbNull(await GovernmentAccess.GetId(faction.Government));
        Factions.Rows.Add(row);

        FactionNameToId[faction.Name] = nextFactionId;

        nextFactionId++;
    }

    private async Task AddSolarSystemFaction(SolarSystem system, Faction faction)
    {
        var row = SolarSystemFactions.NewRow();
        row["SolarSystemId"] = system.Id64;
        row["FactionId"] = FactionNameToId[faction.Name!];
        row["Influence"] = faction.Influence ?? 0;
        row["FactionStateId"] = ValueOrDbNull(await FactionStateAccess.GetId(faction.State));
        SolarSystemFactions.Rows.Add(row);
    }

    private dynamic ValueOrDbNull(object? value)
    {
        return value ?? DBNull.Value;
    }

    public async Task WriteSolarSystems()
    {
        try
        {
            var count = SolarSystems.Rows.Count;
            Console.Write($"writing {count} systems .. ");

            using var connection = DbAccess.GetOpenConnection();

            var transaction = connection.BeginTransaction();

            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);

            bulkCopy.DestinationTableName = "Body";
            await bulkCopy.WriteToServerAsync(Bodies);

            bulkCopy.DestinationTableName = "Faction";
            await bulkCopy.WriteToServerAsync(Factions);

            bulkCopy.DestinationTableName = "SolarSystem";
            await bulkCopy.WriteToServerAsync(SolarSystems);

            bulkCopy.DestinationTableName = "SolarSystemFaction";
            await bulkCopy.WriteToServerAsync(SolarSystemFactions);

            bulkCopy.DestinationTableName = "Station";
            await bulkCopy.WriteToServerAsync(Stations);

            bulkCopy.DestinationTableName = "StationEconomies";
            await bulkCopy.WriteToServerAsync(StationEconomies);

            bulkCopy.DestinationTableName = "StationServices";
            await bulkCopy.WriteToServerAsync(StationServices);

            bulkCopy.DestinationTableName = "StationsMappedToPlaceholderFaction";
            await bulkCopy.WriteToServerAsync(StationsMappedToPlaceholderFaction);

            transaction.Commit();

            Console.WriteLine("Done!");
            Reset();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during bulk write: {ex.Message}");

            throw;
        }
    }

    private DataTable SetupSolarSystemDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("X", typeof(int));
        table.Columns.Add("Y", typeof(int));
        table.Columns.Add("Z", typeof(int));
        table.Columns.Add("AllegianceId", typeof(long));
        table.Columns.Add("GovernmentId", typeof(long));
        table.Columns.Add("PrimaryEconomyId", typeof(long));
        table.Columns.Add("SecondaryEconomyId", typeof(long));
        table.Columns.Add("SecurityId", typeof(long));
        table.Columns.Add("Population", typeof(long));
        table.Columns.Add("BodyCount", typeof(int));
        table.Columns.Add("ControllingFactionId", typeof(long));
        table.Columns.Add("Date", typeof(string));
        table.Columns.Add("PowerStateTimestamp", typeof(string));
        table.Columns.Add("PowersTimestamp", typeof(string));
        table.Columns.Add("ControllingPowerTimestamp", typeof(string));
        table.Columns.Add("ControllingPowerId", typeof(long));
        table.Columns.Add("PowerStateId", typeof(long));
        table.Columns.Add("PowerStateControlProgress", typeof(double));
        table.Columns.Add("PowerStateReinforcement", typeof(double));
        table.Columns.Add("PowerStateUndermining", typeof(double));

        return table;
    }

    private DataTable SetupFactionDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Allegiance", typeof(string));
        table.Columns.Add("Government", typeof(string));

        return table;
    }

    private DataTable SetupSolarSystemFactionDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("SolarSystemId", typeof(long));
        table.Columns.Add("FactionId", typeof(long));
        table.Columns.Add("Influence", typeof(double));
        table.Columns.Add("FactionStateId", typeof(long));

        return table;
    }

    private DataTable SetupBodiesDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("BodyId", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("BodyTypeId", typeof(long));
        table.Columns.Add("BodySubTypeId", typeof(long));
        table.Columns.Add("DistanceToArrival", typeof(double));
        table.Columns.Add("Mainstar", typeof(bool));
        table.Columns.Add("Age", typeof(int));
        table.Columns.Add("SpectralClassId", typeof(long));
        table.Columns.Add("LuminosityId", typeof(long));
        table.Columns.Add("AbsoluteMagnitude", typeof(double));
        table.Columns.Add("SolarMasses", typeof(double));
        table.Columns.Add("SurfaceTemperature", typeof(double));
        table.Columns.Add("RotationalPeriod", typeof(double));
        table.Columns.Add("RotationalPeriodTidallyLocked", typeof(bool));
        table.Columns.Add("AxialTilt", typeof(double));
        table.Columns.Add("OrbitalPeriod", typeof(double));
        table.Columns.Add("SemiMajorAxis", typeof(double));
        table.Columns.Add("OrbitalEccentricity", typeof(double));
        table.Columns.Add("OrbitalInclination", typeof(double));
        table.Columns.Add("ArgOfPeriapsis", typeof(double));
        table.Columns.Add("MeanAnomaly", typeof(double));
        table.Columns.Add("AscendingNode", typeof(double));
        table.Columns.Add("IsLandable", typeof(bool));
        table.Columns.Add("Gravity", typeof(double));
        table.Columns.Add("EarthMasses", typeof(double));
        table.Columns.Add("Radius", typeof(double));
        table.Columns.Add("SurfacePressure", typeof(double));
        table.Columns.Add("VolcanismTypeId", typeof(long));
        table.Columns.Add("AtmosphereTypeId", typeof(long));
        table.Columns.Add("TerraformingStateId", typeof(long));
        table.Columns.Add("ReserveLevelId", typeof(long));
        table.Columns.Add("UpdateTime", typeof(string));
        table.Columns.Add("DistanceToArrivalTimestamp", typeof(string));
        table.Columns.Add("MeanAnomalyTimestamp", typeof(string));
        table.Columns.Add("AscendingNodeTimestamp", typeof(string));
        table.Columns.Add("SolarSystemId", typeof(long));
        return table;
    }

    private DataTable SetupStationsDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("UpdateTime", typeof(string));
        table.Columns.Add("RealName", typeof(string));
        table.Columns.Add("ControllingFactionId", typeof(long));
        table.Columns.Add("ControllingFactionStateId", typeof(long));
        table.Columns.Add("DistanceToArrival", typeof(double));
        table.Columns.Add("PrimaryEconomyId", typeof(long));
        table.Columns.Add("GovernmentId", typeof(long));
        table.Columns.Add("StationTypeId", typeof(long));
        table.Columns.Add("StateId", typeof(long));
        table.Columns.Add("LargePads", typeof(int));
        table.Columns.Add("MediumPads", typeof(int));
        table.Columns.Add("SmallPads", typeof(int));
        table.Columns.Add("MarketUpdateTime", typeof(string));
        table.Columns.Add("CarrierDockingAccessId", typeof(long));
        table.Columns.Add("CarrierName", typeof(string));
        table.Columns.Add("ShipyardUpdateTime", typeof(string));
        table.Columns.Add("OutfittingUpdateTime", typeof(string));
        table.Columns.Add("AllegianceId", typeof(long));
        table.Columns.Add("Latitude", typeof(double));
        table.Columns.Add("Longitude", typeof(double));
        table.Columns.Add("BodyId", typeof(long));
        table.Columns.Add("SolarSystemId", typeof(long));
        return table;
    }

    private DataTable SetupStationEconomiesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("StationId", typeof(long));
        table.Columns.Add("EconomyId", typeof(long));
        table.Columns.Add("Proportion", typeof(double));
        return table;
    }

    private DataTable SetupStationServicesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("StationId", typeof(long));
        table.Columns.Add("ServiceId", typeof(long));
        return table;
    }

    private DataTable SetupStationsMappedToPlaceholderFactionDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("StationId", typeof(long));
        table.Columns.Add("FactionName", typeof(string));
        return table;
    }
}
