using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Update.DumpModel;

[JsonConverter(typeof(Converters.ParentJsonConverter))]
public class Parent
{
    public string? Type { get; set; }

    public int BodyId { get; set; }
}
