
using EliteDangerousChecker.JournalFile.JournalUpdate;

namespace EliteDangerousChecker.BlazorTerm.HostedServices;

public class SystemChangeTrackerService : BackgroundService
{
    private readonly SystemChangeTracker tracker;

    public SystemChangeTrackerService(SystemChangeTracker tracker)
    {
        this.tracker = tracker;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        tracker.Stop();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await tracker.StartOutputLoop();
    }
}
