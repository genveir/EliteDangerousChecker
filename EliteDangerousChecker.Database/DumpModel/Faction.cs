using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;

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

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Unmapped { get; set; }

    public void UnmappedFieldsRecursive(List<string> unmappedFields, string prefix)
    {
        if (Unmapped != null && Unmapped.Any())
        {
            unmappedFields.AddRange(Unmapped.Keys.Select(s => prefix + s));
        }
    }
}
