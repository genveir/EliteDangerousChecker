using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;
internal static class SystemWriter
{
    private static string solarSystemName = "";
    private static GetBodyData.BodyData[] bodyData = [];

    public static void UpdateBody(GetBodyData.BodyData updatedBody)
    {
        var bodyIndex = Array.FindIndex(bodyData, b => b.BodyId == updatedBody.BodyId);

        if (bodyIndex == -1)
        {
            Console.WriteLine($"Unable to find body with id {updatedBody.BodyId} to update");
            return;
        }

        bodyData[bodyIndex] = updatedBody;

        if (IsNotable(updatedBody))
        {
            WriteSystem();
        }
    }

    public static void WriteSystem(string solarSystemName, GetBodyData.BodyData[] bodyData)
    {
        SystemWriter.solarSystemName = solarSystemName;
        SystemWriter.bodyData = bodyData;

        WriteSystem();
    }

    private static void WriteSystem()
    {
        //Console.Clear();
        Console.WriteLine(GetHeader());

        foreach (var body in bodyData)
        {
            if (IsNotable(body))
            {
                Write(solarSystemName, body);
            }
        }
    }

    private static string GetHeader()
    {
        return $"{"Body Name",-30}{"Terraform",-15}{"PC",-10}{"Bio",-4}{"Disc",-5}{"Map",-5}{"Foot",-5}";
    }

    private static bool IsNotable(GetBodyData.BodyData bodyData) =>
        bodyData.TerraformingState == "Terraformable" ||
        bodyData.SubType == "Water world" ||
        bodyData.SubType == "Earth-like world" ||
        bodyData.SubType == "Ammonia world" ||
        bodyData.BioSignals > 0 ||
        IsPrimaryStar(bodyData.Name);

    private static void Write(string solarSystemName, GetBodyData.BodyData bodyData)
    {
        WriteName(solarSystemName, bodyData.Name);
        WriteTerraform(bodyData.TerraformingState);
        WriteBodySubType(bodyData.SubType, bodyData.BodyType);
        WriteBioData(bodyData.BioSignals);
        WriteExploration(bodyData.Discovered);
        WriteExploration(bodyData.Mapped, bodyData.BodyType);
        WriteExploration(bodyData.Landed, bodyData.BodyType);
        Console.WriteLine();
    }

    private static bool IsPrimaryStar(string name) =>
        name.Length == 0 || (name == " A");

    private static void WriteName(string solarSystemName, string name)
    {
        if (IsPrimaryStar(name))
        {
            Console.Write($"{solarSystemName}{name}".PadRight(30));
            return;
        }
        Console.Write(name.Trim().PadRight(30));
    }

    private static void WriteTerraform(string terraformState)
    {
        var isTerraformable = terraformState == "Terraformable";

        if (!isTerraformable)
        {
            Console.Write("".PadRight(15));
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("TERRAFORMABLE".PadRight(15));
        Console.ResetColor();
    }

    private static void WriteBodySubType(string planetClass, string bodyType)
    {
        if (bodyType == "Star")
        {
            Console.Write(planetClass.Split(' ').First().PadRight(10));
            return;
        }

        var formattedClass = FormatPlanetClass(planetClass);

        if (formattedClass is "Water" or "Earth-like" or "Ammonia")
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        Console.Write(formattedClass.PadRight(10));

        Console.ResetColor();
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

    private static void WriteBioData(int bioSignals)
    {
        if (bioSignals == 0)
        {
            Console.Write("".PadRight(4));
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(bioSignals.ToString().PadRight(4));
        Console.ResetColor();
    }

    private static void WriteExploration(string status, string bodyType)
    {
        if (bodyType == "Star")
        {
            Console.Write("".PadRight(5));
            return;
        }

        WriteExploration(status);
    }

    private static void WriteExploration(string status)
    {
        if (status == "No")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("NO".PadRight(5));
            Console.ResetColor();
        }
        else if (status == "Yes")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Yes".PadRight(5));
            Console.ResetColor();
        }
        else if (status == "Commander")
        {
            Console.Write("CMDR".PadRight(5));
        }
        else if (status == null)
        {
            Console.Write("Db".PadRight(5));
        }
    }
}
