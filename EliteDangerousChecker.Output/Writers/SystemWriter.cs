using EliteDangerousChecker.Output.Models;
using EliteDangerousChecker.Output.Util;

namespace EliteDangerousChecker.Output.Writers;
public class SystemWriter
{
    private long currentSolarSystemId = 0;
    private int currentBodyId = 0;
    private string solarSystemName = "";
    private BodyData[] bodyData = [];

    private readonly ITermController terminal;

    public SystemWriter(ITermController terminal)
    {
        this.terminal = terminal;
    }

    public bool IsReadyToWrite() => terminal.IsInitialized;

    public async Task UpdateBody(BodyData updatedBody)
    {
        var bodyIndex = Array.FindIndex(bodyData, b => b.BodyId == updatedBody.BodyId);

        if (bodyIndex == -1)
        {
            Console.WriteLine($"Unable to find body with id {updatedBody.BodyId} to update");
            return;
        }

        bodyData[bodyIndex] = updatedBody;
        currentBodyId = updatedBody.BodyId;

        if (Helper.IsNotable(updatedBody))
        {
            await WriteSystem();
        }
    }

    public async Task WriteSystem(long currentSolarSystemId, int currentBodyId, string solarSystemName, BodyData[] bodyData)
    {
        this.currentSolarSystemId = currentSolarSystemId;
        this.currentBodyId = currentBodyId;
        this.solarSystemName = solarSystemName;
        this.bodyData = bodyData;

        await WriteSystem();
    }

    private async Task WriteSystem()
    {
        await terminal.Clear();

        await WriteSystemHeader();

        var bodyTable = BodyTableWriter.FormatBodyTable(solarSystemName, bodyData);
        await terminal.SendOutputLine(bodyTable);
        await terminal.SendOutputLine($"    Total Scan Value: {bodyData.Sum(b => b.GetScanValue()) / 1000000.0d:N2}M");
        await terminal.SendOutputLine($"    Total Bio Value:  {bodyData.Sum(b => b.GetBioValue()) / 1000000.0d:N2}M");
        await terminal.SendOutputLine($"    Total Value:      {bodyData.Sum(b => b.GetScanValue() + b.GetBioValue()) / 1000000.0d:N2}M");
        await terminal.SendOutputLine(BAR);
        await terminal.SendOutputLine("");

        var bodyInfo = BodyInfoWriter.FormatBodyInfo(solarSystemName, bodyData.SingleOrDefault(bd => bd.BodyId == currentBodyId));
        await terminal.SendOutputLine(bodyInfo);

        await terminal.UpdateView();
    }

    private async Task WriteSystemHeader()
    {
        await terminal.SendOutputLine($"System: {solarSystemName} ({currentSolarSystemId})");

        await terminal.SendOutputLine(BAR);
        await terminal.SendOutputLine(" ");
    }

    private const string BAR = "################################################################################################################################################################";
}
