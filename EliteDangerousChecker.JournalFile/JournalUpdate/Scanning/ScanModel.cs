using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
// { "timestamp":"2026-02-06T18:37:54Z", "event":"Scan", "ScanType":"AutoScan", "BodyName":"Flyooe Eohn TH-W c4-3 C", "BodyID":4, "Parents":[ {"Null":0} ], "StarSystem":"Flyooe Eohn TH-W c4-3", "SystemAddress":908486086698, "DistanceFromArrivalLS":355651.868576, "StarType":"M", "Subclass":5, "StellarMass":0.281250, "Radius":334951296.000000, "AbsoluteMagnitude":9.545364, "Age_MY":12454, "SurfaceTemperature":2812.000000, "Luminosity":"Va", "SemiMajorAxis":69262760877609.250000, "Eccentricity":0.363590, "OrbitalInclination":20.356314, "Periapsis":204.588956, "OrbitalPeriod":318704068660.736084, "AscendingNode":-158.328501, "MeanAnomaly":152.329877, "RotationPeriod":158045.708430, "AxialTilt":0.000000, "WasDiscovered":true, "WasMapped":false, "WasFootfalled":false }
// { "timestamp":"2026-01-10T14:31:43Z", "event":"Scan", "ScanType":"Detailed", "BodyName":"Preae Theia UW-T b30-1 7", "BodyID":8, "Parents":[ {"Star":0} ], "StarSystem":"Preae Theia UW-T b30-1", "SystemAddress":2915343278857, "DistanceFromArrivalLS":2591.487372, "TidalLock":false, "TerraformState":"", "PlanetClass":"Icy body", "Atmosphere":"thin neon atmosphere", "AtmosphereType":"Neon", "AtmosphereComposition":[ { "Name":"Neon", "Percent":100.000000 } ], "Volcanism":"", "MassEM":0.354269, "Radius":5666755.000000, "SurfaceGravity":4.397177, "SurfaceTemperature":44.146687, "SurfacePressure":195.809845, "Landable":true, "Materials":[ { "Name":"sulphur", "Percent":23.495106 }, { "Name":"carbon", "Percent":19.756950 }, { "Name":"iron", "Percent":16.159782 }, { "Name":"phosphorus", "Percent":12.648741 }, { "Name":"nickel", "Percent":12.222581 }, { "Name":"chromium", "Percent":7.267591 }, { "Name":"selenium", "Percent":3.677183 }, { "Name":"zirconium", "Percent":1.876484 }, { "Name":"tin", "Percent":1.043036 }, { "Name":"yttrium", "Percent":0.965205 }, { "Name":"tungsten", "Percent":0.887334 } ], "Composition":{ "Ice":0.686985, "Rock":0.210434, "Metal":0.102581 }, "SemiMajorAxis":776274120807.647705, "Eccentricity":0.001237, "OrbitalInclination":1.261864, "Periapsis":310.455898, "OrbitalPeriod":574252319.335938, "AscendingNode":76.688329, "MeanAnomaly":131.281001, "RotationPeriod":111609.708170, "AxialTilt":0.241908, "WasDiscovered":false, "WasMapped":false, "WasFootfalled":false }
// { "timestamp":"2026-02-06T17:00:02Z", "event":"Scan", "ScanType":"Detailed", "BodyName":"Oochorrs KT-V c4-0 1", "BodyID":1, "Parents":[ {"Star":0} ], "StarSystem":"Oochorrs KT-V c4-0", "SystemAddress":84590465322, "DistanceFromArrivalLS":214.330805, "TidalLock":true, "TerraformState":"Terraformable", "PlanetClass":"High metal content body", "Atmosphere":"hot thick water atmosphere", "AtmosphereType":"Water", "AtmosphereComposition":[ { "Name":"Water", "Percent":99.997849 } ], "Volcanism":"", "MassEM":0.098376, "Radius":2962084.750000, "SurfaceGravity":4.468950, "SurfaceTemperature":649.889343, "SurfacePressure":532071.187500, "Landable":false, "Composition":{ "Ice":0.000000, "Rock":0.670953, "Metal":0.329047 }, "SemiMajorAxis":64252809286.117554, "Eccentricity":0.000032, "OrbitalInclination":-0.002564, "Periapsis":280.614743, "OrbitalPeriod":10964279.770851, "AscendingNode":152.542645, "MeanAnomaly":198.558496, "RotationPeriod":10964334.929184, "AxialTilt":0.017119, "WasDiscovered":true, "WasMapped":true, "WasFootfalled":false }
public class ScanModel
{
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("ScanType")]
    public string? ScanType { get; set; }

    [JsonPropertyName("BodyID")]
    public int? BodyId { get; set; }

    [JsonPropertyName("BodyName")]
    public string? BodyName { get; set; }

    [JsonPropertyName("DistanceFromArrivalLS")]
    public double? DistanceFromArrivalLS { get; set; }

    [JsonPropertyName("PlanetClass")]
    public string? PlanetClass { get; set; }

    [JsonPropertyName("WasDiscovered")]
    public bool? WasDiscovered { get; set; }

    [JsonPropertyName("WasMapped")]
    public bool? WasMapped { get; set; }

    [JsonPropertyName("WasFootfalled")]
    public bool? WasFootfalled { get; set; }

    [JsonPropertyName("TerraformState")]
    public string? TerraformState { get; set; }
}