using EliteDangerousChecker.Core.Entities.Galaxy;
using EliteDangerousChecker.Core.Entities.Market;
using EliteDangerousChecker.Core.Plugins;

namespace EliteDangerousChecker.Core.Operations;

public interface IGetPricePoints
{
    Task<PricePoint[]> Execute(Commodity commodity, SolarSystem searchFrom);
}

public class GetPricePoints : IGetPricePoints
{
    private readonly IInaraClient inaraClient;

    public GetPricePoints(IInaraClient inaraClient)
    {
        this.inaraClient = inaraClient;
    }

    public async Task<PricePoint[]> Execute(Commodity commodity, SolarSystem searchFrom)
    {
        return await inaraClient.GetSellingPrices(commodity, searchFrom);
    }
}
