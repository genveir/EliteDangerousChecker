using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

public class Belt
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("mass")]
    public double? Mass { get; set; }

    [JsonPropertyName("innerRadius")]
    public double? InnerRadius { get; set; }

    [JsonPropertyName("outerRadius")]
    public double? OuterRadius { get; set; }

    [JsonPropertyName("id64")]
    public long? Id64 { get; set; }
}