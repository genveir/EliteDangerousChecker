using EliteDangerousChecker.Database.FromJournal;
using EliteDangerousChecker.JournalFile.NavRouteUpdate;
using EliteDangerousChecker.JournalFile.PublicAbstractions;
using EliteDangerousChecker.Output.Models;
using EliteDangerousChecker.Output.Writers;

namespace EliteDangerousChecker.JournalFile.JournalUpdate;

internal class SystemChangeTracker : ISystemChangeTracker, ISystemChangeTrackingService
{
    private readonly SystemWriter systemWriter;

    private bool PrintedInitial = false;

    private bool HasGeneralChanges = false;
    private readonly List<int>[] BodyChanges = [[], []];
    private int WriteIndex = 0;

    private bool HasNavRouteChanges = false;

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

    public void MarkNavRouteChange()
    {
        Console.WriteLine($"SystemChangeTracker Nav Route Change marked");

        HasNavRouteChanges = true;
    }

    public void MarkBodyChange(int bodyId)
    {
        Console.WriteLine($"SystemChangeTracker Body Change marked ({bodyId})");

        if (BodyChanges[WriteIndex].Contains(bodyId))
            return;

        BodyChanges[WriteIndex].Add(bodyId);
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

            var hasChanges = HasGeneralChanges || (BodyChanges[WriteIndex].Count > 0) || HasNavRouteChanges;

            if (hasChanges)
            {
                bool general = HasGeneralChanges;
                HasGeneralChanges = false;

                WriteIndex = 1 - WriteIndex;
                var bodiesToUpdate = BodyChanges[1 - WriteIndex].ToArray();
                BodyChanges[1 - WriteIndex].Clear();

                bool hasNavRouteChanges = HasNavRouteChanges;
                HasNavRouteChanges = false;

                var solarSystemName = await GetSolarSystemName.Execute(CurrentSystemAddress);

                NavData[] navRoute = [];
                if (hasNavRouteChanges || general)
                {
                    navRoute = await NavRouteAccess.GetRoute();
                }

                if (general)
                {
                    Console.WriteLine("updating whole system");

                    var systemData = await GetBodyData.Execute(CurrentSystemAddress);
                    var mappedBodies = systemData.Select(sd => new BodyData(sd)).ToArray();

                    await systemWriter.WriteSystem(CurrentSystemAddress, CurrentBody, solarSystemName, mappedBodies, navRoute);
                }
                else if (hasNavRouteChanges)
                {
                    Console.WriteLine("updating nav route");

                    await systemWriter.UpdateNavRoute(navRoute);
                }
                else if (bodiesToUpdate.Length > 0)
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
