using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

public class Ring
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("mass")]
    public double? Mass { get; set; }

    [JsonPropertyName("innerRadius")]
    public double? InnerRadius { get; set; }

    [JsonPropertyName("outerRadius")]
    public double? OuterRadius { get; set; }

    [JsonPropertyName("id64")]
    public long? Id64 { get; set; }

    [JsonPropertyName("signals")]
    public Signals? Signals { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Unmapped { get; set; }

    public void UnmappedFieldsRecursive(List<string> unmappedFields, string prefix)
    {
        if (Unmapped != null && Unmapped.Count != 0)
        {
            unmappedFields.AddRange(Unmapped.Keys.Select(s => prefix + s));
        }

        Signals?.UnmappedFieldsRecursive(unmappedFields, prefix + "signals.");
    }
}
