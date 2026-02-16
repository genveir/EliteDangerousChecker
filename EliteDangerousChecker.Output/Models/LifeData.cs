namespace EliteDangerousChecker.Output.Models;

public record LifeData(string Genus, string Species, int Value, int Bonus, string Scanned)
{
    public LifeData(Database.FromJournal.Models.LifeData input) : this(
        input.Genus,
        input.Species,
        input.Value,
        input.Bonus,
        input.Scanned)
    { }
}
