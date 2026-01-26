using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;

public class Market
{
    [JsonPropertyName("commodities")]
    public List<Commodity>? Commodities { get; set; }

    [JsonPropertyName("prohibitedCommodities")]
    public List<string>? ProhibitedCommodities { get; set; }

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

        if (Commodities != null)
        {
            for (int n = 0; n < Commodities.Count; n++) { Commodities[n].UnmappedFieldsRecursive(unmappedFields, prefix + $"commodities[{n}]."); }
        }
    }
}
