using EliteDangerousChecker.Output.Models;
using EliteDangerousChecker.Output.Util;

namespace EliteDangerousChecker.Output.Writers;
public class SystemWriter
{
    private long currentSolarSystemId = 0;
    private int currentBodyId = 0;
    private string solarSystemName = "";
    private BodyData[] bodyData = [];
    private NavData[] navData = [];

    private readonly ITermController terminal;

    public SystemWriter(ITermController terminal)
    {
        this.terminal = terminal;
    }

    public bool IsReadyToWrite() => terminal.IsInitialized;

    public async Task UpdateBody(BodyData updatedBody)
    {
        var bodyIndex = Array.FindIndex(bodyData, b => b.BodyId == updatedBody.BodyId);

        bodyData[bodyIndex] = updatedBody;
        currentBodyId = updatedBody.BodyId;

        if (Helper.IsNotable(updatedBody))
        {
            await WriteSystem();
        }
    }

    public async Task UpdateNavRoute(NavData[] updatedNavData)
    {
        navData = updatedNavData;

        await WriteSystem();
    }

    public async Task WriteSystem(long currentSolarSystemId, int currentBodyId, string solarSystemName, BodyData[] bodyData, NavData[] navData)
    {
        this.currentSolarSystemId = currentSolarSystemId;
        this.currentBodyId = currentBodyId;
        this.solarSystemName = solarSystemName;
        this.bodyData = bodyData;
        this.navData = navData;

        await WriteSystem();
    }

    private async Task WriteSystem()
    {
        await terminal.Clear();

        var systemHeader = SystemInfoWriter.FormatSystemHeader(solarSystemName, currentSolarSystemId, navData);
        await terminal.SendOutputLine(systemHeader);

        var bodyTable = BodyTableWriter.FormatBodyTable(solarSystemName, bodyData);
        await terminal.SendOutputLine(bodyTable);
        await terminal.SendOutputLine("");

        var bodyInfo = BodyInfoWriter.FormatBodyInfo(solarSystemName, bodyData.SingleOrDefault(bd => bd.BodyId == currentBodyId));
        await terminal.SendOutputLine(bodyInfo);

        await terminal.UpdateView();
    }
}
