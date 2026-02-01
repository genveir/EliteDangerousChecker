using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;
public class SolarSystem
{
    [JsonPropertyName("id64")]
    public long Id64 { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("coords")]
    public Coordinate? Coordinates { get; set; }

    [JsonPropertyName("allegiance")]
    public string? Allegiance { get; set; }

    [JsonPropertyName("government")]
    public string? Government { get; set; }

    [JsonPropertyName("primaryEconomy")]
    public string? PrimaryEconomy { get; set; }

    [JsonPropertyName("secondaryEconomy")]
    public string? SecondaryEconomy { get; set; }

    [JsonPropertyName("security")]
    public string? Security { get; set; }

    [JsonPropertyName("population")]
    public long? Population { get; set; }

    [JsonPropertyName("bodyCount")]
    public int? BodyCount { get; set; }

    [JsonPropertyName("controllingFaction")]
    public Faction? ControllingFaction { get; set; }

    [JsonPropertyName("factions")]
    public List<Faction>? Factions { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("bodies")]
    public List<Body>? Bodies { get; set; }

    [JsonPropertyName("stations")]
    public List<Station>? Stations { get; set; }

    [JsonPropertyName("timestamps")]
    public SolarSystemTimeStamps? Timestamps { get; set; }

    [JsonPropertyName("controllingPower")]
    public string? ControllingPower { get; set; }

    [JsonPropertyName("powerState")]
    public string? PowerState { get; set; }

    [JsonPropertyName("powerStateControlProgress")]
    public double? PowerStateControlProgress { get; set; }

    [JsonPropertyName("powerStateReinforcement")]
    public int? PowerStateReinforcement { get; set; }

    [JsonPropertyName("powerStateUndermining")]
    public int? PowerStateUndermining { get; set; }

    [JsonPropertyName("powerConflictProgress")]
    public List<PowerConflictProgress>? PowerConflictProgress { get; set; }

    [JsonPropertyName("powers")]
    public List<string>? Powers { get; set; }
}