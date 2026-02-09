namespace EliteDangerousChecker.JournalFile.NavRouteUpdate;
internal static class NavRouteCache
{
    private static readonly Dictionary<string, NavHopModel> cache = [];
    public static NavHopModel? Last => cache.Values.LastOrDefault() ?? null;

    public static void Set(NavRouteModel model)
    {
        cache.Clear();
        foreach (var hop in model.Route!)
        {
            cache[hop.StarSystem!] = hop;
        }
    }

    public static NavHopModel[] GetAll() => cache.Values.ToArray();
    public static bool TryGet(string systemName, out NavHopModel? result) => cache.TryGetValue(systemName, out result);
    public static void Clear() => cache.Clear();
}
