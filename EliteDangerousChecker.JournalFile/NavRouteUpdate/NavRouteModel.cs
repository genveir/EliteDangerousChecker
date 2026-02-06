using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.NavRouteUpdate;

public class NavRouteModel
{
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("Route")]
    public NavHopModel[]? Route { get; set; }
}

public class NavHopModel
{
    [JsonPropertyName("StarSystem")]
    public string? StarSystem { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("StarPos")]
    public double[]? StarPos { get; set; }

    [JsonPropertyName("StarClass")]
    public string? StarClass { get; set; }
}

