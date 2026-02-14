using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.MarketUpdate;
internal class MarketModel
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("MarketID")]
    public long MarketID { get; set; }

    [JsonPropertyName("Items")]
    public List<MarketItemModel>? Items { get; set; }
}

internal class MarketItemModel
{
    [JsonPropertyName("Name_Localised")]
    public string? Name { get; set; }

    [JsonPropertyName("Category_Localised")]
    public string? Category { get; set; }

    [JsonPropertyName("BuyPrice")]
    public int BuyPrice { get; set; }

    [JsonPropertyName("SellPrice")]
    public int SellPrice { get; set; }

    [JsonPropertyName("Demand")]
    public int Demand { get; set; }

    [JsonPropertyName("Stock")]
    public int Supply { get; set; }
}
