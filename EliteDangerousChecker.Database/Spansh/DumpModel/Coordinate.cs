using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;

public class Coordinate
{
    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("z")]
    public double Z { get; set; }
}
