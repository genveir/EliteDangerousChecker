using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

public class Body
{
    [JsonPropertyName("id64")]
    public long Id64 { get; set; }

    [JsonPropertyName("bodyId")]
    public int BodyId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("subType")]
    public string? SubType { get; set; }

    [JsonPropertyName("distanceToArrival")]
    public double? DistanceToArrival { get; set; }

    [JsonPropertyName("mainStar")]
    public bool? MainStar { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("spectralClass")]
    public string? SpectralClass { get; set; }

    [JsonPropertyName("luminosity")]
    public string? Luminosity { get; set; }

    [JsonPropertyName("absoluteMagnitude")]
    public double? AbsoluteMagnitude { get; set; }

    [JsonPropertyName("solarMasses")]
    public double? SolarMasses { get; set; }

    [JsonPropertyName("solarRadius")]
    public double? SolarRadius { get; set; }

    [JsonPropertyName("surfaceTemperature")]
    public double? SurfaceTemperature { get; set; }

    [JsonPropertyName("rotationalPeriod")]
    public double? RotationalPeriod { get; set; }

    [JsonPropertyName("rotationalPeriodTidallyLocked")]
    public bool? RotationalPeriodTidallyLocked { get; set; }

    [JsonPropertyName("axialTilt")]
    public double? AxialTilt { get; set; }

    [JsonPropertyName("parents")]
    public Parent[]? Parents { get; set; }

    [JsonPropertyName("orbitalPeriod")]
    public double? OrbitalPeriod { get; set; }

    [JsonPropertyName("semiMajorAxis")]
    public double? SemiMajorAxis { get; set; }

    [JsonPropertyName("orbitalEccentricity")]
    public double? OrbitalEccentricity { get; set; }

    [JsonPropertyName("orbitalInclination")]
    public double? OrbitalInclination { get; set; }

    [JsonPropertyName("argOfPeriapsis")]
    public double? ArgOfPeriapsis { get; set; }

    [JsonPropertyName("meanAnomaly")]
    public double? MeanAnomaly { get; set; }

    [JsonPropertyName("ascendingNode")]
    public double? AscendingNode { get; set; }

    [JsonPropertyName("isLandable")]
    public bool? IsLandable { get; set; }

    [JsonPropertyName("gravity")]
    public double? Gravity { get; set; }

    [JsonPropertyName("earthMasses")]
    public double? EarthMasses { get; set; }

    [JsonPropertyName("radius")]
    public double? Radius { get; set; }

    [JsonPropertyName("surfacePressure")]
    public double? SurfacePressure { get; set; }

    [JsonPropertyName("volcanismType")]
    public string? VolcanismType { get; set; }

    [JsonPropertyName("atmosphereType")]
    public string? AtmosphereType { get; set; }

    [JsonPropertyName("atmosphereComposition")]
    public Dictionary<string, double>? AtmosphereComposition { get; set; }

    [JsonPropertyName("solidComposition")]
    public Dictionary<string, double>? SolidComposition { get; set; }

    [JsonPropertyName("terraformingState")]
    public string? TerraformingState { get; set; }

    [JsonPropertyName("materials")]
    public Dictionary<string, double>? Materials { get; set; }

    [JsonPropertyName("signals")]
    public Signals? Signals { get; set; }

    [JsonPropertyName("rings")]
    public List<Ring>? Rings { get; set; }

    [JsonPropertyName("reserveLevel")]
    public string? ReserveLevel { get; set; }

    [JsonPropertyName("stations")]
    public List<Station>? Stations { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }

    [JsonPropertyName("belts")]
    public List<Belt>? Belts { get; set; }

    [JsonPropertyName("timestamps")]
    public BodyTimeStamps? Timestamps { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Unmapped { get; set; }

    public void UnmappedFieldsRecursive(List<string> unmappedFields, string prefix)
    {
        if (Unmapped != null && Unmapped.Count != 0)
        {
            unmappedFields.AddRange(Unmapped.Keys.Select(s => prefix + s));
        }

        Signals?.UnmappedFieldsRecursive(unmappedFields, prefix + "signals.");

        if (Rings != null)
        {
            for (int n = 0; n < Rings.Count; n++) { Rings[n].UnmappedFieldsRecursive(unmappedFields, prefix + $"rings[{n}]."); }
        }

        if (Stations != null)
        {
            for (int n = 0; n < Stations.Count; n++) { Stations[n].UnmappedFieldsRecursive(unmappedFields, prefix + $"stations[{n}]."); }
        }

        if (Belts != null)
        {
            for (int n = 0; n < Belts.Count; n++) { Belts[n].UnmappedFieldsRecursive(unmappedFields, prefix + $"belts[{n}]."); }
        }

        Timestamps?.UnmappedFieldsRecursive(unmappedFields, prefix + "timestamps.");
    }
}
