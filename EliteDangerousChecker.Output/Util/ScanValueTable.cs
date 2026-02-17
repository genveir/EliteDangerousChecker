internal class ScanValueEntry
{
    public string PlanetType { get; set; }
    public bool Terraformable { get; set; }
    public int FSS { get; set; }
    public int FSS_FD { get; set; }
    public int FSS_DSS { get; set; }
    public int FSS_FD_DSS { get; set; }
}

internal static class ScanValueTable
{
    private static readonly List<ScanValueEntry> Entries = new()
    {
        new ScanValueEntry { PlanetType = "Ammonia World", Terraformable = false, FSS = 143463, FSS_FD = 373004, FSS_DSS = 597762, FSS_FD_DSS = 1724965 },
        new ScanValueEntry { PlanetType = "Earth-like World", Terraformable = false, FSS = 270290, FSS_FD = 702753, FSS_DSS = 1126206, FSS_FD_DSS = 3249900 },
        new ScanValueEntry { PlanetType = "Water World", Terraformable = false, FSS = 99747, FSS_FD = 259343, FSS_DSS = 415613, FSS_FD_DSS = 1199337 },
        new ScanValueEntry { PlanetType = "Water World", Terraformable = true, FSS = 268616, FSS_FD = 698400, FSS_DSS = 1119231, FSS_FD_DSS = 3229773 },
        new ScanValueEntry { PlanetType = "High Metal Content World", Terraformable = false, FSS = 14070, FSS_FD = 36581, FSS_DSS = 58624, FSS_FD_DSS = 169171 },
        new ScanValueEntry { PlanetType = "High Metal Content World", Terraformable = true, FSS = 163948, FSS_FD = 426264, FSS_DSS = 683116, FSS_FD_DSS = 1971272 },
        new ScanValueEntry { PlanetType = "Icy Body", Terraformable = false, FSS = 500, FSS_FD = 1300, FSS_DSS = 1569, FSS_FD_DSS = 4527 },
        new ScanValueEntry { PlanetType = "Metal Rich Body", Terraformable = false, FSS = 31632, FSS_FD = 82244, FSS_DSS = 131802, FSS_FD_DSS = 380341 },
        new ScanValueEntry { PlanetType = "Rocky Body", Terraformable = false, FSS = 500, FSS_FD = 1300, FSS_DSS = 1476, FSS_FD_DSS = 4260 },
        new ScanValueEntry { PlanetType = "Rocky Body", Terraformable = true, FSS = 129504, FSS_FD = 336711, FSS_DSS = 539601, FSS_FD_DSS = 1557130 },
        new ScanValueEntry { PlanetType = "Rocky Ice Body", Terraformable = false, FSS = 500, FSS_FD = 1300, FSS_DSS = 1752, FSS_FD_DSS = 5057 },
        new ScanValueEntry { PlanetType = "Class I Gas Giant", Terraformable = false, FSS = 3845, FSS_FD = 9997, FSS_DSS = 16021, FSS_FD_DSS = 46233 },
        new ScanValueEntry { PlanetType = "Class II Gas Giant", Terraformable = false, FSS = 28405, FSS_FD = 73853, FSS_DSS = 118354, FSS_FD_DSS = 341536 },
        new ScanValueEntry { PlanetType = "Class III Gas Giant", Terraformable = false, FSS = 995, FSS_FD = 2587, FSS_DSS = 4145, FSS_FD_DSS = 11963 },
        new ScanValueEntry { PlanetType = "Class IV Gas Giant", Terraformable = false, FSS = 1119, FSS_FD = 2910, FSS_DSS = 4663, FSS_FD_DSS = 13457 },
        new ScanValueEntry { PlanetType = "Class V Gas Giant", Terraformable = false, FSS = 966, FSS_FD = 2510, FSS_DSS = 4023, FSS_FD_DSS = 11609 },
        new ScanValueEntry { PlanetType = "Gas Giant with Ammonia-based Life", Terraformable = false, FSS = 774, FSS_FD = 2014, FSS_DSS = 3227, FSS_FD_DSS = 9312 },
        new ScanValueEntry { PlanetType = "Gas Giant with Water-based Life", Terraformable = false, FSS = 883, FSS_FD = 2295, FSS_DSS = 3679, FSS_FD_DSS = 10616 },
        new ScanValueEntry { PlanetType = "Helium-Rich Gas Giant", Terraformable = false, FSS = 900, FSS_FD = 2339, FSS_DSS = 3749, FSS_FD_DSS = 10818 },
        new ScanValueEntry { PlanetType = "Water Giant", Terraformable = false, FSS = 667, FSS_FD = 1734, FSS_DSS = 2779, FSS_FD_DSS = 8019 },
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