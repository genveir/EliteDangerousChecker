using EliteDangerousChecker.Database.DumpModel;
using System.Text.Json;

namespace EliteDangerousChecker.Database;

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

        errorFileWriter.WriteLine(reader.ReadLine()); // Skip opening bracket
    }

    public bool HasMore()
    {
        return !reader.EndOfStream;
    }

    public async Task<Result> ReadSystem()
    {
        var json = await reader.ReadLineAsync();

        if (json == null || json.StartsWith("]"))
        {
            errorFileWriter.WriteLine(json);
            return new(null, true, []);
        }

        json = json.TrimEnd();
        if (json.EndsWith(","))
            json = json.Substring(0, json.Length - 1);

        var system = JsonSerializer.Deserialize<SolarSystem>(json);

        if (system == null)
        {
            errorFileWriter.WriteLine(json);
            return new(null, true, []);
        }

        var unmappableFields = system.UnmappedFieldsRecursive();
        if (unmappableFields.Any())
        {
            await unmappedWriter.WriteLineAsync(json);
            return new(null, false, unmappableFields);
        }

        return new(system, false, []);
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

    public record Result(SolarSystem? System, bool Errored, string[] WasUnmappable);
}
