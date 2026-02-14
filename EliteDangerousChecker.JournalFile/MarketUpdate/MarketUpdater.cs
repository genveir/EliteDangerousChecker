using EliteDangerousChecker.Database.FromJournal;
using System.Text.Json;

namespace EliteDangerousChecker.JournalFile.MarketUpdate;
internal class MarketUpdater
{
    const string MarketLocation = @"C:\Users\genve\Saved Games\Frontier Developments\Elite Dangerous\Market.json";

    public async Task UpdateMarket()
    {
        Console.WriteLine($"Starting market update");

        using var reader = new StreamReader(new FileStream(MarketLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

        var json = await reader.ReadToEndAsync();

        var model = JsonSerializer.Deserialize<MarketModel>(json);

        if (model == null)
        {
            Console.WriteLine($"failed to deserialize market json");
            return;
        }

        var updater = new UpdateMarket(model.MarketID, model.Timestamp);

        foreach (var item in model.Items ?? [])
        {
            if (item.Name == null)
            {
                Console.WriteLine($"skipping item with null name");
                continue;
            }

            await updater.AddItem(item.Name, item.Category, item.BuyPrice, item.SellPrice, item.Demand, item.Supply);
        }

        await updater.DoUpdate();

        Console.WriteLine("Finished market update");
    }
}
