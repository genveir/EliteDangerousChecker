using EliteDangerousChecker.Database.DirectQueries;
using System.Text.Json;

namespace EliteDangerousChecker.JournalFile.JournalUpdate.Market;
internal static class MarketSell
{
    public static async Task HandleMarketSell(string journalLine)
    {
        var parsed = JsonSerializer.Deserialize<MarketSellModel>(journalLine);

        if (parsed == null)
        {
            Console.WriteLine("Failed to parse MarketSell journal entry");
            return;
        }

        if (parsed.Commodity == null)
        {
            parsed.Commodity = GetCommodityNameForType(parsed.TypeName ?? "");
        }

        if (parsed.Commodity == null)
        {
            Console.WriteLine($"Skipping MarketSell entry with unknown commodity \"{parsed.TypeName}\"");
            return;
        }

        Console.WriteLine($"Processing MarketSell entry: {parsed.Count}x {parsed.Commodity} for {parsed.SellPrice}");
        await InsertCommoditiesSoldRow.Execute(
            timestamp: parsed.Timestamp,
            stationId: parsed.StationId,
            commodity: parsed.Commodity,
            count: parsed.Count,
            sellPrice: parsed.SellPrice);
    }

    private static string? GetCommodityNameForType(string type)
    {
        return type switch
        {
            "alexandrite" => "Alexandrite",
            "bauxite" => "Bauxite",
            "benitoite" => "Benitoite",
            "bertrandite" => "Bertrandite",
            "beryllium" => "Beryllium",
            "bromellite" => "Bromellite",
            "cobalt" => "Cobalt",
            "coltan" => "Coltan",
            "gallite" => "Gallite",
            "gallium" => "Gallium",
            "gold" => "Gold",
            "haematite" => "Haematite",
            "indium" => "Indium",
            "indite" => "Indite",
            "lepidolite" => "Lepidolite",
            "monazite" => "Monazite",
            "musgravite" => "Musgravite",
            "osmium" => "Osmium",
            "painite" => "Painite",
            "palladium" => "Palladium",
            "platinum" => "Platinum",
            "praseodymium" => "Praseodymium",
            "samarium" => "Samarium",
            "serendibite" => "Serendibite",
            "silver" => "Silver",
            "tritium" => "Tritium",
            "uraninite" => "Uraninite",
            "uranium" => "Uranium",
            _ => null
        };
    }
}
