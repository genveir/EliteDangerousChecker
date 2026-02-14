using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;

public class FSSBodySignalsModel
{
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("BodyID")]
    public int? BodyId { get; set; }

    [JsonPropertyName("BodyName")]
    public string? BodyName { get; set; }

    [JsonPropertyName("Signals")]
    public List<Signal>? Signals { get; set; }
}

public class Signal
{
    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    [JsonPropertyName("Type_Localised")]
    public string? TypeLocalised { get; set; }

    [JsonPropertyName("Count")]
    public int? Count { get; set; }
}