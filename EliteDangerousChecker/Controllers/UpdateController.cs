using Dapper;
using EliteDangerousChecker.Database;
using EliteDangerousChecker.Database.Update;
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

    [HttpGet("api/update/test/{batchToProcess}")]
    public async Task<IActionResult> ProcessSingle(int batchToProcess) =>
        await ProcessNumber(batchToProcess, batchToProcess);

    [HttpGet("api/data/initial/{firstBatchToProcess}/{lastBatchToProcess}")]
    public async Task<IActionResult> ProcessNumber(int firstBatchToProcess, int lastBatchToProcess)
    {
        Dictionary<string, List<string>> results = [];
        results.Add("progress", []);
        results.Add("unmappedSystems", []);
        results.Add("systemsWithErrors", []);

        var stopWatch = new System.Diagnostics.Stopwatch();

        stopWatch.Start();

        for (int batchToProcess = firstBatchToProcess; batchToProcess <= lastBatchToProcess; batchToProcess++)
        {
            Console.WriteLine($"starting batch {batchToProcess}, last is {lastBatchToProcess}");

            results["progress"]!.Add($"processed batch {batchToProcess}");
            await ParseJson(results, batchToProcess);
        }

        stopWatch.Stop();

        results["progress"].Add($"time taken: {stopWatch.ElapsedMilliseconds / 1000} s");

        return Ok(results);
    }

    // 38, 68
    [HttpGet("api/data/fix/{batchToProcess}")]
    public async Task<IActionResult> FixSingle(int batchToProcess)
    {
        Dictionary<string, List<string>> results = [];
        results.Add("progress", []);
        results.Add("unmappedSystems", []);
        results.Add("systemsWithErrors", []);

        var stopWatch = new System.Diagnostics.Stopwatch();

        stopWatch.Start();

        await FixBatch(results, batchToProcess);

        stopWatch.Stop();

        results["progress"].Add($"time taken: {stopWatch.ElapsedMilliseconds / 1000} s");

        return Ok(results);
    }

    private async Task FixBatch(Dictionary<string, List<string>> results, int batchToProcess)
    {
        using var jsonReader = jsonReaderFactory.CreateJsonReader(
            fileName: @$"e:\temp\elite\batch_{batchToProcess}.json",
            errorFileName: @$"d:\temp\elite\errors_{batchToProcess}.jsonx",
            unmappedFileName: @$"d:\temp\elite\unmapped_{batchToProcess}.jsonx");

        var bulkWriter = new BulkWriter();
        await bulkWriter.Initialize();

        using var connection = DbAccess.GetOpenConnection();

        while (jsonReader.HasMore())
        {
            var result = await jsonReader.ReadSystem();

            var solarSystemId = result.System!.Id64;

            var exists = await connection.ExecuteScalarAsync<long?>(
                "select Id from SolarSystem where Id = @Id",
                new { Id = solarSystemId });

            if (exists == null)
            {
                Console.WriteLine("Skipped to first missing system");
                await ParseJson(jsonReader, bulkWriter, results);
            }
        }
    }


    private async Task ParseJson(Dictionary<string, List<string>> results, int batchToProcess)
    {
        using var jsonReader = jsonReaderFactory.CreateJsonReader(
            fileName: @$"e:\temp\elite\batch_{batchToProcess}.json",
            errorFileName: @$"d:\temp\elite\errors_{batchToProcess}.jsonx",
            unmappedFileName: @$"d:\temp\elite\unmapped_{batchToProcess}.jsonx");

        var bulkWriter = new BulkWriter();
        await bulkWriter.Initialize();

        await ParseJson(jsonReader, bulkWriter, results);
    }

    private async Task ParseJson(JsonReader jsonReader, BulkWriter bulkWriter, Dictionary<string, List<string>> results)
    {
        int recordIndex = 0;

        while (jsonReader.HasMore())
        {
            var result = await jsonReader.ReadSystem();

            if (result.Errored != null)
            {
                results["systemsWithErrors"]!.Add($"System {recordIndex} errored: {result.Errored}");
                continue;
            }
            if (result.WasUnmappable.Length != 0)
            {
                var unmapped = JsonSerializer.Serialize(result.WasUnmappable);

                results["unmappedSystems"]!.Add($"System {recordIndex} had unmapped fields: {unmapped}");
                continue;
            }

            await bulkWriter.AddSystem(result.System!);

            recordIndex++;

            if (bulkWriter.RowCount == 100001)
            {
                await bulkWriter.WriteSolarSystems();
            }
        }

        await bulkWriter.WriteSolarSystems();
    }
}
