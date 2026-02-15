using EliteDangerousChecker.Database.FromJournal;
using EliteDangerousChecker.JournalFile.PublicAbstractions;
using EliteDangerousChecker.Output;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;

internal class SystemChangeTracker : ISystemChangeTracker, ISystemChangeTrackingService
{
    private readonly SystemWriter systemWriter;

    private bool PrintedInitial = false;

    private bool HasGeneralChanges = false;
    private List<int> BodyChanges = [];

    private long CurrentSystemAddress = 0;
    private int CurrentBody = 0;

    private bool Running = true;

    public SystemChangeTracker(SystemWriter systemWriter)
    {
        this.systemWriter = systemWriter;
    }

    public void MarkSystemChange(long newSystemAddress)
    {
        CurrentSystemAddress = newSystemAddress;
        CurrentBody = 0;
        HasGeneralChanges = true;
    }

    public void MarkGeneralChange()
    {
        Console.WriteLine($"SystemChangeTracker General Change marked");

        HasGeneralChanges = true;
    }

    public void MarkBodyChange(int bodyId)
    {
        Console.WriteLine($"SystemChangeTracker Body Change marked ({bodyId})");

        BodyChanges.Add(bodyId);
        CurrentBody = bodyId;
    }

    public async Task StartOutputLoop()
    {
        Console.WriteLine("Starting system change tracker output loop.");

        while (Running)
        {
            if (!PrintedInitial)
            {
                if (systemWriter.IsReadyToWrite())
                {
                    PrintedInitial = true;
                    await SystemLogPrinter.PrintLogForCurrentSystem(this);
                }
            }

            var hasChanges = HasGeneralChanges || (BodyChanges.Count > 0);

            if (hasChanges)
            {
                bool general = HasGeneralChanges;
                int[] bodiesToUpdate = general ? [] : BodyChanges.ToArray();

                HasGeneralChanges = false;
                BodyChanges.Clear();

                var solarSystemName = await GetSolarSystemName.Execute(CurrentSystemAddress);

                if (general)
                {
                    Console.WriteLine("updating whole system");
                    var systemData = await GetBodyData.Execute(CurrentSystemAddress);
                    var mappedData = systemData.Select(sd => new BodyData(sd)).ToArray();

                    await systemWriter.WriteSystem(CurrentSystemAddress, CurrentBody, solarSystemName, mappedData);
                }
                if (bodiesToUpdate.Length > 0)
                {
                    foreach (var bodyToUpdate in bodiesToUpdate)
                    {
                        Console.WriteLine($"updating body {bodyToUpdate}");

                        var bodyData = await GetBodyData.Execute(CurrentSystemAddress, bodyToUpdate);

                        if (bodyData == null)
                        {
                            Console.WriteLine($"Unable to find body data for {bodyToUpdate}");
                            continue;
                        }

                        var mappedData = new BodyData(bodyData);

                        await systemWriter.UpdateBody(mappedData);
                    }
                }
            }
            await Task.Delay(1000);
        }

        Console.WriteLine("SystemChangeTracker stopped");
    }

    public void Stop()
    {
        Running = false;
    }
}
