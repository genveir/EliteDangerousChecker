using EliteDangerousChecker.Database.FromJournal;

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

        if (Notability.IsNotable(updatedBody))
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

        var bodyTable = BodyTableWriter.FormatBodyTable(solarSystemName, bodyData);
        await terminal.SendOutputLine(bodyTable);
        await terminal.UpdateView();
    }
}
