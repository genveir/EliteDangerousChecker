using EliteDangerousChecker.Database.Spansh.DumpModel.Converters;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.Spansh.DumpModel;

[JsonConverter(typeof(ParentJsonConverter))]
public class Parent
{
    public string? Type { get; set; }

    public int BodyId { get; set; }
}
