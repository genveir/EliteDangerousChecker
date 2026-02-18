namespace EliteDangerousChecker.Output.Models;
public record ScanValues(long ScanValue, int Uncertainty, long BioValue)
{
    public ScanValues(Database.FromJournal.Models.ScanValues input) : this(
        input.ScanValue,
        input.Uncertainty,
        input.BioValue)
    { }
}
