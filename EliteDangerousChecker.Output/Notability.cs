using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.Output;
internal static class Notability
{
    public static bool IsNotable(GetBodyData.BodyData bodyData) =>
        bodyData.TerraformingState == "Terraformable" ||
        bodyData.SubType == "Water world" ||
        bodyData.SubType == "Earth-like world" ||
        bodyData.SubType == "Ammonia world" ||
        bodyData.BioSignals > 0 ||
        IsPrimaryStar(bodyData.Name);

    private static bool IsPrimaryStar(string name) =>
        name.Length == 0 || name == " A";
}
