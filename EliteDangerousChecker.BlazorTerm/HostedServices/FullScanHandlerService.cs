
using EliteDangerousChecker.JournalFile;

namespace EliteDangerousChecker.BlazorTerm.HostedServices;

public class FullScanHandlerService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var fullScanHandlers = JournalFileServiceRegistration.FullScanHandlersToRun();

        foreach (var fullScanHandler in fullScanHandlers)
        {
            await fullScanHandler.DoFullScan();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
