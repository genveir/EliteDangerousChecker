using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;
public class Outfitting
{
    [JsonPropertyName("modules")]
    public List<Module>? Modules { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Unmapped { get; set; }

    public void UnmappedFieldsRecursive(List<string> unmappedFields, string prefix)
    {
        if (Unmapped != null && Unmapped.Any())
        {
            unmappedFields.AddRange(Unmapped.Keys.Select(s => prefix + s));
        }

        if (Modules != null)
        {
            for (int n = 0; n < Modules.Count; n++) { Modules[n].UnmappedFieldsRecursive(unmappedFields, prefix + $"modules[{n}]."); }
        }
    }
}
