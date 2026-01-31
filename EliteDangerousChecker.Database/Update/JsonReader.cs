using EliteDangerousChecker.Database.Update.DumpModel;
using System.Text.Json;

namespace EliteDangerousChecker.Database.Update;

public interface IJsonReaderFactory
{
    JsonReader CreateJsonReader(string fileName, string errorFileName, string unmappedFileName);
}

public class JsonReaderFactory : IJsonReaderFactory
{
    public JsonReader CreateJsonReader(string fileName, string errorFileName, string unmappedFileName)
    {
        var reader = new StreamReader(new FileStream(fileName, FileMode.Open));
        var errorWriter = new StreamWriter(new FileStream(errorFileName, FileMode.Create));
        var unmappedWriter = new StreamWriter(new FileStream(unmappedFileName, FileMode.Create));

        return new JsonReader(reader, errorWriter, unmappedWriter);
    }
}

public sealed class JsonReader : IDisposable
{
    private readonly StreamReader reader;
    private readonly StreamWriter errorFileWriter;
    private readonly StreamWriter unmappedWriter;

    public JsonReader(StreamReader reader, StreamWriter errorFileWriter, StreamWriter unmappedWriter)
    {
        this.reader = reader;
        this.errorFileWriter = errorFileWriter;
        this.unmappedWriter = unmappedWriter;
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
            errorFileWriter.WriteLine(json);
            return new(null, "json was null or started with bracket", []);
        }

        if (json.EndsWith(','))
            json = json[..^1];

        var system = JsonSerializer.Deserialize<SolarSystem>(json);

        if (system == null)
        {
            errorFileWriter.WriteLine(json);
            return new(null, "system deserialized to null", []);
        }

        var unmappableFields = system.UnmappedFieldsRecursive();
        if (unmappableFields.Length != 0)
        {
            await unmappedWriter.WriteLineAsync(json);
            return new(null, null, unmappableFields);
        }

        if (solarSystemIds.Contains(system.Id64))
        {
            errorFileWriter.WriteLine(json);
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
        errorFileWriter.Dispose();
        unmappedWriter.Dispose();
    }

    public record Result(SolarSystem? System, string? Errored, string[] WasUnmappable);
}
