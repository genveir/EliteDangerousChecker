using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;
public class Outfitting
{
    [JsonPropertyName("modules")]
    public List<Module>? Modules { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }
}
