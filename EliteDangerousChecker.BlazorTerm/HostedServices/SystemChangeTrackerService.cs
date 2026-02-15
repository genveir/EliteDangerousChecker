using EliteDangerousChecker.JournalFile.PublicAbstractions;

namespace EliteDangerousChecker.BlazorTerm.HostedServices;

public class SystemChangeTrackerService : BackgroundService
{
    private readonly ISystemChangeTrackingService tracker;

    public SystemChangeTrackerService(ISystemChangeTrackingService tracker)
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
