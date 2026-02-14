
using EliteDangerousChecker.JournalFile.JournalUpdate;

namespace EliteDangerousChecker.BlazorTerm.HostedServices;

public class SystemChangeTrackerService : IHostedService
{
    private readonly SystemChangeTracker tracker;

    public SystemChangeTrackerService(SystemChangeTracker tracker)
    {
        this.tracker = tracker;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        tracker.StartOutputLoop();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        tracker.Stop();

        return Task.CompletedTask;
    }
}
