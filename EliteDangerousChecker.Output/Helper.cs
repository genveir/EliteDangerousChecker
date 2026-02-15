namespace EliteDangerousChecker.Output;
internal static class Helper
{
    public static bool IsNotable(BodyData bodyData) =>
        bodyData.TerraformingState == "Terraformable" ||
        bodyData.SubType == "Water world" ||
        bodyData.SubType == "Earth-like world" ||
        bodyData.SubType == "Ammonia world" ||
        bodyData.BioSignals > 0 ||
        IsPrimaryStar(bodyData.Name);

    public static bool IsPrimaryStar(string name) =>
        name.Length == 0 || name == " A";
}
