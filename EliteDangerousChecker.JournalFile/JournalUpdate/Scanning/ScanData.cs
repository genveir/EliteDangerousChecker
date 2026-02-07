namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;

internal class ScanData
{
    internal readonly int bodyId;
    internal readonly string? bodyName;
    internal string? scanType;
    internal double? distanceFromArrivalLS;
    internal bool? wasDiscovered;
    internal bool? wasMapped;
    internal bool? wasFootfalled;
    internal string? terraformState;
    internal string? planetClass;
    internal int biologicalSignatures = 0;

    internal bool wasPrinted = false;

    public ScanData(int bodyId, string? bodyName, int biologicalSignatures)
    {
        this.bodyId = bodyId;
        this.bodyName = bodyName;
        this.biologicalSignatures = biologicalSignatures;
    }

    public ScanData(string? scanType, double? distanceFromArrivalLS, string? bodyName, bool? wasDiscovered, bool? wasMapped, bool? wasFootfalled, string? terraformState, string? planetClass)
    {
        this.scanType = scanType;
        this.distanceFromArrivalLS = distanceFromArrivalLS;
        this.bodyName = bodyName;
        this.wasDiscovered = wasDiscovered;
        this.wasMapped = wasMapped;
        this.terraformState = terraformState;
        this.planetClass = planetClass;
    }

    public void UpdateFromScan(string? scanType, double? distanceFromArrivalLS, bool? wasDiscovered, bool? wasMapped, bool? wasFootfalled, string? terraformState, string? planetClass)
    {
        this.scanType = scanType;
        this.distanceFromArrivalLS = distanceFromArrivalLS;
        this.wasDiscovered = wasDiscovered;
        this.wasMapped = wasMapped;
        this.wasFootfalled = wasFootfalled;
        this.terraformState = terraformState;
        this.planetClass = planetClass;
    }

    public bool IsNotable()
    {
        if (string.Equals(scanType, "AutoScan", StringComparison.OrdinalIgnoreCase) && distanceFromArrivalLS == 0.0d)
            return true;

        if (string.Equals(terraformState, "Terraformable", StringComparison.OrdinalIgnoreCase))
            return true;

        if (IsNotablePlanetClass(planetClass))
            return true;

        if (biologicalSignatures > 0)
            return true;

        return false;
    }

    private static bool IsNotablePlanetClass(string? planetClass)
    {
        if (string.IsNullOrWhiteSpace(planetClass))
            return false;

        string[] notableClasses = ["Earthlike body", "Water world", "Ammonia world"];

        return notableClasses.Any(c => string.Equals(c, planetClass, StringComparison.OrdinalIgnoreCase));
    }
}
