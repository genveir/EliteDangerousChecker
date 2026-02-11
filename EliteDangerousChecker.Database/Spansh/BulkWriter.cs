using Dapper;
using EliteDangerousChecker.Database.Shared;
using EliteDangerousChecker.Database.Shared.ImmediateWrite;
using EliteDangerousChecker.Database.Spansh.DumpModel;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EliteDangerousChecker.Database.Spansh;
public class BulkWriter
{
    DataTable SolarSystems { get; set; }

    DataTable SectorPrefixes { get; set; }

    DataTable Factions { get; set; }
    long nextFactionId { get; set; }
    Dictionary<string, long> FactionNameToId = new();

    DataTable SolarSystemFactions { get; set; }
    DataTable SolarSystemPowers { get; set; }
    DataTable SolarSystemPowerConflictProgress { get; set; }

    DataTable Bodies { get; set; }
    DataTable Rings { get; set; }
    long nextRingId { get; set; }

    DataTable BodySignalTypes { get; set; }
    DataTable BodySignalGenuses { get; set; }

    DataTable RingSignalTypes { get; set; }
    DataTable RingSignalGenuses { get; set; }

    DataTable Stations { get; set; }
    DataTable StationCommodities { get; set; }
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
        SolarSystemPowers = DataTables.SetupSolarSystemPowerDataTable();
        SolarSystemPowerConflictProgress = DataTables.SetupSolarSystemPowerConflictProgressDataTable();
        Bodies = DataTables.SetupBodiesDataTable();
        Rings = DataTables.SetupRingsDataTable();
        BodySignalTypes = DataTables.SetupBodySignalTypesDataTable();
        BodySignalGenuses = DataTables.SetupBodySignalGenusesDataTable();
        RingSignalTypes = DataTables.SetupRingSignalTypesDataTable();
        RingSignalGenuses = DataTables.SetupRingSignalGenusesDataTable();
        Stations = DataTables.SetupStationsDataTable();
        StationCommodities = DataTables.SetupStationCommoditiesDataTable();
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

        var existingLowestRingId = await connection.QueryFirstOrDefaultAsync<long?>("select min(Id) from Ring");
        nextRingId = existingLowestRingId.HasValue
            ? existingLowestRingId.Value - 1
            : -1;

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

        if (solarSystem.Bodies == null || solarSystem.Bodies.Count == 0)
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

        if (solarSystem.Powers != null)
        {
            foreach (var power in solarSystem.Powers)
            {
                await AddSolarSystemPower(solarSystem, power);
            }
        }

        if (solarSystem.PowerConflictProgress != null)
        {
            foreach (var powerConflict in solarSystem.PowerConflictProgress)
            {
                await AddSolarSystemPowerConflictProgress(solarSystem, powerConflict);
            }
        }

        var (prefixWords, suffix, postfix) = ParseName(solarSystem);

        await AddSolarSystemToDataTable(solarSystem, suffix, postfix);

        await AddSectorPrefixToDataTable(solarSystem, prefixWords);

        foreach (var body in solarSystem.Bodies)
        {
            await AddBody(body, solarSystem);
        }

        if (solarSystem.Stations != null)
        {
            foreach (var station in solarSystem.Stations)
            {
                await AddStationToDataTable(station, solarSystem);
                await AddStationCommoditiesToDataTable(station.Market, station);
                await AddStationEconomiesToDataTable(station);
                await AddStationServicesToDataTable(station);
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
                await AddStationCommoditiesToDataTable(station.Market, station);
                await AddStationEconomiesToDataTable(station);
                await AddStationServicesToDataTable(station);
            }
        }

        if (body.Rings != null)
        {
            foreach (var ring in body.Rings)
            {
                await AddRingToDataTable(ring, body);

                if (ring.Signals != null)
                {
                    foreach (var signalType in ring.Signals.SignalTypes ?? [])
                    {
                        await AddRingSignalTypeToDataTable(ring, signalType);
                    }
                    foreach (var signalGenus in ring.Signals.Genuses ?? [])
                    {
                        await AddRingSignalGenusToDataTable(ring, signalGenus);
                    }
                }
            }
        }

        if (body.Signals != null)
        {
            foreach (var signalType in body.Signals.SignalTypes ?? [])
            {
                await AddBodySignalTypeToDataTable(body, signalType);
            }

            foreach (var signalGenus in body.Signals.Genuses ?? [])
            {
                await AddBodySignalGenusToDataTable(body, signalGenus);
            }
        }
    }

    private async Task AddSolarSystemToDataTable(SolarSystem solarSystem, string? suffix, string? postfix)
    {
        long? suffixId = suffix != null ? await SectorSuffixAccess.GetId(suffix) : null;
        long? postfixId = postfix != null ? await SectorPostfixAccess.GetId(postfix) : null;

        int? subSector = CalculateSubSector(solarSystem.Coordinates?.X, solarSystem.Coordinates?.Y, solarSystem.Coordinates?.Z);

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
        row["Date"] = ValueOrDbNull(OffsetToUnix(solarSystem.Date));
        row["PowerStateTimestamp"] = ValueOrDbNull(OffsetToUnix(solarSystem.Timestamps?.PowerState));
        row["PowersTimestamp"] = ValueOrDbNull(OffsetToUnix(solarSystem.Timestamps?.Powers));
        row["ControllingPowerTimestamp"] = ValueOrDbNull(OffsetToUnix(solarSystem.Timestamps?.ControllingPower));
        row["ControllingPowerId"] = ValueOrDbNull(await PowerAccess.GetId(solarSystem.ControllingPower));
        row["PowerStateId"] = ValueOrDbNull(await PowerStateAccess.GetId(solarSystem.PowerState));
        row["PowerStateControlProgress"] = ValueOrDbNull(solarSystem.PowerStateControlProgress ?? 0);
        row["PowerStateReinforcement"] = ValueOrDbNull(solarSystem.PowerStateReinforcement ?? 0);
        row["PowerStateUndermining"] = ValueOrDbNull(solarSystem.PowerStateUndermining ?? 0);
        row["SectorSuffixId"] = ValueOrDbNull(suffixId);
        row["SectorPostfixId"] = ValueOrDbNull(postfixId);
        row["SolarSystemRegionId"] = ValueOrDbNull(await SolarSystemRegionAccess.GetId(solarSystem.Coordinates?.X, solarSystem.Coordinates?.Y, solarSystem.Coordinates?.Z));
        row["SubSector"] = ValueOrDbNull(subSector);

        SolarSystems.Rows.Add(row);
    }

    private int CalculateSubSector(double? x, double? y, double? z)
    {
        var sectorX = (int)Math.Floor((x ?? 0) / 100.0d);
        var sectorY = (int)Math.Floor((y ?? 0) / 100.0d);
        var sectorZ = (int)Math.Floor((z ?? 0) / 100.0d);

        var subSectorX = (int)Math.Floor(((x ?? 0) - sectorX * 100) / 10.0d);
        var subSectorY = (int)Math.Floor(((y ?? 0) - sectorY * 100) / 10.0d);
        var subSectorZ = (int)Math.Floor(((z ?? 0) - sectorZ * 100) / 10.0d);

        return subSectorX * 100 + subSectorY * 10 + subSectorZ;
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
            bodyName = body.Name!.Substring(solarSystem.Name!.Length);
        }

        if ((bodyName?.Length ?? 0) > 16)
        {
            bodyName = bodyName![..16];
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
        row["UpdateTime"] = ValueOrDbNull(OffsetToUnix(body.UpdateTime));
        row["DistanceToArrivalTimestamp"] = ValueOrDbNull(OffsetToUnix(body.Timestamps?.DistanceToArrival));
        row["MeanAnomalyTimestamp"] = ValueOrDbNull(OffsetToUnix(body.Timestamps?.MeanAnomaly));
        row["AscendingNodeTimestamp"] = ValueOrDbNull(OffsetToUnix(body.Timestamps?.AscendingNode));
        row["SolarSystemId"] = solarSystem.Id64;
        row["SolarSystemNameIsPrefix"] = hasPrefix;
        row["Carbon"] = GetDictValueOrDbNull(body.Materials, "Carbon");
        row["Iron"] = GetDictValueOrDbNull(body.Materials, "Iron");
        row["Nickel"] = GetDictValueOrDbNull(body.Materials, "Nickel");
        row["Niobium"] = GetDictValueOrDbNull(body.Materials, "Niobium");
        row["Phosphorus"] = GetDictValueOrDbNull(body.Materials, "Phosphorus");
        row["Sulphur"] = GetDictValueOrDbNull(body.Materials, "Sulphur");
        row["Tellurium"] = GetDictValueOrDbNull(body.Materials, "Tellurium");
        row["Tungsten"] = GetDictValueOrDbNull(body.Materials, "Tungsten");
        row["Vanadium"] = GetDictValueOrDbNull(body.Materials, "Vanadium");
        row["Zinc"] = GetDictValueOrDbNull(body.Materials, "Zinc");
        row["Zirconium"] = GetDictValueOrDbNull(body.Materials, "Zirconium");
        row["Germanium"] = GetDictValueOrDbNull(body.Materials, "Germanium");
        row["Manganese"] = GetDictValueOrDbNull(body.Materials, "Manganese");
        row["Molybdenum"] = GetDictValueOrDbNull(body.Materials, "Molybdenum");
        row["Selenium"] = GetDictValueOrDbNull(body.Materials, "Selenium");
        row["Yttrium"] = GetDictValueOrDbNull(body.Materials, "Yttrium");
        row["Cadmium"] = GetDictValueOrDbNull(body.Materials, "Cadmium");
        row["Ruthenium"] = GetDictValueOrDbNull(body.Materials, "Ruthenium");
        row["Arsenic"] = GetDictValueOrDbNull(body.Materials, "Arsenic");
        row["Antimony"] = GetDictValueOrDbNull(body.Materials, "Antimony");
        row["Chromium"] = GetDictValueOrDbNull(body.Materials, "Chromium");
        row["Tin"] = GetDictValueOrDbNull(body.Materials, "Tin");
        row["Mercury"] = GetDictValueOrDbNull(body.Materials, "Mercury");
        row["Technetium"] = GetDictValueOrDbNull(body.Materials, "Technetium");
        row["Polonium"] = GetDictValueOrDbNull(body.Materials, "Polonium");
        Bodies.Rows.Add(row);
    }

    private async Task AddBodySignalTypeToDataTable(Body body, KeyValuePair<string, int> signalType)
    {
        var row = BodySignalTypes.NewRow();
        row["BodyId"] = body.Id64;
        row["SignalTypeId"] = ValueOrDbNull(await SignalTypeAccess.GetId(signalType.Key));
        row["Number"] = ValueOrDbNull(signalType.Value);
        BodySignalTypes.Rows.Add(row);
    }

    private async Task AddBodySignalGenusToDataTable(Body body, string genus)
    {
        var row = BodySignalGenuses.NewRow();
        row["BodyId"] = body.Id64;
        row["SignalGenusId"] = ValueOrDbNull(await SignalGenusAccess.GetId(genus));
        BodySignalGenuses.Rows.Add(row);
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
        row["UpdateTime"] = ValueOrDbNull(OffsetToUnix(station.UpdateTime));
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
        row["MarketUpdateTime"] = ValueOrDbNull(OffsetToUnix(station.Market?.UpdateTime));
        row["CarrierDockingAccessId"] = ValueOrDbNull(await CarrierDockingAccessAccess.GetId(station.CarrierDockingAccess));
        row["CarrierName"] = ValueOrDbNull(station.CarrierName);
        row["ShipyardUpdateTime"] = ValueOrDbNull(OffsetToUnix(station.Shipyard?.UpdateTime));
        row["OutfittingUpdateTime"] = ValueOrDbNull(OffsetToUnix(station.Outfitting?.UpdateTime));
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

    private async Task AddStationCommoditiesToDataTable(Market? market, Station station)
    {
        if (market?.Commodities == null)
            return;

        foreach (var commodity in market.Commodities)
        {
            var row = StationCommodities.NewRow();
            row["StationId"] = station.Id;
            row["CommodityId"] = ValueOrDbNull(await CommodityAccess.GetId(commodity.Name, commodity.Category));
            row["Demand"] = ValueOrDbNull(commodity.Demand);
            row["Supply"] = ValueOrDbNull(commodity.Supply);
            row["BuyPrice"] = ValueOrDbNull(commodity.BuyPrice);
            row["SellPrice"] = ValueOrDbNull(commodity.SellPrice);
            StationCommodities.Rows.Add(row);
        }
    }

    private async Task AddStationEconomiesToDataTable(Station station)
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

    private async Task AddStationServicesToDataTable(Station station)
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

    private async Task AddRingToDataTable(Ring ring, Body body)
    {
        var hasPrefix = ring.Name != null && ring.Name.StartsWith(body.Name!);

        string? ringName = ring.Name;
        if (hasPrefix)
        {
            ringName = ring.Name!.Substring(body.Name!.Length);
        }

        if (ringName != null && ringName.Length > 32)
        {
            ringName = ringName[..32];
        }

        if (ring.Id64 == null)
        {
            ring.Id64 = nextRingId--;
        }

        var row = Rings.NewRow();
        row["Id"] = ring.Id64;
        row["Name"] = ValueOrDbNull(ringName);
        row["BodyNameIsPrefix"] = hasPrefix;
        row["BodyId"] = body.Id64;
        row["RingTypeId"] = ValueOrDbNull(await RingTypeAccess.GetId(ring.Type));
        row["Mass"] = ValueOrDbNull(ring.Mass);
        row["InnerRadius"] = ValueOrDbNull(ring.InnerRadius);
        row["OuterRadius"] = ValueOrDbNull(ring.OuterRadius);
        Rings.Rows.Add(row);
    }

    private async Task AddRingSignalTypeToDataTable(Ring Ring, KeyValuePair<string, int> signalType)
    {
        var row = RingSignalTypes.NewRow();
        row["RingId"] = Ring.Id64;
        row["SignalTypeId"] = ValueOrDbNull(await SignalTypeAccess.GetId(signalType.Key));
        row["Number"] = ValueOrDbNull(signalType.Value);
        RingSignalTypes.Rows.Add(row);
    }

    private async Task AddRingSignalGenusToDataTable(Ring Ring, string genus)
    {
        var row = RingSignalGenuses.NewRow();
        row["RingId"] = Ring.Id64;
        row["SignalGenusId"] = ValueOrDbNull(await SignalGenusAccess.GetId(genus));
        RingSignalGenuses.Rows.Add(row);
    }

    private async Task AddFaction(Faction faction)
    {
        if (faction.Name == null || FactionNameToId.ContainsKey(faction.Name))
            return;

        if (faction.Name.Length > 64)
        {
            faction.Name = faction.Name[..64];
        }

        var row = Factions.NewRow();
        row["Id"] = nextFactionId;
        row["Name"] = faction.Name;
        row["AllegianceId"] = ValueOrDbNull(await AllegianceAccess.GetId(faction.Allegiance));
        row["GovernmentId"] = ValueOrDbNull(await GovernmentAccess.GetId(faction.Government));
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

    private async Task AddSolarSystemPower(SolarSystem system, string? power)
    {
        var row = SolarSystemPowers.NewRow();
        row["SolarSystemId"] = system.Id64;
        row["PowerId"] = ValueOrDbNull(await PowerAccess.GetId(power));
        SolarSystemPowers.Rows.Add(row);
    }

    private async Task AddSolarSystemPowerConflictProgress(SolarSystem system, PowerConflictProgress powerConflictProgress)
    {
        var row = SolarSystemPowerConflictProgress.NewRow();
        row["SolarSystemId"] = system.Id64;
        row["PowerId"] = ValueOrDbNull(await PowerAccess.GetId(powerConflictProgress.Power));
        row["Progress"] = ValueOrDbNull(powerConflictProgress.Progress ?? 0);
        SolarSystemPowerConflictProgress.Rows.Add(row);
    }

    private long? OffsetToUnix(string? offset) =>
        DbHelper.OffsetToUnix(offset);

    private dynamic GetDictValueOrDbNull<T>(Dictionary<string, T>? dict, string key) =>
        DbHelper.GetDictValueOrDbNull(dict, key);

    private dynamic ValueOrDbNull(object? value) =>
        DbHelper.ValueOrDbNull(value);

    public async Task WriteSolarSystems(bool writeToUpdateTables)
    {
        var schema = writeToUpdateTables ? "upd" : "dbo";

        try
        {
            var count = SolarSystems.Rows.Count;
            Console.Write($"writing {count} systems .. ");

            using var connection = DbAccess.GetOpenConnection();

            var transaction = connection.BeginTransaction();

            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);

            bulkCopy.DestinationTableName = $"{schema}.Body";
            await bulkCopy.WriteToServerAsync(Bodies);

            bulkCopy.DestinationTableName = $"{schema}.Ring";
            await bulkCopy.WriteToServerAsync(Rings);

            bulkCopy.DestinationTableName = $"{schema}.BodySignalType";
            await bulkCopy.WriteToServerAsync(BodySignalTypes);

            bulkCopy.DestinationTableName = $"{schema}.BodySignalGenus";
            await bulkCopy.WriteToServerAsync(BodySignalGenuses);

            bulkCopy.DestinationTableName = $"{schema}.RingSignalType";
            await bulkCopy.WriteToServerAsync(RingSignalTypes);

            bulkCopy.DestinationTableName = $"{schema}.RingSignalGenus";
            await bulkCopy.WriteToServerAsync(RingSignalGenuses);

            bulkCopy.DestinationTableName = $"{schema}.Faction";
            await bulkCopy.WriteToServerAsync(Factions);

            bulkCopy.DestinationTableName = $"{schema}.SolarSystem";
            await bulkCopy.WriteToServerAsync(SolarSystems);

            bulkCopy.DestinationTableName = $"{schema}.SectorPrefix";
            await bulkCopy.WriteToServerAsync(SectorPrefixes);

            bulkCopy.DestinationTableName = $"{schema}.SolarSystemFaction";
            await bulkCopy.WriteToServerAsync(SolarSystemFactions);

            bulkCopy.DestinationTableName = $"{schema}.SolarSystemPower";
            await bulkCopy.WriteToServerAsync(SolarSystemPowers);

            bulkCopy.DestinationTableName = $"{schema}.SolarSystemPowerConflictProgress";
            await bulkCopy.WriteToServerAsync(SolarSystemPowerConflictProgress);

            bulkCopy.DestinationTableName = $"{schema}.Station";
            await bulkCopy.WriteToServerAsync(Stations);

            bulkCopy.DestinationTableName = $"{schema}.StationCommodities";
            await bulkCopy.WriteToServerAsync(StationCommodities);

            bulkCopy.DestinationTableName = $"{schema}.StationEconomies";
            await bulkCopy.WriteToServerAsync(StationEconomies);

            bulkCopy.DestinationTableName = $"{schema}.StationServices";
            await bulkCopy.WriteToServerAsync(StationServices);

            bulkCopy.DestinationTableName = $"{schema}.StationsMappedToPlaceholderFaction";
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
