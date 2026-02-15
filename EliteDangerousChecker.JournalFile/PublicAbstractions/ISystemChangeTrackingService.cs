namespace EliteDangerousChecker.JournalFile.PublicAbstractions;
public interface ISystemChangeTrackingService
{
    Task StartOutputLoop();
    void Stop();
}
