using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;
public class SolarSystemTimeStamps
{
    [JsonPropertyName("powerState")]
    public string? PowerState { get; set; }

    [JsonPropertyName("powers")]
    public string? Powers { get; set; }

    [JsonPropertyName("controllingPower")]
    public string? ControllingPower { get; set; }

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
