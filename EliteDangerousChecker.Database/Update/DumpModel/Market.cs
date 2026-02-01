using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

public class Market
{
    [JsonPropertyName("commodities")]
    public List<Commodity>? Commodities { get; set; }

    [JsonPropertyName("prohibitedCommodities")]
    public List<string>? ProhibitedCommodities { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }
}
