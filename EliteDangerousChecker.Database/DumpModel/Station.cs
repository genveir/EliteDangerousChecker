using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteDangerousChecker.Database.DumpModel;

public class Station
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("updateTime")]
    public string? UpdateTime { get; set; }

    [JsonPropertyName("realName")]
    public string? RealName { get; set; }

    [JsonPropertyName("controllingFaction")]
    public string? ControllingFaction { get; set; }

    [JsonPropertyName("controllingFactionState")]
    public string? ControllingFactionState { get; set; }

    [JsonPropertyName("distanceToArrival")]
    public double? DistanceToArrival { get; set; }

    [JsonPropertyName("primaryEconomy")]
    public string? PrimaryEconomy { get; set; }

    [JsonPropertyName("economies")]
    public Dictionary<string, double>? Economies { get; set; }

    [JsonPropertyName("government")]
    public string? Government { get; set; }

    [JsonPropertyName("services")]
    public List<string>? Services { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("landingPads")]
    public LandingPads? LandingPads { get; set; }

    [JsonPropertyName("market")]
    public Market? Market { get; set; }

    [JsonPropertyName("carrierDockingAccess")]
    public string? CarrierDockingAccess { get; set; }

    [JsonPropertyName("carrierName")]
    public string? CarrierName { get; set; }

    [JsonPropertyName("shipyard")]
    public Shipyard? Shipyard { get; set; }

    [JsonPropertyName("outfitting")]
    public Outfitting? Outfitting { get; set; }

    [JsonPropertyName("allegiance")]
    public string? Allegiance { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Unmapped { get; set; }

    public void UnmappedFieldsRecursive(List<string> unmappedFields, string prefix)
    {
        if (Unmapped != null && Unmapped.Any())
        {
            unmappedFields.AddRange(Unmapped.Keys.Select(s => prefix + s));
        }

        LandingPads?.UnmappedFieldsRecursive(unmappedFields, prefix + "landingPads.");
        Market?.UnmappedFieldsRecursive(unmappedFields, prefix + "market.");
        Shipyard?.UnmappedFieldsRecursive(unmappedFields, prefix + "shipyard.");
        Outfitting?.UnmappedFieldsRecursive(unmappedFields, prefix + "outfitting.");
    }
}
