using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;

public class Coordinate
{
    [JsonPropertyName("x")]
    public decimal X { get; set; }

    [JsonPropertyName("y")]
    public decimal Y { get; set; }

    [JsonPropertyName("z")]
    public decimal Z { get; set; }

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
