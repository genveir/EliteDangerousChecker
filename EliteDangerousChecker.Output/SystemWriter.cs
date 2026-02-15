using EliteDangerousChecker.Database.FromJournal;
using System.Text;

namespace EliteDangerousChecker.Output;
public class SystemWriter
{
    private string solarSystemName = "";
    private GetBodyData.BodyData[] bodyData = [];

    private readonly ITermController terminal;

    public SystemWriter(ITermController terminal)
    {
        this.terminal = terminal;
    }

    public bool IsReadyToWrite() => terminal.IsInitialized;

    public async Task UpdateBody(GetBodyData.BodyData updatedBody)
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
            await WriteSystem();
        }
    }

    public async Task WriteSystem(string solarSystemName, GetBodyData.BodyData[] bodyData)
    {
        this.solarSystemName = solarSystemName;
        this.bodyData = bodyData;

        await WriteSystem();
    }

    private async Task WriteSystem()
    {
        await terminal.Clear();
        await terminal.SendOutputLine(GetHeader());

        foreach (var body in bodyData)
        {
            if (IsNotable(body))
            {
                var bodyLine = FormatString(solarSystemName, body);

                await terminal.SendOutputLine(bodyLine);
            }
        }

        await terminal.DoTick();
    }

    private string GetHeader()
    {
        return $"{"Body Name",-30}{"Terraform",-15}{"PC",-10}{"Bio",-4}{"Disc",-5}{"Map",-5}{"Foot",-5}";
    }

    private bool IsNotable(GetBodyData.BodyData bodyData) =>
        bodyData.TerraformingState == "Terraformable" ||
        bodyData.SubType == "Water world" ||
        bodyData.SubType == "Earth-like world" ||
        bodyData.SubType == "Ammonia world" ||
        bodyData.BioSignals > 0 ||
        IsPrimaryStar(bodyData.Name);

    private string FormatString(string solarSystemName, GetBodyData.BodyData bodyData)
    {
        StringBuilder builder = new();

        AppendName(builder, solarSystemName, bodyData.Name);
        AppendTerraform(builder, bodyData.TerraformingState);
        AppendBodySubType(builder, bodyData.SubType, bodyData.BodyType);
        AppendBioData(builder, bodyData.BioSignals);
        AppendExploration(builder, bodyData.Discovered);
        AppendExploration(builder, bodyData.Mapped, bodyData.BodyType);
        AppendExploration(builder, bodyData.Landed, bodyData.BodyType);

        return builder.ToString();
    }

    private bool IsPrimaryStar(string name) =>
        name.Length == 0 || name == " A";

    private void AppendName(StringBuilder builder, string solarSystemName, string name)
    {
        if (IsPrimaryStar(name))
        {
            builder.Append($"{solarSystemName}{name}".PadRight(30));
            return;
        }
        builder.Append(name.Trim().PadRight(30));
    }

    private void AppendTerraform(StringBuilder builder, string terraformState)
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

    private void AppendBodySubType(StringBuilder builder, string planetClass, string bodyType)
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

    private string FormatPlanetClass(string planetClass)
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

    private void AppendBioData(StringBuilder builder, int bioSignals)
    {
        if (bioSignals == 0)
        {
            builder.Append("".PadRight(4));
            return;
        }

        builder.Append(ANSI_Colors.BrightGreen);
        builder.Append(bioSignals.ToString().PadRight(4));
        builder.Append(ANSI_Colors.Reset);
    }

    private void AppendExploration(StringBuilder builder, string status, string bodyType)
    {
        if (bodyType == "Star")
        {
            builder.Append("".PadRight(5));
            return;
        }

        AppendExploration(builder, status);
    }

    private void AppendExploration(StringBuilder builder, string status)
    {
        if (status == "No")
        {
            builder.Append(ANSI_Colors.BrightGreen);
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
}
