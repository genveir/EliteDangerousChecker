namespace EliteDangerousChecker.JournalFile.PublicAbstractions;

public interface IJournalFolderWatcher : IDisposable
{
    Task StartWatching(CancellationToken cancellationToken);
}