using EliteDangerousChecker.Core.Entities.Galaxy;
using EliteDangerousChecker.Core.Entities.Market;
using HtmlAgilityPack;

namespace EliteDangerousChecker.InaraScraper.Parsers;
internal static class InaraPricePointParser
{
    public static PricePoint[] ParsePricePointsFromHtmlDocument(HtmlDocument document, Commodity commodity)
    {
        var pricePoints = new List<PricePoint>();

        var rows = document.DocumentNode
            .SelectNodes("//div[contains(@class, 'maincontainer')]//div[contains(@class, 'maincontent')]//table//tr");

        foreach (var row in rows.Skip(1))
        {
            var cells = row.SelectNodes("td");
            if (cells == null || cells.Count < 7)
            {
                continue; // malformed row
            }
            // Location cell
            var locationCell = cells[0];
            var stationNameNode = locationCell.SelectSingleNode(".//span[contains(@class, 'standardcase')]");
            var systemNameNode = locationCell.SelectSingleNode(".//span[contains(@class, 'uppercase')]");
            if (stationNameNode == null || systemNameNode == null)
            {
                continue; // malformed location cell
            }
            var stationName = stationNameNode.InnerText.Replace("|", "").Trim();
            var systemName = systemNameNode.InnerText.Trim();
            // Price cell
            var priceCell = cells[5];
            var priceString = priceCell.GetAttributeValue("data-order", "0");
            if (!int.TryParse(priceString, out int price))
            {
                continue; // malformed price cell
            }
            // Updated cell
            var updatedCell = cells[6];
            var updatedString = updatedCell.GetAttributeValue("data-order", "0");
            if (!long.TryParse(updatedString, out long updatedUnix))
            {
                continue; // malformed updated cell
            }
            var updatedTime = DateTimeOffset.FromUnixTimeSeconds(updatedUnix);
            var hoursSinceUpdate = (int)(DateTimeOffset.UtcNow - updatedTime).TotalHours;
            var solarSystem = new SolarSystem(systemName);
            var station = new Station(stationName, solarSystem);
            var pricePoint = new PricePoint(commodity, station, price, hoursSinceUpdate);
            pricePoints.Add(pricePoint);
        }

        return pricePoints.ToArray();
    }
}
