using EliteDangerousChecker.Database.Update.DumpModel;
using System.Text.Json;

namespace EliteDangerousChecker.Database.Update;

public interface IJsonReaderFactory
{
    JsonReader CreateJsonReader(string fileName);
}

public class JsonReaderFactory : IJsonReaderFactory
{
    public JsonReader CreateJsonReader(string fileName)
    {
        var reader = new StreamReader(new FileStream(fileName, FileMode.Open));

        return new JsonReader(reader);
    }
}

public sealed class JsonReader : IDisposable
{
    private readonly StreamReader reader;

    public JsonReader(StreamReader reader)
    {
        this.reader = reader;
    }

    public bool HasMore()
    {
        return !reader.EndOfStream;
    }

    HashSet<long> solarSystemIds = [];
    public async Task<Result> ReadSystem()
    {
        var json = await reader.ReadLineAsync();

        if (json == null || json.StartsWith(']') || json.StartsWith('['))
        {
            return new(null, "json was null or started with bracket", []);
        }

        if (json.EndsWith(','))
            json = json[..^1];

        var system = JsonSerializer.Deserialize<SolarSystem>(json);

        if (system == null)
        {
            return new(null, "system deserialized to null", []);
        }

        if (solarSystemIds.Contains(system.Id64))
        {
            return new(null, $"duplicate system id {system.Id64}", []);
        }

        solarSystemIds.Add(system.Id64);

        return new(system, null, []);
    }

    public decimal GetProgressPercentage()
    {
        decimal position = reader.BaseStream.Position;
        decimal total = reader.BaseStream.Length;

        return position / total;
    }

    public void Dispose()
    {
        reader.Dispose();
    }

    public record Result(SolarSystem? System, string? Errored, string[] WasUnmappable);
}
