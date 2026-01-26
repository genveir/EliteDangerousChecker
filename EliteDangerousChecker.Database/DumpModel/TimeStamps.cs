using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;
public class TimeStamps
{
    [JsonPropertyName("distanceToArrival")]
    public string? DistanceToArrival { get; set; }

    [JsonPropertyName("meanAnomaly")]
    public string? MeanAnomaly { get; set; }

    [JsonPropertyName("powerState")]
    public string? PowerState { get; set; }

    [JsonPropertyName("powers")]
    public string? Powers { get; set; }

    [JsonPropertyName("controllingPower")]
    public string? ControllingPower { get; set; }

    [JsonPropertyName("ascendingNode")]
    public string? AscendingNode { get; set; }

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
