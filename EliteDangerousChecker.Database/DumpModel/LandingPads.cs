using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;

public class LandingPads
{
    [JsonPropertyName("large")]
    public int? Large { get; set; }

    [JsonPropertyName("medium")]
    public int? Medium { get; set; }

    [JsonPropertyName("small")]
    public int? Small { get; set; }

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
