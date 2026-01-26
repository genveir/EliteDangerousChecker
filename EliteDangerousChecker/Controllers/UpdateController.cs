using EliteDangerousChecker.Database;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EliteDangerousChecker.Api.Controllers;

public class UpdateController : ControllerBase
{
    private readonly IJsonReaderFactory jsonReaderFactory;

    public UpdateController(IJsonReaderFactory jsonReaderFactory)
    {
        this.jsonReaderFactory = jsonReaderFactory;
    }

    [HttpGet("api/update/test/{toProcess}")]
    public async Task<IActionResult> Get(int toProcess)
    {
        using var jsonReader = jsonReaderFactory.CreateJsonReader(
            fileName: @"e:\temp\galaxy.json",
            errorFileName: @"d:\temp\errors.jsonx",
            unmappedFileName: @"d:\temp\unmapped.jsonx");

        Dictionary<string, List<string>> results = [];
        results.Add("progress", new List<string>());
        results.Add("unmappedSystems", new List<string>());
        results.Add("systemsWithErrors", new List<string>());
        results.Add("processedSystems", new List<string>());

        var stopWatch = new System.Diagnostics.Stopwatch();

        stopWatch.Start();
        for (int n = 0; n < toProcess; n++)
        {
            var result = await jsonReader.ReadSystem();

            if (result.Errored)
            {
                results["systemsWithErrors"]!.Add($"System {n} errored");
                continue;
            }
            if (result.WasUnmappable.Any())
            {
                var unmapped = JsonSerializer.Serialize(result.WasUnmappable);

                results["unmappedSystems"]!.Add($"System {n} had unmapped fields: {unmapped}");
                continue;
            }
            results["processedSystems"]!.Add($"System {result.System.Name} processed");
        }
        stopWatch.Stop();

        var progress = jsonReader.GetProgressPercentage();
        results["progress"].Add($"parsed {toProcess} records, which was {progress} percent of the whole");
        results["progress"].Add($"time taken: {stopWatch.ElapsedMilliseconds / 1000} s");

        return Ok(results);
    }
}
