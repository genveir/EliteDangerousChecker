using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

public class Faction
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("allegiance")]
    public string? Allegiance { get; set; }

    [JsonPropertyName("government")]
    public string? Government { get; set; }

    [JsonPropertyName("influence")]
    public decimal? Influence { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }
}
