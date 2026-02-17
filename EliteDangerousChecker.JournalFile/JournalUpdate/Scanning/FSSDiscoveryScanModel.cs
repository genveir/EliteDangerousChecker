using System.Text.Json.Serialization;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal class FSSDiscoveryScanModel
{
    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("BodyCount")]
    public int? BodyCount { get; set; }
}
