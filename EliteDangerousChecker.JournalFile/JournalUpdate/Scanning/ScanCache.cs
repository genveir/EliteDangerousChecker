namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class ScanCache
{
    private static readonly Dictionary<int, ScanData> _cache = new Dictionary<int, ScanData>();
    public static void AddOrUpdate(int bodyId, ScanData scanData) => _cache[bodyId] = scanData;
    public static bool TryGet(int bodyId, out ScanData? result) => _cache.TryGetValue(bodyId, out result);
    public static void Clear() => _cache.Clear();
}
