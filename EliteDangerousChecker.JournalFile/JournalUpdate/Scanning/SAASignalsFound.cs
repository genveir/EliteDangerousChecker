namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class SAASignalsFound
{
    public static Task HandleSAASignalsFound(string journalLine)
    {
        var parsed = System.Text.Json.JsonSerializer.Deserialize<SAASignalsFoundModel>(journalLine);
        if (parsed == null)
        {
            Console.WriteLine("Failed to parse SAASignalsFound journal entry");
            return Task.CompletedTask;
        }

        if (parsed.Genuses != null && parsed.Genuses.Count > 0)
        {
            if (parsed.BodyName != null)
            {
                Console.Write($"{parsed.BodyName,-30}");
            }

            Console.WriteLine(string.Join(", ", parsed.Genuses.Select(g => g.GenusLocalised)));
        }

        return Task.CompletedTask;
    }
}
