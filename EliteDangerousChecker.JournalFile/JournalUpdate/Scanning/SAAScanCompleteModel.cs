using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal class SAAScanCompleteModel
{
    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("BodyID")]
    public int? BodyId { get; set; }
}
