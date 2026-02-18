namespace EliteDangerousChecker.Output.Models;
public record BodyData(int BodyId, string Name, string Discovered, string Mapped, string Landed, string TerraformingState, string BodyType, string SubType, int BioSignals, bool MainStar, long ScanValue, LifeData[] LifeData)
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
        input.MainStar,
        input.ScanValue,
        input.LifeData.Select(ld => new LifeData(ld)).ToArray())
    { }

    public (int uncertainty, int uncertaintyValue, int guaranteedValue) GetBioValue()
    {
        var lifeDataToConsider = LifeData.Where(ld => ld.Scanned == "Commander");

        var uncertainty = lifeDataToConsider.Count(ld => ld.Value == 0);
        var uncertaintyValue = uncertainty * 5000000;
        var guaranteedValue = lifeDataToConsider.Sum(ld => ld.Value + ld.Bonus);

        return (uncertainty, uncertaintyValue, guaranteedValue);
    }
}
