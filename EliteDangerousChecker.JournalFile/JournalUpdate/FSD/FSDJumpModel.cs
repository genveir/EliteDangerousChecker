using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.FSD;

public class FSDJumpModel
{
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("Taxi")]
    public bool? Taxi { get; set; }

    [JsonPropertyName("Multicrew")]
    public bool? Multicrew { get; set; }

    [JsonPropertyName("StarSystem")]
    public string? StarSystem { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("StarPos")]
    public double[]? StarPos { get; set; }

    [JsonPropertyName("SystemAllegiance")]
    public string? SystemAllegiance { get; set; }

    [JsonPropertyName("SystemEconomy")]
    public string? SystemEconomy { get; set; }

    [JsonPropertyName("SystemEconomy_Localised")]
    public string? SystemEconomy_Localised { get; set; }

    [JsonPropertyName("SystemSecondEconomy")]
    public string? SystemSecondEconomy { get; set; }

    [JsonPropertyName("SystemSecondEconomy_Localised")]
    public string? SystemSecondEconomy_Localised { get; set; }

    [JsonPropertyName("SystemGovernment")]
    public string? SystemGovernment { get; set; }

    [JsonPropertyName("SystemGovernment_Localised")]
    public string? SystemGovernment_Localised { get; set; }

    [JsonPropertyName("SystemSecurity")]
    public string? SystemSecurity { get; set; }

    [JsonPropertyName("SystemSecurity_Localised")]
    public string? SystemSecurity_Localised { get; set; }

    [JsonPropertyName("Population")]
    public long? Population { get; set; }

    [JsonPropertyName("Body")]
    public string? Body { get; set; }

    [JsonPropertyName("BodyID")]
    public int? BodyID { get; set; }

    [JsonPropertyName("BodyType")]
    public string? BodyType { get; set; }

    [JsonPropertyName("JumpDist")]
    public double? JumpDist { get; set; }

    [JsonPropertyName("FuelUsed")]
    public double? FuelUsed { get; set; }

    [JsonPropertyName("FuelLevel")]
    public double? FuelLevel { get; set; }
}
