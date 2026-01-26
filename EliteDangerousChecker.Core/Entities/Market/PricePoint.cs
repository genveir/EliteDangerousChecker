using EliteDangerousChecker.Core.Entities.Galaxy;

namespace EliteDangerousChecker.Core.Entities.Market;
public record PricePoint(
    Commodity Commodity,
    Station Station,
    int Price,
    int HoursSinceLastUpdate);
