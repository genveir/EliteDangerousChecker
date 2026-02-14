using EliteDangerousChecker.Database.Shared;
using EliteDangerousChecker.Database.Spansh;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EliteDangerousChecker.Api.Controllers;

public class DataController : ControllerBase
{
    private readonly IJsonReaderFactory jsonReaderFactory;

    public DataController(IJsonReaderFactory jsonReaderFactory)
    {
        this.jsonReaderFactory = jsonReaderFactory;
    }

    [HttpGet("api/data/insert/{batchToProcess}/max")]
    public async Task<IActionResult> ProcessNumber(int batchToProcess)
    {
        var lastBatch = Directory.GetFiles(@"e:\temp\elite\", "batch_*.json")
            .Select(f => int.Parse(Path.GetFileName(f).Split(['_', '.'])[1]))
            .Max();

        return await ProcessNumber(batchToProcess, lastBatch);
    }

    [HttpGet("api/data/insert/{firstBatchToProcess}/{lastBatchToProcess}")]
    public async Task<IActionResult> ProcessNumber(int firstBatchToProcess, int lastBatchToProcess)
    {
        Dictionary<string, List<string>> results = [];
        results.Add("progress", []);
        results.Add("unmappedSystems", []);
        results.Add("systemsWithErrors", []);
        results.Add("batchesWithErrors", []);

        var stopWatch = new System.Diagnostics.Stopwatch();

        stopWatch.Start();

        for (int batchToProcess = firstBatchToProcess; batchToProcess <= lastBatchToProcess; batchToProcess++)
        {
            Console.WriteLine($"starting batch {batchToProcess}, last is {lastBatchToProcess}");

            results["progress"]!.Add($"processed batch {batchToProcess}");

            try
            {
                await InsertBatch(results, batchToProcess);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                results["batchesWithErrors"].Add($"{batchToProcess}: {e.Message}");

                using (var writer = new StreamWriter(@$"d:\temp\elite\batch-error-{batchToProcess}.log"))
                {
                    writer.WriteLine(e.ToString());
                }
            }
        }

        stopWatch.Stop();

        results["progress"].Add($"time taken: {stopWatch.ElapsedMilliseconds / 1000} s");

        return Ok(results);
    }

    [HttpGet("api/data/update/{batchToProcess}")]
    public async Task<IActionResult> UpdateSingle(int batchToProcess) =>
        await UpdateNumberInFolder(batchToProcess, batchToProcess, null);

    [HttpGet("api/data/update/{firstBatchToProcess}/max")]
    public async Task<IActionResult> UpdateNumber(int firstBatchToProcess) =>
        await UpdateNumberInFolder(firstBatchToProcess, null);

    [HttpGet("api/data/update/{firstBatchToProcess}/max/{subFolder}")]
    public async Task<IActionResult> UpdateNumberInFolder(int firstBatchToProgress, string? subFolder)
    {
        var lastBatch = Directory.GetFiles(@"e:\temp\elite\", "batch_*.json")
            .Select(f => int.Parse(Path.GetFileName(f).Split(['_', '.'])[1]))
            .Max();

        return await UpdateNumberInFolder(firstBatchToProgress, lastBatch, subFolder);
    }

    [HttpGet("api/data/update/{firstBatchToProcess}/{lastBatchToProcess}")]
    public async Task<IActionResult> UpdateNumber(int firstBatchToProcess, int lastBatchToProcess) =>
        await UpdateNumberInFolder(firstBatchToProcess, lastBatchToProcess, null);


    [HttpGet("api/data/update/{firstBatchToProcess}/{lastBatchToProcess}/{subFolder}")]
    public async Task<IActionResult> UpdateNumberInFolder(int firstBatchToProcess, int lastBatchToProcess, string? subFolder)
    {
        Dictionary<string, List<string>> results = [];
        results.Add("progress", []);
        results.Add("unmappedSystems", []);
        results.Add("systemsWithErrors", []);

        var stopWatch = new System.Diagnostics.Stopwatch();

        stopWatch.Start();

        for (int batchToProcess = firstBatchToProcess; batchToProcess <= lastBatchToProcess; batchToProcess++)
        {
            Console.WriteLine($"starting update batch {batchToProcess}, last is {lastBatchToProcess}");

            results["progress"]!.Add($"processed update batch {batchToProcess}");

            await UpdateBatch(results, batchToProcess, subFolder);
        }

        stopWatch.Stop();

        results["progress"].Add($"time taken: {stopWatch.ElapsedMilliseconds / 1000} s");

        return Ok(results);
    }

    private async Task UpdateBatch(Dictionary<string, List<string>> results, int batchToProcess, string? subFolder)
    {
        var subFolderString = subFolder == null ? "" : $@"{subFolder}\";
        var dashSubFolderString = subFolder == null ? "" : $"-{subFolder}";

        using var jsonReader = jsonReaderFactory.CreateJsonReader(
            fileName: @$"e:\temp\elite\{subFolderString}\batch_{batchToProcess}.json");

        var bulkWriter = new BulkWriter();
        await bulkWriter.Initialize();

        await ParseJson(jsonReader, bulkWriter, results, doUpdate: true);
    }


    private async Task InsertBatch(Dictionary<string, List<string>> results, int batchToProcess)
    {
        using var jsonReader = jsonReaderFactory.CreateJsonReader(
            fileName: @$"e:\temp\elite\batch_{batchToProcess}.json");

        var bulkWriter = new BulkWriter();
        await bulkWriter.Initialize();

        await ParseJson(jsonReader, bulkWriter, results, doUpdate: false);
    }

    private async Task ParseJson(JsonReader jsonReader, BulkWriter bulkWriter, Dictionary<string, List<string>> results, bool doUpdate)
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

            if (bulkWriter.RowCount == 100002)
            {
                if (doUpdate)
                    await Updater.RecreateUpdateTables();

                await bulkWriter.WriteSolarSystems(doUpdate);

                if (doUpdate)
                    await Updater.UpdateDatabaseFromUpdateTables();
            }
        }

        if (doUpdate)
            await Updater.RecreateUpdateTables();

        await bulkWriter.WriteSolarSystems(doUpdate);

        if (doUpdate)
            await Updater.UpdateDatabaseFromUpdateTables();
    }
}
