using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;

[JsonConverter(typeof(Converters.ParentJsonConverter))]
public class Parent
{
    public string? Type { get; set; }

    public int BodyId { get; set; }
}
