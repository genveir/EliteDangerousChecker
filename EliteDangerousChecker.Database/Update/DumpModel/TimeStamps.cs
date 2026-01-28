using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;
public class BodyTimeStamps
{
    [JsonPropertyName("distanceToArrival")]
    public string? DistanceToArrival { get; set; }

    [JsonPropertyName("meanAnomaly")]
    public string? MeanAnomaly { get; set; }

    [JsonPropertyName("ascendingNode")]
    public string? AscendingNode { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Unmapped { get; set; }

    public void UnmappedFieldsRecursive(List<string> unmappedFields, string prefix)
    {
        if (Unmapped != null && Unmapped.Count != 0)
        {
            unmappedFields.AddRange(Unmapped.Keys.Select(s => prefix + s));
        }
    }
}
