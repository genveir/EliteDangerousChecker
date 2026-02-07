namespace EliteDangerousChecker.JournalFile.JournalUpdate.Scanning;
internal static class ScanFormatter
{
    public static string GetHeader()
    {
        return $"{"Body Name",-30}{"Terraform",-15}{"PC",-10}{"Bio",-4}{"Disc",-5} {"Map",-5} {"Foot",-5}";

    }

    public static string Format(ScanData scanData)
    {
        string bodyName = $"{scanData.bodyName ?? "Unknown body"}";
        string terraform = string.Equals(scanData.terraformState, "Terraformable", StringComparison.OrdinalIgnoreCase) ? "TERRAFORMABLE " : "";
        string planetClass = FormatPlanetClass(scanData.planetClass ?? "Unknown");
        string bio = scanData.biologicalSignatures > 0 ? scanData.biologicalSignatures.ToString() : "";
        string discovered = scanData.wasDiscovered == true ? "Yes" : "No";
        string mapped = scanData.wasMapped == true ? "Yes" : "No";
        string footfalled = scanData.wasFootfalled == true ? "Yes" : "No";

        return $"{bodyName,-30}{terraform,-15}{planetClass,-10}{bio,-4}{discovered,-5} {mapped,-5} {footfalled,-5}";
    }

    private static string FormatPlanetClass(string planetClass)
    {
        return planetClass switch
        {
            "Earthlike body" => "Earth-like",
            "Water world" => "Water",
            "Ammonia world" => "Ammonia",
            "High metal content body" => "HMCB",
            "Icy body" => "Icy",
            "Rocky body" => "Rocky",
            "Rocky ice body" => "Rocky Icy",
            "Metal-rich body" => "Metal-rich",
            _ => planetClass
        };
    }
}
