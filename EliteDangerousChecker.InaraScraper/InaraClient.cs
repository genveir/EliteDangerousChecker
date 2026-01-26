using EliteDangerousChecker.Core.Entities.Galaxy;
using EliteDangerousChecker.Core.Entities.Market;
using EliteDangerousChecker.Core.Plugins;
using EliteDangerousChecker.InaraScraper.Parsers;

namespace EliteDangerousChecker.InaraScraper;

public class InaraClient : IInaraClient
{
    private readonly HttpClient _httpClient;

    public InaraClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://inara.cz/");
    }

    public async Task<PricePoint[]> GetSellingPrices(Commodity commodity, SolarSystem searchFrom)
    {
        // min landing pad medium, no fleet carriers, max price age 30 days, price condition better than average + 50%, power Jerome Archer, order by best price
        var path = $"https://inara.cz/elite/commodities/?formbrief=1&pi1=2&pa1%5B%5D={(int)commodity}&ps1={searchFrom.Name}&pi10=1&pi11=0&pi3=2&pi9=0&pi4=0&pi8=1&pi13=0&pi5=720&pi12=50&pi7=0&pi14=12&ps3=";

        var response = await _httpClient.GetAsync(path);

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException($"Failed to retrieve data from Inara. Response content was: {content}");
        }

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(content);

        return InaraPricePointParser.ParsePricePointsFromHtmlDocument(doc, commodity);
    }
}
