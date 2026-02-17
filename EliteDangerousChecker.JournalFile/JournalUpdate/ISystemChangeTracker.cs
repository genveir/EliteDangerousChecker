namespace EliteDangerousChecker.JournalFile.JournalUpdate;

public interface ISystemChangeTracker
{
    void MarkBodyChange(int bodyId);
    void MarkTotalBodyCountChange();
    void MarkNavRouteChange();
    void MarkGeneralChange();
    void MarkSystemChange(long newSystemAddress);
}