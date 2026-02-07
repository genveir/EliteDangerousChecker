using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.FSD;

public class FSDTargetModel
{
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("StarClass")]
    public string? StarClass { get; set; }

    [JsonPropertyName("RemainingJumpsInRoute")]
    public int? RemainingJumpsInRoute { get; set; }
}
