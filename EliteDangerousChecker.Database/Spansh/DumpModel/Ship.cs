using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;
public class Ship
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("shipId")]
    public long ShipId { get; set; }
}
