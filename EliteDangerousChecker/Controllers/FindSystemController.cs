using EliteDangerousChecker.Database.FromJournal;
using EliteDangerousChecker.Database.Spansh;
using Microsoft.AspNetCore.Mvc;

namespace EliteDangerousChecker.Api.Controllers;

public class FindSystemController : ControllerBase
{
    private readonly IJsonReaderFactory jsonReaderFactory;

    public FindSystemController(IJsonReaderFactory jsonReaderFactory)
    {
        this.jsonReaderFactory = jsonReaderFactory;
    }

    [HttpGet("api/findsystem/{batchToStart}/{batchToEnd}/{subFolder}")]
    public async Task<IActionResult> FindSystem(int batchToStart, int batchToEnd, string subFolder, [FromQuery] string urlEncodenSystemName)
    {
        for (int batchToProcess = batchToStart; batchToProcess <= batchToEnd; batchToProcess++)
        {
            Console.WriteLine("Reading batch " + batchToProcess);

            var subFolderString = subFolder == null ? "" : $@"{subFolder}\";
            var dashSubFolderString = subFolder == null ? "" : $"-{subFolder}";

            using var jsonReader = jsonReaderFactory.CreateJsonReader(
                fileName: @$"e:\temp\elite\{subFolderString}\batch_{batchToProcess}.json");

            var systemName = System.Net.WebUtility.UrlDecode(urlEncodenSystemName);

            while (jsonReader.HasMore())
            {
                var result = await jsonReader.ReadSystem();

                if (result.System != null && result.System.Name == systemName)
                {
                    return Ok(new { batch = batchToProcess, system = result.System });
                }
            }
        }

        return NotFound();
    }

    [HttpGet("api/findsystem/uninserted/{batchToStart}/{batchToEnd}")]
    public async Task<IActionResult> FindFirstUninserted(int batchToStart, int batchToEnd) =>
        await FindFirstUninserted(batchToStart, batchToEnd, subFolder: null);

    [HttpGet("api/findsystem/uninserted/{batchToStart}/{batchToEnd}/{subFolder}")]
    public async Task<IActionResult> FindFirstUninserted(int batchToStart, int batchToEnd, string? subFolder)
    {
        for (int batchToProcess = batchToStart; batchToProcess < batchToEnd; batchToProcess++)
        {
            Console.WriteLine("Reading batch " + batchToProcess);

            var subFolderString = subFolder == null ? "" : $@"{subFolder}\";
            var dashSubFolderString = subFolder == null ? "" : $"-{subFolder}";
            using var jsonReader = jsonReaderFactory.CreateJsonReader(
                fileName: @$"e:\temp\elite\{subFolderString}\batch_{batchToProcess}.json");

            if (jsonReader.HasMore())
            {
                var result = await jsonReader.ReadSystem();

                var exists = await ExistsSolarSystem.Execute(result.System!.Id64);

                if (!exists)
                {
                    return Ok(new { batch = batchToProcess, system = result.System });
                }
            }
        }

        return NotFound();
    }
}
