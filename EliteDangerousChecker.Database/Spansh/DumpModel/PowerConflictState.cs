using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;
public class PowerConflictProgress
{
    [JsonPropertyName("power")]
    public string? Power { get; set; }

    [JsonPropertyName("progress")]
    public double? Progress { get; set; }
}
