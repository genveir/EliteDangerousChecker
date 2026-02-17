using EliteDangerousChecker.Output.Models;

namespace EliteDangerousChecker.Output.Util;
internal static class Helper
{
    public static bool IsNotable(BodyData bodyData) =>
        bodyData.TerraformingState == "Terraformable" ||
        bodyData.SubType == "Water world" ||
        bodyData.SubType == "Earth-like world" ||
        bodyData.SubType == "Ammonia world" ||
        bodyData.BioSignals > 0 ||
        bodyData.MainStar;

    public const string BAR = "###############################################################################################################";

    public static string ClearTerminal() => "\u001b[2J";

    public static string SetCursorPosition(int left, int top) => $"\u001b[{top};{left}H";
}
