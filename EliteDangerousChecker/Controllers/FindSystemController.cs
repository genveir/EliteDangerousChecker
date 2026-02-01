using EliteDangerousChecker.Database.Update;
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
                fileName: @$"e:\temp\elite\{subFolderString}\batch_{batchToProcess}.json",
                errorFileName: @$"d:\temp\elite\errors{dashSubFolderString}_{batchToProcess}.jsonx",
                unmappedFileName: @$"d:\temp\elite\unmapped{dashSubFolderString}_{batchToProcess}.jsonx");

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
}
