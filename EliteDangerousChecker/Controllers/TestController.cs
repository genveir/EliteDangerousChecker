using EliteDangerousChecker.Core.Entities.Galaxy;
using EliteDangerousChecker.Core.Entities.Market;
using EliteDangerousChecker.Core.Operations;
using Microsoft.AspNetCore.Mvc;

namespace EliteDangerousChecker.Api.Controllers;

public class TestController : ControllerBase
{
    private readonly IGetPricePoints operation;

    public TestController(IGetPricePoints operation)
    {
        this.operation = operation;
    }

    [HttpGet("api/test")]
    public async Task<IActionResult> Get()
    {
        var solarSystem = new SolarSystem("Lembava");
        var commodity1 = Commodity.Monazite;
        var commodity2 = Commodity.Alexandrite;

        var pricePoints1 = await operation.Execute(commodity1, solarSystem);
        var pricePoints2 = await operation.Execute(commodity2, solarSystem);

        List<CombinedPriceModel> combinedPrices = [];
        foreach (var p1 in pricePoints1)
        {
            var matchIn2 = pricePoints2.SingleOrDefault(p2 => p2.Station.Equals(p1.Station));

            if (matchIn2 == null) continue;

            combinedPrices.Add(new(p1.Station,
                [
                    new(p1.Commodity.ToString(), p1.Price, p1.HoursSinceLastUpdate),
                    new(matchIn2.Commodity.ToString(), matchIn2.Price, matchIn2.HoursSinceLastUpdate)
                ]));
        }

        return Ok(combinedPrices);
    }

    private record CombinedPriceModel(Station Station, CommodityPrice[] CommodityPrices);

    private record CommodityPrice(string CommodityName, int Price, int AgeInHours);
}
