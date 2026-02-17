using EliteDangerousChecker.Output.Models;
using EliteDangerousChecker.Output.Util;

namespace EliteDangerousChecker.Output.Writers;
public class SystemWriter
{
    private long currentSolarSystemId = 0;
    private int currentBodyId = 0;
    private int? totalBodies = null;
    private string solarSystemName = "";
    private List<BodyData> bodyData = [];
    private NavData[] navData = [];

    private int KnownBodies => bodyData
        .Where(bd => bd.BodyType == "Planet" || bd.BodyType == "Star")
        .Count();

    private readonly ITermController terminal;

    public SystemWriter(ITermController terminal)
    {
        this.terminal = terminal;
    }

    public bool IsReadyToWrite() => terminal.IsInitialized;

    public async Task UpdateBody(BodyData updatedBody)
    {
        var bodyIndex = bodyData.FindIndex(b => b.BodyId == updatedBody.BodyId);

        bool wasKnown = bodyIndex != -1;
        if (bodyIndex == -1)
        {
            bodyData.Add(updatedBody);
        }
        else
        {
            bodyData[bodyIndex] = updatedBody;
        }

        currentBodyId = updatedBody.BodyId;

        if (Helper.IsNotable(updatedBody))
        {
            await WriteSystem();
        }
        else if (!wasKnown)
        {
            await UpdateBodyCount();
        }
    }

    public async Task UpdateNavRoute(NavData[] updatedNavData)
    {
        navData = updatedNavData;

        await WriteSystem();
    }

    public async Task UpdateTotalBodies(int totalBodies)
    {
        this.totalBodies = totalBodies;

        await WriteSystem();
    }

    public async Task WriteSystem(long currentSolarSystemId, int currentBodyId, int? totalBodies, string solarSystemName, BodyData[] bodyData, NavData[] navData)
    {
        this.currentSolarSystemId = currentSolarSystemId;
        this.currentBodyId = currentBodyId;
        this.totalBodies = totalBodies;
        this.solarSystemName = solarSystemName;
        this.bodyData = [.. bodyData];
        this.navData = navData;

        await WriteSystem();
    }

    private async Task UpdateBodyCount()
    {
        var updateString = SystemInfoWriter.FormatBodyUpdate(1, totalBodies, KnownBodies);
        await terminal.SendOutputLine(updateString);
        await terminal.UpdateView();
    }

    private async Task WriteSystem()
    {
        await terminal.SendOutputLine(Helper.ClearTerminal());

        var systemHeader = SystemInfoWriter.FormatSystemHeader(solarSystemName, currentSolarSystemId, totalBodies, KnownBodies, navData);
        await terminal.SendOutputLine(systemHeader);

        var bodyTable = BodyTableWriter.FormatBodyTable(solarSystemName, [.. bodyData]);
        await terminal.SendOutputLine(bodyTable);
        await terminal.SendOutputLine("");

        var bodyInfo = BodyInfoWriter.FormatBodyInfo(solarSystemName, bodyData.SingleOrDefault(bd => bd.BodyId == currentBodyId));
        await terminal.SendOutputLine(bodyInfo);

        await terminal.UpdateView();
    }
}
