using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Planetary;
internal class DisembarkModel
{
    [JsonPropertyName("OnPlanet")]
    public bool? OnPlanet { get; set; }

    [JsonPropertyName("SRV")]
    public bool? SRV { get; set; }

    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("BodyID")]
    public int? BodyId { get; set; }
}
