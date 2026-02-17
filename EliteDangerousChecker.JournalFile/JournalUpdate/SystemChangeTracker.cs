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
    private bool HasTotalBodyChanges = false;

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

    public void MarkTotalBodyCountChange()
    {
        Console.WriteLine("SystemChangeTracker Total Body Change marked");

        HasTotalBodyChanges = true;
    }

    public void MarkBodyChange(int bodyId)
    {
        Console.WriteLine($"SystemChangeTracker Body Change marked ({bodyId})");

        if (BodyChanges[WriteIndex].Contains(bodyId))
        {
            BodyChanges[WriteIndex].Remove(bodyId);
        }

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

                var solarSystemNameAndBodyCount = await GetSolarSystemNameAndBodyCount.Execute(CurrentSystemAddress);

                NavData[] navRoute = [];
                if (hasNavRouteChanges || general)
                {
                    navRoute = await NavRouteAccess.GetRoute();
                }

                if (general)
                {
                    await UpdateGeneral(solarSystemNameAndBodyCount.Name, solarSystemNameAndBodyCount.BodyCount, navRoute);
                    continue;
                }

                if (hasNavRouteChanges)
                {
                    await UpdateNavRoute(navRoute);
                }

                if (HasTotalBodyChanges)
                {
                    await UpdateTotalBodies(solarSystemNameAndBodyCount.BodyCount);
                }

                if (bodiesToUpdate.Length > 0)
                {
                    foreach (var bodyToUpdate in bodiesToUpdate)
                    {
                        await UpdateBody(bodyToUpdate);
                    }
                }
            }
            await Task.Delay(1000);
        }

        Console.WriteLine("SystemChangeTracker stopped");
    }

    private async Task UpdateGeneral(string systemName, int? bodyCount, NavData[] navRoute)
    {
        Console.WriteLine("updating whole system");

        var systemData = await GetBodyData.Execute(CurrentSystemAddress);
        var mappedBodies = systemData.Select(sd => new BodyData(sd)).ToArray();

        await systemWriter.WriteSystem(
            currentSolarSystemId: CurrentSystemAddress,
            currentBodyId: CurrentBody,
            totalBodies: bodyCount,
            solarSystemName: systemName,
            bodyData: mappedBodies,
            navData: navRoute);
    }

    private async Task UpdateNavRoute(NavData[] navRoute)
    {
        Console.WriteLine("updating nav route");

        await systemWriter.UpdateNavRoute(navRoute);
    }

    private async Task UpdateTotalBodies(int? totalBodies)
    {
        Console.WriteLine("updating total body count");

        await GetSolarSystemNameAndBodyCount.Execute(CurrentSystemAddress);

        if (totalBodies == null)
        {
            Console.WriteLine($"Unable to find body count for system address {CurrentSystemAddress}. Aborting total body count update.");
            return;
        }

        await systemWriter.UpdateTotalBodies(totalBodies.Value);
    }

    private async Task UpdateBody(int bodyToUpdate)
    {
        Console.WriteLine($"updating body {bodyToUpdate}");

        var bodyData = await GetBodyData.Execute(CurrentSystemAddress, bodyToUpdate);

        if (bodyData == null)
        {
            Console.WriteLine($"Unable to find body data for {bodyToUpdate}");
            return;
        }

        var mappedData = new BodyData(bodyData);

        await systemWriter.UpdateBody(mappedData);
    }

    public void Stop()
    {
        Running = false;
    }
}
