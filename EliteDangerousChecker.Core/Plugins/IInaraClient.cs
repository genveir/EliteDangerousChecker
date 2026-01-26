using EliteDangerousChecker.Core.Entities.Galaxy;
using EliteDangerousChecker.Core.Entities.Market;

namespace EliteDangerousChecker.Core.Plugins;
public interface IInaraClient
{
    Task<PricePoint[]> GetSellingPrices(Commodity commodity, SolarSystem searchFrom);
}
