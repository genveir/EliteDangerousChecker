using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;
public class Module
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("moduleId")]
    public long? ModuleId { get; set; }

    [JsonPropertyName("class")]
    public int? Class { get; set; }

    [JsonPropertyName("rating")]
    public string? Rating { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("ship")]
    public string? Ship { get; set; }
}
