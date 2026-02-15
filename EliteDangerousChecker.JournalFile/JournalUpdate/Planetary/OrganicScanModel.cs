using System.Text.Json.Serialization;

// { "timestamp":"2026-02-15T10:16:07Z", "event":"ScanOrganic", "ScanType":"Analyse", "Genus":"$Codex_Ent_Conchas_Genus_Name;", "Genus_Localised":"Concha", "Species":"$Codex_Ent_Conchas_03_Name;", "Species_Localised":"Concha Labiata", "Variant":"$Codex_Ent_Conchas_03_A_Name;", "Variant_Localised":"Concha Labiata - Teal", "WasLogged":false, "SystemAddress":46634696003931, "Body":60 }
namespace EliteDangerousChecker.JournalFile.JournalUpdate.Planetary;
internal class ScanOrganicModel
{
    [JsonPropertyName("SystemAddress")]
    public long? SystemAddress { get; set; }

    [JsonPropertyName("Body")]
    public int? BodyID { get; set; }

    [JsonPropertyName("ScanType")]
    public string? ScanType { get; set; }

    [JsonPropertyName("Genus")]
    public string? Genus { get; set; }

    [JsonPropertyName("Species_Localised")]
    public string? SpeciesLocalised { get; set; }

    [JsonPropertyName("WasLogged")]
    public bool WasLogged { get; set; }
}
