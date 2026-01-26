using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;
public class Ship
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("shipId")]
    public long ShipId { get; set; }

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
