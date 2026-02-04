using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;
public class MarketSellModel
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("MarketID")]
    public long StationId { get; set; }

    [JsonPropertyName("Type")]
    public string? TypeName { get; set; }

    [JsonPropertyName("Type_Localised")]
    public string? Commodity { get; set; }

    [JsonPropertyName("Count")]
    public int Count { get; set; }

    [JsonPropertyName("SellPrice")]
    public int SellPrice { get; set; }
}
