using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;
public class SolarSystemTimeStamps
{
    [JsonPropertyName("powerState")]
    public string? PowerState { get; set; }

    [JsonPropertyName("powers")]
    public string? Powers { get; set; }

    [JsonPropertyName("controllingPower")]
    public string? ControllingPower { get; set; }
}
