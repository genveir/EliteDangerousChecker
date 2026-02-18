using EliteDangerousChecker.Output.Models;
using EliteDangerousChecker.Output.Util;
using System.Text;

namespace EliteDangerousChecker.Output.Writers;
internal static class BodyTableWriter
{
    public static string GetHeader()
    {
        return $"{"Body Name",-30}{"Terraform",-15}{"PC",-10}{"Bio",-8}{"Disc",-5}{"Map",-5}{"Foot",-5}{"ScanValue",-15}{"BioValue",-15}";
    }

    public static string FormatBodyTable(string solarSystemName, BodyData[] bodyData, ScanValues tripScanValues)
    {
        StringBuilder builder = new();
        builder.AppendLine(GetHeader());

        foreach (var body in bodyData)
        {
            if (Helper.IsNotable(body))
            {
                builder.AppendLine(FormatBodyForTable(solarSystemName, body));
            }
        }
        AppendBodyValues(builder, bodyData, tripScanValues);
        return builder.ToString();
    }

    private static string FormatBodyForTable(string solarSystemName, BodyData bodyData)
    {
        StringBuilder builder = new();

        AppendName(builder, solarSystemName, bodyData);
        AppendTerraform(builder, bodyData.TerraformingState);
        AppendBodySubType(builder, bodyData.SubType, bodyData.BodyType);
        AppendBioData(builder, bodyData.BioSignals, bodyData.LifeData.Sum(ld => ld.Scanned == "Commander" ? 1 : 0));
        AppendExploration(builder, bodyData.Discovered);
        AppendExploration(builder, bodyData.Mapped, bodyData.BodyType);
        AppendExploration(builder, bodyData.Landed, bodyData.BodyType);
        builder.Append(bodyData.ScanValue.ToString().PadRight(15));
        AppendBioValue(builder, bodyData);

        return builder.ToString();
    }

    private static void AppendName(StringBuilder builder, string solarSystemName, BodyData bodyData)
    {
        if (bodyData.MainStar)
        {
            builder.Append($"{solarSystemName}{bodyData.Name}".PadRight(30));
            return;
        }
        builder.Append(bodyData.Name.Trim().PadRight(30));
    }

    private static void AppendTerraform(StringBuilder builder, string terraformState)
    {
        var isTerraformable = terraformState == "Terraformable";

        if (!isTerraformable)
        {
            builder.Append("".PadRight(15));
            return;
        }

        builder.Append(ANSI_Colors.BrightGreen);
        builder.Append("TERRAFORMABLE".PadRight(15));
        builder.Append(ANSI_Colors.Reset);
    }

    private static void AppendBodySubType(StringBuilder builder, string planetClass, string bodyType)
    {
        if (bodyType == "Star")
        {
            builder.Append(planetClass.Split(' ').First().PadRight(10));
            return;
        }

        var formattedClass = FormatPlanetClass(planetClass);

        if (formattedClass is "Water" or "Earth-like" or "Ammonia")
        {
            builder.Append(ANSI_Colors.BrightGreen);
        }

        builder.Append(formattedClass.PadRight(10));

        builder.Append(ANSI_Colors.Reset);
    }

    private static string FormatPlanetClass(string planetClass)
    {
        return planetClass switch
        {
            "High metal content world" => "HMCW",
            "Class III gas giant" => "GG III",
            "Icy body" => "Icy",
            "Water world" => "Water",
            "Class I gas giant" => "GG I",
            "Rocky Ice world" => "Rocky Ice",
            "Metal-rich body" => "Metal-rich",
            "Rocky body" => "Rocky",
            "Gas giant with water-based life" => "GG W life",
            "Class II gas giant" => "GG II",
            "Earth-like world" => "Earth-like",
            "Class V gas giant" => "GG V",
            "Class IV gas giant" => "GG IV",
            "Ammonia world" => "Ammonia",
            "Gas giant with ammonia-based life" => "GG A life",
            "Water giant" => "Water giant",
            _ => planetClass ?? "Unknown"
        };
    }



    private static void AppendBioData(StringBuilder builder, int bioSignals, int numScanned)
    {
        if (bioSignals == 0)
        {
            builder.Append("".PadRight(8));
            return;
        }

        if (numScanned == 0)
        {
            builder.Append(ANSI_Colors.BrightRed);
        }
        else if (numScanned > 0 && numScanned < bioSignals)
        {
            builder.Append(ANSI_Colors.BrightYellow);
        }
        else
        {
            builder.Append(ANSI_Colors.BrightGreen);
        }

        var bioText = numScanned + " / " + bioSignals;

        builder.Append(bioText.PadRight(8));
        builder.Append(ANSI_Colors.Reset);
    }

    private static void AppendExploration(StringBuilder builder, string status, string bodyType)
    {
        if (bodyType == "Star")
        {
            builder.Append("".PadRight(5));
            return;
        }

        AppendExploration(builder, status);
    }

    private static void AppendExploration(StringBuilder builder, string status)
    {
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
            builder.Append("Db".PadRight(5));
        }

        builder.Append(ANSI_Colors.Reset);
    }

    private static void AppendBioValue(StringBuilder builder, BodyData bodyData)
    {
        var (uncertainty, uncertaintyValue, guaranteedValue) = bodyData.GetBioValue();

        var uncertaintyText = uncertainty == 0 ? "" : "+";

        builder.Append((uncertaintyValue + guaranteedValue) + uncertaintyText);
    }

    private static void AppendBodyValues(StringBuilder builder, BodyData[] bodyData, ScanValues tripScanValues)
    {
        var (hasUncertainty, scanValue, bioValue, totalValue) = GetBodyValues(bodyData);

        var scanValueText = $"{scanValue / 1000000.0d:N2}M";
        var bioValueText = $"{bioValue / 1000000.0d:N2}M";
        var totalValueText = $"{totalValue / 1000000.0d:N2}M";

        var tripScanValueText = $"{tripScanValues.ScanValue / 1000000.0d:N2}M";
        var tripBioValueText = $"{tripScanValues.BioValue / 1000000.0d:N2}M";
        var tripTotalValueText = $"{(tripScanValues.ScanValue + tripScanValues.BioValue) / 1000000.0d:N2}M";

        var uncertaintyText = hasUncertainty ? "+" : " ";

        builder.Append($"    Total Scan Value:{scanValueText,9} ".PadRight(80));
        builder.AppendLine($"Trip Scan Value: {tripScanValueText,10}");

        builder.Append($"    Total Bio Value: {bioValueText,9}{uncertaintyText}".PadRight(80));
        builder.AppendLine($"Trip Bio Value:  {tripBioValueText,10}{uncertaintyText}");

        builder.Append($"    Total Value:     {totalValueText,9}{uncertaintyText}".PadRight(80));
        builder.AppendLine($"Trip Total Value:{tripTotalValueText,10}{uncertaintyText}");

        builder.AppendLine(Helper.BAR);
    }

    private static (bool hasUncertainty, long scanValue, long bioValue, long totalValue) GetBodyValues(BodyData[] bodyData)
    {
        if (bodyData.Length < 2)
            return (false, 0, 0, 0);

        var (uncertainty, uncertaintyValue, guaranteedValue) = bodyData
            .Select(b => b.GetBioValue())
            .Aggregate((a, b) => (
                a.uncertainty + b.uncertainty,
                a.uncertaintyValue + b.uncertaintyValue,
                a.guaranteedValue + b.guaranteedValue));

        var scanValue = bodyData.Sum(b => b.ScanValue);
        var bioValue = uncertaintyValue + guaranteedValue;
        var totalValue = scanValue + bioValue;

        return (uncertainty > 0, scanValue, bioValue, totalValue);
    }
}
