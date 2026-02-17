using EliteDangerousChecker.Output.Models;
using EliteDangerousChecker.Output.Util;
using System.Text;

namespace EliteDangerousChecker.Output.Writers;
internal static class SystemInfoWriter
{
    public static string FormatSystemHeader(string solarSystemName, long currentSolarSystemId, int? totalBodies, int knownBodies, NavData[] navRoute)
    {
        var builder = new StringBuilder();

        builder.Append($"System: {solarSystemName} ({currentSolarSystemId})  ".PadRight(40));
        AppendBodyCount(builder, totalBodies, knownBodies);
        builder.AppendLine();

        AppendNavRoute(builder, currentSolarSystemId, navRoute);

        builder.AppendLine(Helper.BAR);

        return builder.ToString();
    }

    public static string FormatBodyUpdate(int systemHeaderStartRow, int? totalBodies, int knownBodies)
    {
        var builder = new StringBuilder();
        builder.Append(Helper.SetCursorPosition(41, systemHeaderStartRow));
        AppendBodyCount(builder, totalBodies, knownBodies);
        return builder.ToString();
    }

    private static void AppendBodyCount(StringBuilder builder, int? totalBodies, int knownBodies)
    {
        builder.Append("Bodies: ");

        if (totalBodies == null)
        {
            builder.Append(ANSI_Colors.BrightRed);
        }
        else if (knownBodies < totalBodies)
        {
            builder.Append(ANSI_Colors.BrightYellow);
        }
        else if (knownBodies == totalBodies)
        {
            builder.Append(ANSI_Colors.BrightGreen);
        }
        else
        {
            builder.Append(ANSI_Colors.BrightCyan);
        }

        builder.Append($"{knownBodies}/{totalBodies?.ToString() ?? "?"}");

        builder.Append(ANSI_Colors.Reset);
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
