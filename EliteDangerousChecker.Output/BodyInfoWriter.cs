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

        foreach (var lifeData in bodyData.lifeData)
        {
            builder.AppendLine(FormatLifeDataForTable(lifeData));
        }

        return builder.ToString();
    }

    private static string GetHeader()
    {
        return $"{"Genus",-15}{"Species",-15}{"Value",-14}{"Scanned",-8}{"First",-5}";
    }

    private static string FormatLifeDataForTable(LifeData lifeData)
    {
        return $"{lifeData.Genus,-15}{lifeData.Species,-15}{lifeData.Value,-14}{lifeData.Scanned,-8}{lifeData.First,-5}";
    }
}
