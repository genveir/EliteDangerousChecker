namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class ScanCache
{
    private static readonly Dictionary<int, ScanData> cache = new Dictionary<int, ScanData>();

    public static ScanData[] GetAll() => cache.Values.ToArray();
    public static void AddOrUpdate(int bodyId, ScanData scanData) => cache[bodyId] = scanData;
    public static bool TryGet(int bodyId, out ScanData? result) => cache.TryGetValue(bodyId, out result);
    public static void Clear() => cache.Clear();
}
