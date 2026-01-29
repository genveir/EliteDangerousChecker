using Dapper;
using EliteDangerousChecker.Database.Update.DumpModel;
using EliteDangerousChecker.Database.Update.ImmediateWrite;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EliteDangerousChecker.Database.Update;
public class BulkWriter
{
    DataTable SolarSystems { get; set; }

    DataTable SectorPrefixes { get; set; }

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

#pragma warning disable CS8618
    public BulkWriter()
    {
        Reset();
    }
#pragma warning restore

    private void Reset()
    {
        SolarSystems = DataTables.SetupSolarSystemDataTable();
        SectorPrefixes = DataTables.SetupSectorPrefixDataTable();
        Factions = DataTables.SetupFactionDataTable();
        SolarSystemFactions = DataTables.SetupSolarSystemFactionDataTable();
        Bodies = DataTables.SetupBodiesDataTable();
        Stations = DataTables.SetupStationsDataTable();
        StationEconomies = DataTables.SetupStationEconomiesDataTable();
        StationServices = DataTables.SetupStationServicesDataTable();
        StationsMappedToPlaceholderFaction = DataTables.SetupStationsMappedToPlaceholderFactionDataTable();
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

        var (prefixWords, suffix, postfix) = ParseName(solarSystem);

        await AddSolarSystemToDataTable(solarSystem, suffix, postfix);

        await AddSectorPrefixToDataTable(solarSystem, prefixWords);

        if (solarSystem.Bodies != null)
        {
            foreach (var body in solarSystem.Bodies)
            {
                await AddBody(body, solarSystem);
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

    private (string[] prefixWords, string? suffix, string? postfix) ParseName(SolarSystem solarSystem)
    {
        if (solarSystem.Name == null)
        {
            return (Array.Empty<string>(), null, null);
        }

        var nameParts = solarSystem.Name.Split(' ');
        if (nameParts.Length >= 3)
        {
            var suffix = nameParts[^2];
            var postfix = nameParts[^1];
            if (suffix.Length == 4 && suffix[2] == '-' &&
                postfix.Length <= 5 && postfix[1] >= '0' && postfix[1] <= '9')
            {
                return (nameParts[..^2], suffix, postfix);
            }
        }
        return (nameParts, null, null);
    }

    private async Task AddBody(Body body, SolarSystem solarSystem)
    {
        await AddBodyToDataTable(body, solarSystem);

        if (body.Stations != null)
        {
            foreach (var station in body.Stations)
            {
                await AddStationToDataTable(station, solarSystem, body);
                await AddStationEconomies(station);
                await AddStationServices(station);
            }
        }
    }

    private async Task AddSolarSystemToDataTable(SolarSystem solarSystem, string? suffix, string? postfix)
    {
        long? suffixId = suffix != null ? await SectorSuffixAccess.GetId(suffix) : null;
        long? postfixId = postfix != null ? await SectorPostfixAccess.GetId(postfix) : null;

        var row = SolarSystems.NewRow();
        row["Id"] = solarSystem.Id64;
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
        row["SectorSuffixId"] = ValueOrDbNull(suffixId);
        row["SectorPostfixId"] = ValueOrDbNull(postfixId);

        SolarSystems.Rows.Add(row);
    }

    private async Task AddSectorPrefixToDataTable(SolarSystem solarSystem, string[] prefixes)
    {
        int sequence = 0;

        for (int n = 0; n < prefixes.Length; n++)
        {
            var prefix = prefixes[n];

            if (prefix.Contains('-'))
            {
                var split = prefix.Split('-');

                await AddSectorPrefixToDataTable(solarSystem, split[0], sequence++, startWithDash: false);
                for (int i = 1; i < split.Length; i++)
                {
                    await AddSectorPrefixToDataTable(solarSystem, split[i], sequence++, startWithDash: true);
                }
            }
            else
            {
                await AddSectorPrefixToDataTable(solarSystem, prefix, sequence++, startWithDash: false);
            }
        }
    }

    private async Task AddSectorPrefixToDataTable(SolarSystem solarSystem, string prefix, int sequence, bool startWithDash)
    {
        bool startWithJ = false;
        bool isNumber = false;
        int prefixNumeric = 0;

        if (prefix.StartsWith('J') && prefix.Length > 1)
        {
            isNumber = int.TryParse(prefix[1..], out prefixNumeric);
            startWithJ = isNumber;
        }

        if (!startWithJ)
        {
            isNumber = int.TryParse(prefix, out prefixNumeric);
        }

        var row = SectorPrefixes.NewRow();
        row["SolarSystemId"] = solarSystem.Id64;
        row["Sequence"] = sequence;
        row["SectorPrefixWordId"] = ValueOrDbNull(isNumber ? null : await SectorPrefixWordAccess.GetId(prefix));
        row["SectorPrefixNumber"] = ValueOrDbNull(isNumber ? prefixNumeric : (int?)null);
        row["StartWithDash"] = startWithDash;
        row["StartWithJ"] = startWithJ;
        SectorPrefixes.Rows.Add(row);
    }

    private async Task AddBodyToDataTable(Body body, SolarSystem solarSystem)
    {
        var hasPrefix = body.Name != null && body.Name.StartsWith(solarSystem.Name!);

        string? bodyName = body.Name;
        if (hasPrefix)
        {
            bodyName = body.Name!.Substring(solarSystem.Name!.Length).Trim();
        }

        var row = Bodies.NewRow();
        row["Id"] = body.Id64;
        row["BodyId"] = body.BodyId;
        row["Name"] = ValueOrDbNull(bodyName);
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
        row["SolarSystemId"] = solarSystem.Id64;
        row["SolarSystemNameIsPrefix"] = hasPrefix;
        Bodies.Rows.Add(row);
    }

    private async Task AddStationToDataTable(Station station, SolarSystem solarSystem)
    {
        var row = Stations.NewRow();
        await FillStationRows(station, row);
        row["solarSystemId"] = solarSystem.Id64;
        Stations.Rows.Add(row);
    }

    private async Task AddStationToDataTable(Station station, SolarSystem solarSystem, Body body)
    {
        var row = Stations.NewRow();
        await FillStationRows(station, row);
        row["bodyId"] = body.Id64;
        row["solarSystemId"] = solarSystem.Id64;
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

            bulkCopy.DestinationTableName = "SectorPrefix";
            await bulkCopy.WriteToServerAsync(SectorPrefixes);

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
}
