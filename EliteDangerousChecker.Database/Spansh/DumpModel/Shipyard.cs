using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;
public class Shipyard
{
    [JsonPropertyName("ships")]
    public List<Ship>? Ships { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }
}
