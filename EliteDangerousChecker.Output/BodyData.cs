namespace EliteDangerousChecker.Output;
public record BodyData(int BodyId, string Name, string Discovered, string Mapped, string Landed, string TerraformingState, string BodyType, string SubType, int BioSignals, LifeData[] LifeData)
{
    public BodyData(Database.FromJournal.Models.BodyData input) : this(
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

    public int GetScanValue()
    {
        var firstDiscovery = Discovered == "Commander";
        var mapped = Mapped == "Commander";
        var Terraformable = TerraformingState == "Terraformable";

        return ScanValueTable.GetScanValue(SubType, Terraformable, firstDiscovery, mapped);
    }

    public int GetBioValue()
    {
        return LifeData.Sum(ld => ld.Value + ld.Bonus);
    }
}

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
