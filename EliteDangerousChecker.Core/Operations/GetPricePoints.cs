using EliteDangerousChecker.Core.Entities.Galaxy;
using EliteDangerousChecker.Core.Entities.Market;

namespace EliteDangerousChecker.Core.Operations;

public interface IGetPricePoints
{
    Task<PricePoint[]> Execute(Commodity commodity, SolarSystem searchFrom);
}

public class GetPricePoints : IGetPricePoints
{

    public GetPricePoints()
    {
    }

    public async Task<PricePoint[]> Execute(Commodity commodity, SolarSystem searchFrom)
    {
        await Task.CompletedTask;

        return [];
    }
}
