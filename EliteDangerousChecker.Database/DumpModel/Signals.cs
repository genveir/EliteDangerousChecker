using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;

public class Signals
{
    [JsonPropertyName("signals")]
    public Dictionary<string, int>? SignalTypes { get; set; }

    [JsonPropertyName("genuses")]
    public List<string>? Genuses { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }

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
