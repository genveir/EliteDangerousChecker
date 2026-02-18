internal record ScanValueEntry(string PlanetType, bool Terraformable, int FSS, int FSS_FD, int FSS_DSS, int FSS_FD_DSS);

internal static class ScanValueTable
{
    private static readonly List<ScanValueEntry> Entries = new()
    {
        new ScanValueEntry("Ammonia World", false, 143463, 373004, 597762, 1724965),
        new ScanValueEntry("Earth-like World", false, 270290, 702753, 1126206, 3249900),
        new ScanValueEntry("Water World", false, 99747, 259343, 415613, 1199337),
        new ScanValueEntry("Water World", true, 268616, 698400, 1119231, 3229773),
        new ScanValueEntry("High Metal Content World", false, 14070, 36581, 58624, 169171),
        new ScanValueEntry("High Metal Content World", true, 163948, 426264, 683116, 1971272),
        new ScanValueEntry("Icy Body", false, 500, 1300, 1569, 4527),
        new ScanValueEntry("Metal Rich Body", false, 31632, 82244, 131802, 380341),
        new ScanValueEntry("Rocky Body", false, 500, 1300, 1476, 4260),
        new ScanValueEntry("Rocky Body", true, 129504, 336711, 539601, 1557130),
        new ScanValueEntry("Rocky Ice Body", false, 500, 1300, 1752, 5057),
        new ScanValueEntry("Class I Gas Giant", false, 3845, 9997, 16021, 46233),
        new ScanValueEntry("Class II Gas Giant", false, 28405, 73853, 118354, 341536),
        new ScanValueEntry("Class III Gas Giant", false, 995, 2587, 4145, 11963),
        new ScanValueEntry("Class IV Gas Giant", false, 1119, 2910, 4663, 13457),
        new ScanValueEntry("Class V Gas Giant", false, 966, 2510, 4023, 11609),
        new ScanValueEntry("Gas Giant with Ammonia-based Life", false, 774, 2014, 3227, 9312),
        new ScanValueEntry("Gas Giant with Water-based Life", false, 883, 2295, 3679, 10616),
        new ScanValueEntry("Helium-Rich Gas Giant", false, 900, 2339, 3749, 10818),
        new ScanValueEntry("Water Giant", false, 667, 1734, 2779, 8019),
    };

    public static int GetScanValue(string planetClass, bool terraformable, bool FD, bool DSS)
    {
        var entry = Entries.Find(e =>
            string.Equals(e.PlanetType, planetClass, StringComparison.OrdinalIgnoreCase) &&
            e.Terraformable == terraformable);

        if (entry == null)
            return 0; // Or throw, if you prefer

        if (FD && DSS) return entry.FSS_FD_DSS;
        if (FD) return entry.FSS_FD;
        if (DSS) return entry.FSS_DSS;
        return entry.FSS;
    }
}