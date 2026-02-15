using EliteDangerousChecker.Database.FromJournal;

namespace EliteDangerousChecker.Output;
public class SystemWriter
{
    private long currentSolarSystemId = 0;
    private int currentBodyId = 0;
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

        if (Notability.IsNotable(updatedBody))
        {
            await WriteSystem();
        }
    }

    public async Task WriteSystem(long currentSolarSystemId, int currentBodyId, string solarSystemName, GetBodyData.BodyData[] bodyData)
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
        await terminal.UpdateView();
    }

    private async Task WriteSystemHeader()
    {
        await terminal.SendOutputLine($"System: {solarSystemName} ({currentSolarSystemId})");
        var bar = new string(Enumerable.Repeat('#', 80).ToArray());

        await terminal.SendOutputLine(bar);
        await terminal.SendOutputLine(" ");
    }
}
