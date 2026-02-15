using System.Text;

namespace EliteDangerousChecker.Output;
internal static class BodyInfoWriter
{
    public static string FormatBodyInfo(string solarSystemName, BodyData? bodyData)
    {
        if (bodyData == null)
        {
            return "Not currently on a body.";
        }

        var builder = new StringBuilder();

        builder.AppendLine($"Current Body: {solarSystemName}{bodyData.Name} ({bodyData.BodyId})");
        builder.AppendLine($"Type: {bodyData.SubType}");
        builder.AppendLine();
        builder.AppendLine(GetHeader());

        foreach (var lifeData in bodyData.LifeData)
        {
            builder.AppendLine(FormatLifeDataForTable(lifeData));
        }

        return builder.ToString();
    }

    private static string GetHeader()
    {
        return $"{"Genus",-15}{"Species",-15}{"Value",-14}{"Bonus",-14}{"Scanned",-8}";
    }

    private static string FormatLifeDataForTable(LifeData lifeData)
    {
        var scanText = FormatScanned(lifeData.Scanned);
        var speciesText = lifeData.Species?.Replace(lifeData.Genus, "").Trim();

        return $"{lifeData.Genus,-15}{speciesText,-15}{lifeData.Value,-14}{lifeData.Bonus,-14}{scanText,-8}";
    }

    private static string FormatScanned(string status)
    {
        var builder = new StringBuilder();

        if (status == "No")
        {
            builder.Append(ANSI_Colors.BrightYellow);
            builder.Append("NO".PadRight(5));
        }
        else if (status == "Yes")
        {
            builder.Append(ANSI_Colors.BrightRed);
            builder.Append("Yes".PadRight(5));
        }
        else if (status == "Commander")
        {
            builder.Append(ANSI_Colors.BrightGreen);
            builder.Append("CMDR".PadRight(5));
        }
        else if (status == null)
        {
            builder.Append("Unseen".PadRight(5));
        }

        builder.Append(ANSI_Colors.Reset);

        return builder.ToString();
    }
}
