namespace EliteDangerousChecker.JournalFile.JournalUpdate.Market;
internal static class SellOrganicData
{
    public static async Task HandleSellOrganicData(string journalLine)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<SellOrganicDataModel>(journalLine);

        if (parsed == null)
        {
            Console.WriteLine("Failed to parse SellOrganicData journal entry");
            return;
        }

        if (parsed.BioData == null)
        {
            Console.WriteLine("SellOrganicData journal entry is missing BioData");
            return;
        }

        foreach (var bioData in parsed.BioData)
        {
            if (string.IsNullOrEmpty(bioData.Species))
            {
                Console.WriteLine("SellOrganicData journal entry has BioData entry with missing Species");
                continue;
            }

            await Database.FromJournal.UpdateSpeciesFromOrganicSale.Execute(
                species: bioData.Species,
                Value: bioData.Value,
                Bonus: bioData.Bonus);
        }
    }
}
