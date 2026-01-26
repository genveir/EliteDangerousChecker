using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel.Converters;

public class ParentJsonConverter : JsonConverter<Parent>
{
    public override Parent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            return null;

        using (var doc = JsonDocument.ParseValue(ref reader))
        {
            var obj = doc.RootElement;
            foreach (var prop in obj.EnumerateObject())
            {
                return new Parent
                {
                    Type = prop.Name,
                    BodyId = prop.Value.GetInt32()
                };
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, Parent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(value.Type ?? "Unknown", value.BodyId);
        writer.WriteEndObject();
    }
}