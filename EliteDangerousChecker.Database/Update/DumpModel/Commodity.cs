using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

public class Commodity
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("commodityId")]
    public long CommodityId { get; set; }

    [JsonPropertyName("demand")]
    public int? Demand { get; set; }

    [JsonPropertyName("supply")]
    public int? Supply { get; set; }

    [JsonPropertyName("buyPrice")]
    public int? BuyPrice { get; set; }

    [JsonPropertyName("sellPrice")]
    public int? SellPrice { get; set; }

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