using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;
public class BodyTimeStamps
{
    [JsonPropertyName("distanceToArrival")]
    public string? DistanceToArrival { get; set; }

    [JsonPropertyName("meanAnomaly")]
    public string? MeanAnomaly { get; set; }

    [JsonPropertyName("ascendingNode")]
    public string? AscendingNode { get; set; }
}
