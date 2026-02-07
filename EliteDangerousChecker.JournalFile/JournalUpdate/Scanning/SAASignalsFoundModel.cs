using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal class SAASignalsFoundModel
{
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("event")]
    public string? Event { get; set; }

    [JsonPropertyName("BodyName")]
    public string? BodyName { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("BodyID")]
    public int? BodyId { get; set; }

    [JsonPropertyName("Signals")]
    public List<SaaSignal>? Signals { get; set; }

    [JsonPropertyName("Genuses")]
    public List<SaaGenus>? Genuses { get; set; }
}

internal class SaaSignal
{
    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    [JsonPropertyName("Type_Localised")]
    public string? TypeLocalised { get; set; }

    [JsonPropertyName("Count")]
    public int? Count { get; set; }
}

internal class SaaGenus
{
    [JsonPropertyName("Genus")]
    public string? GenusName { get; set; }

    [JsonPropertyName("Genus_Localised")]
    public string? GenusLocalised { get; set; }
}