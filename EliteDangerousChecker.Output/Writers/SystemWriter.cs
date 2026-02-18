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
    private ScanValues tripScanValues = new(0, 0, 0);

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
        Console.WriteLine($"updating body {updatedBody.BodyId}");

        var bodyIndex = bodyData.FindIndex(b => b.BodyId == updatedBody.BodyId);

        bool wasKnown = bodyIndex != -1;
        if (bodyIndex == -1)
        {
            Console.WriteLine("adding new body");
            bodyData.Add(updatedBody);

            var (uncertainty, uncertaintyValue, guaranteedValue) = updatedBody.GetBioValue();

            tripScanValues = new ScanValues(
                tripScanValues.ScanValue + updatedBody.ScanValue,
                tripScanValues.Uncertainty + uncertainty,
                tripScanValues.BioValue + uncertaintyValue + guaranteedValue);
        }
        else
        {
            var scanDiff = updatedBody.ScanValue - bodyData[bodyIndex].ScanValue;
            var (oldUncertainty, oldUncertaintyValue, oldGuaranteedValue) = bodyData[bodyIndex].GetBioValue();
            var (newUncertainty, newUncertaintyValue, newGuaranteedValue) = updatedBody.GetBioValue();
            var (uncertaintyDiff, uncertaintyValueDiff, guaranteedValueDiff) = (
                newUncertainty - oldUncertainty,
                newUncertaintyValue - oldUncertaintyValue,
                newGuaranteedValue - oldGuaranteedValue);

            tripScanValues = new ScanValues(
                tripScanValues.ScanValue + scanDiff,
                tripScanValues.Uncertainty + uncertaintyDiff,
                tripScanValues.BioValue + uncertaintyValueDiff + guaranteedValueDiff);

            Console.WriteLine("updating existing body");
            bodyData[bodyIndex] = updatedBody;
        }

        currentBodyId = updatedBody.BodyId;

        if (Helper.IsNotable(updatedBody))
        {
            Console.WriteLine("notable body, writing system");
            await WriteSystem();
        }
        else if (!wasKnown)
        {
            Console.WriteLine("new body is not notable, but it is a new discovery, updating body count");
            await UpdateBodyCount();
        }

        bodyData = bodyData.OrderBy(b => b.Name).ToList();
    }

    public async Task UpdateNavRoute(NavData[] updatedNavData)
    {
        navData = updatedNavData;

        await WriteSystem();
    }

    public async Task UpdateTotalBodies(int totalBodies)
    {
        this.totalBodies = totalBodies;

        await UpdateBodyCount();
    }

    public async Task WriteSystem(long currentSolarSystemId, int currentBodyId, int? totalBodies, string solarSystemName, BodyData[] bodyData, NavData[] navData, ScanValues tripScanValues)
    {
        this.currentSolarSystemId = currentSolarSystemId;
        this.currentBodyId = currentBodyId;
        this.totalBodies = totalBodies;
        this.solarSystemName = solarSystemName;
        this.bodyData = bodyData.OrderBy(b => b.Name).ToList();
        this.navData = navData;
        this.tripScanValues = tripScanValues;

        await WriteSystem();
    }

    private async Task UpdateBodyCount()
    {
        Console.WriteLine("updating body count in system header");

        var updateString = SystemInfoWriter.FormatBodyUpdate(2, totalBodies, KnownBodies);
        await terminal.SendOutputLine(updateString);
        await terminal.UpdateView();
    }

    private async Task WriteSystem()
    {
        Console.WriteLine("writing system");

        await terminal.SendOutputLine(Helper.ClearTerminal());

        var systemHeader = SystemInfoWriter.FormatSystemHeader(solarSystemName, currentSolarSystemId, totalBodies, KnownBodies, navData);
        await terminal.SendOutputLine(systemHeader);

        var bodyTable = BodyTableWriter.FormatBodyTable(solarSystemName, [.. bodyData], tripScanValues);
        await terminal.SendOutputLine(bodyTable);
        await terminal.SendOutputLine("");

        var bodyInfo = BodyInfoWriter.FormatBodyInfo(solarSystemName, bodyData.SingleOrDefault(bd => bd.BodyId == currentBodyId));
        await terminal.SendOutputLine(bodyInfo);

        await terminal.UpdateView();
    }
}
