namespace EliteDangerousChecker.JournalFile.JournalUpdate;

public interface ISystemChangeTracker
{
    void MarkBodyChange(int bodyId);
    void MarkGeneralChange();
    void MarkSystemChange(long newSystemAddress);
}