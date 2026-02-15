using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.Output;
public record BodyData(int BodyId, string Name, string Discovered, string Mapped, string Landed, string TerraformingState, string BodyType, string SubType, int BioSignals, LifeData[] lifeData)
{
    public BodyData(GetBodyData.BodyData input) : this(
        input.BodyId,
        input.Name,
        input.Discovered,
        input.Mapped,
        input.Landed,
        input.TerraformingState,
        input.BodyType,
        input.SubType,
        input.BioSignals,
        input.LifeData.Select(ld => new LifeData(ld)).ToArray())
    { }
}

public record LifeData(string Genus, string Species, int Value, bool Scanned, bool First)
{
    public LifeData(GetBodyData.LifeData input) : this(
        input.Genus,
        input.Species,
        input.Value,
        input.Scanned,
        input.First)
    { }
}
