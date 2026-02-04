using System.Data;

namespace EliteDangerousChecker.Database.Shared;
internal static class DataTables
{
    public static DataTable SetupSolarSystemDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("X", typeof(float));
        table.Columns.Add("Y", typeof(float));
        table.Columns.Add("Z", typeof(float));
        table.Columns.Add("AllegianceId", typeof(long));
        table.Columns.Add("GovernmentId", typeof(long));
        table.Columns.Add("PrimaryEconomyId", typeof(long));
        table.Columns.Add("SecondaryEconomyId", typeof(long));
        table.Columns.Add("SecurityId", typeof(long));
        table.Columns.Add("Population", typeof(long));
        table.Columns.Add("BodyCount", typeof(int));
        table.Columns.Add("ControllingFactionId", typeof(long));
        table.Columns.Add("Date", typeof(long));
        table.Columns.Add("PowerStateTimestamp", typeof(long));
        table.Columns.Add("PowersTimestamp", typeof(long));
        table.Columns.Add("ControllingPowerTimestamp", typeof(long));
        table.Columns.Add("ControllingPowerId", typeof(long));
        table.Columns.Add("PowerStateId", typeof(long));
        table.Columns.Add("PowerStateControlProgress", typeof(double));
        table.Columns.Add("PowerStateReinforcement", typeof(double));
        table.Columns.Add("PowerStateUndermining", typeof(double));
        table.Columns.Add("HasSectorName", typeof(bool));
        table.Columns.Add("SectorSuffixId", typeof(long));
        table.Columns.Add("SectorPostfixId", typeof(long));

        return table;
    }

    public static DataTable SetupSectorPrefixDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("SolarSystemId", typeof(long));
        table.Columns.Add("Sequence", typeof(int));
        table.Columns.Add("SectorPrefixWordId", typeof(long));
        table.Columns.Add("SectorPrefixNumber", typeof(int));
        table.Columns.Add("StartWithDash", typeof(bool));
        table.Columns.Add("StartWithJ", typeof(bool));
        return table;
    }

    public static DataTable SetupFactionDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("AllegianceId", typeof(long));
        table.Columns.Add("GovernmentId", typeof(long));

        return table;
    }

    public static DataTable SetupSolarSystemFactionDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("SolarSystemId", typeof(long));
        table.Columns.Add("FactionId", typeof(long));
        table.Columns.Add("Influence", typeof(double));
        table.Columns.Add("FactionStateId", typeof(long));

        return table;
    }

    public static DataTable SetupSolarSystemPowerDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("SolarSystemId", typeof(long));
        table.Columns.Add("PowerId", typeof(long));
        return table;
    }

    public static DataTable SetupSolarSystemPowerConflictProgressDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("SolarSystemId", typeof(long));
        table.Columns.Add("PowerId", typeof(long));
        table.Columns.Add("Progress", typeof(double));
        return table;
    }

    public static DataTable SetupBodiesDataTable()
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
        table.Columns.Add("SolarSystemNameIsPrefix", typeof(bool));
        table.Columns.Add("Carbon", typeof(double));
        table.Columns.Add("Iron", typeof(double));
        table.Columns.Add("Nickel", typeof(double));
        table.Columns.Add("Niobium", typeof(double));
        table.Columns.Add("Phosphorus", typeof(double));
        table.Columns.Add("Sulphur", typeof(double));
        table.Columns.Add("Tellurium", typeof(double));
        table.Columns.Add("Tungsten", typeof(double));
        table.Columns.Add("Vanadium", typeof(double));
        table.Columns.Add("Zinc", typeof(double));
        table.Columns.Add("Zirconium", typeof(double));
        table.Columns.Add("Germanium", typeof(double));
        table.Columns.Add("Manganese", typeof(double));
        table.Columns.Add("Molybdenum", typeof(double));
        table.Columns.Add("Selenium", typeof(double));
        table.Columns.Add("Yttrium", typeof(double));
        table.Columns.Add("Cadmium", typeof(double));
        table.Columns.Add("Ruthenium", typeof(double));
        table.Columns.Add("Arsenic", typeof(double));
        table.Columns.Add("Antimony", typeof(double));
        table.Columns.Add("Chromium", typeof(double));
        table.Columns.Add("Tin", typeof(double));
        table.Columns.Add("Mercury", typeof(double));
        table.Columns.Add("Technetium", typeof(double));
        table.Columns.Add("Polonium", typeof(double));
        return table;
    }

    public static DataTable SetupStationsDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("UpdateTime", typeof(long));
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
        table.Columns.Add("MarketUpdateTime", typeof(long));
        table.Columns.Add("CarrierDockingAccessId", typeof(long));
        table.Columns.Add("CarrierName", typeof(string));
        table.Columns.Add("ShipyardUpdateTime", typeof(long));
        table.Columns.Add("OutfittingUpdateTime", typeof(long));
        table.Columns.Add("AllegianceId", typeof(long));
        table.Columns.Add("Latitude", typeof(double));
        table.Columns.Add("Longitude", typeof(double));
        table.Columns.Add("BodyId", typeof(long));
        table.Columns.Add("SolarSystemId", typeof(long));
        return table;
    }

    public static DataTable SetupStationCommoditiesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("StationId", typeof(long));
        table.Columns.Add("CommodityId", typeof(long));
        table.Columns.Add("Demand", typeof(int));
        table.Columns.Add("Supply", typeof(int));
        table.Columns.Add("BuyPrice", typeof(int));
        table.Columns.Add("SellPrice", typeof(int));
        return table;
    }

    public static DataTable SetupStationEconomiesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("StationId", typeof(long));
        table.Columns.Add("EconomyId", typeof(long));
        table.Columns.Add("Proportion", typeof(double));
        return table;
    }

    public static DataTable SetupStationServicesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("StationId", typeof(long));
        table.Columns.Add("ServiceId", typeof(long));
        return table;
    }

    public static DataTable SetupStationsMappedToPlaceholderFactionDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("StationId", typeof(long));
        table.Columns.Add("FactionName", typeof(string));
        return table;
    }

    public static DataTable SetupRingsDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Id", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("BodyNameIsPrefix", typeof(bool));
        table.Columns.Add("BodyId", typeof(long));
        table.Columns.Add("RingTypeId", typeof(long));
        table.Columns.Add("Mass", typeof(double));
        table.Columns.Add("InnerRadius", typeof(double));
        table.Columns.Add("OuterRadius", typeof(double));
        return table;
    }

    public static DataTable SetupBodySignalTypesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("BodyId", typeof(long));
        table.Columns.Add("SignalTypeId", typeof(long));
        table.Columns.Add("Number", typeof(int));
        return table;
    }

    public static DataTable SetupBodySignalGenusesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("BodyId", typeof(long));
        table.Columns.Add("SignalGenusId", typeof(long));
        return table;
    }

    public static DataTable SetupRingSignalTypesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("RingId", typeof(long));
        table.Columns.Add("SignalTypeId", typeof(long));
        table.Columns.Add("Number", typeof(int));
        return table;
    }

    public static DataTable SetupRingSignalGenusesDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("RingId", typeof(long));
        table.Columns.Add("SignalGenusId", typeof(long));
        return table;
    }
}
