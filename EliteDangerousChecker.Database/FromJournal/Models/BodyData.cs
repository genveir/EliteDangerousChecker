namespace EliteDangerousChecker.Database.FromJournal.Models;
public record BodyData(int BodyId, string Name, string Discovered, string Mapped, string Landed, string TerraformingState, string BodyType, string SubType, int BioSignals, bool MainStar)
{
    public LifeData[] LifeData { get; set; } = Array.Empty<LifeData>();
}