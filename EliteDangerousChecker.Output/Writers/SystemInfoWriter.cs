using EliteDangerousChecker.Output.Models;
using EliteDangerousChecker.Output.Util;
using System.Text;

namespace EliteDangerousChecker.Output.Writers;
internal static class SystemInfoWriter
{
    public static string FormatSystemHeader(string solarSystemName, long currentSolarSystemId, NavData[] navRoute)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"System: {solarSystemName} ({currentSolarSystemId})");
        AppendNavRoute(builder, currentSolarSystemId, navRoute);

        builder.AppendLine(Helper.BAR);

        return builder.ToString();
    }

    private static void AppendNavRoute(StringBuilder builder, long currentSolarSystemId, NavData[] navRoute)
    {
        if (navRoute.Length == 0)
        {
            builder.AppendLine("No nav route set");
            return;
        }
        builder.Append("Nav Route: ");

        int numPrinted = 0;
        bool positionFound = false;
        for (int n = 0; n < navRoute.Length; n++)
        {
            if (navRoute[n].SolarSystemId == currentSolarSystemId)
            {
                positionFound = true;
                continue;
            }

            if (!positionFound)
                continue;

            AppendNavRouteEntry(builder, navRoute[n]);
            numPrinted++;

            if (numPrinted > 40)
            {
                builder.AppendLine("...");
                break;
            }
        }

        builder.AppendLine();
    }

    private static void AppendNavRouteEntry(StringBuilder builder, NavData navData)
    {
        switch (navData.Discovered)
        {
            case "No":
                builder.Append(ANSI_Colors.BrightGreen);
                break;
            case "Yes":
                builder.Append(ANSI_Colors.BrightRed);
                break;
            case "Commander":
                builder.Append(ANSI_Colors.BrightYellow);
                break;
            default:
                builder.Append(ANSI_Colors.BrightCyan);
                break;
        }

        builder.Append(navData.StarType);
        builder.Append(" ");

        builder.Append(ANSI_Colors.Reset);
    }
}
