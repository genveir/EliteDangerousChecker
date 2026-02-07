namespace EliteDangerousChecker.JournalFile.JournalUpdate.FSD;
internal static class FSDCache
{
    private static readonly Dictionary<long, FSDTargetModel> _cache = new Dictionary<long, FSDTargetModel>();

    public static void AddOrUpdate(FSDTargetModel target)
    {
        if (target.SystemAddress == null)
            return;

        _cache[target.SystemAddress.Value] = target;
    }

    public static FSDTargetModel? Get(long? systemAddress)
    {
        if (systemAddress == null)
            return null;

        _cache.TryGetValue(systemAddress.Value, out var target);

        return target;
    }

    public static void Clear()
    {
        _cache.Clear();
    }
}
