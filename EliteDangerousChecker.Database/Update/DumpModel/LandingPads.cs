using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

public class LandingPads
{
    [JsonPropertyName("large")]
    public int? Large { get; set; }

    [JsonPropertyName("medium")]
    public int? Medium { get; set; }

    [JsonPropertyName("small")]
    public int? Small { get; set; }
}
