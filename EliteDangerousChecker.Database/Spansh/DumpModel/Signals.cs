using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;

public class Signals
{
    [JsonPropertyName("signals")]
    public Dictionary<string, int>? SignalTypes { get; set; }

    [JsonPropertyName("genuses")]
    public List<string>? Genuses { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }
}
