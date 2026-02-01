with EligibleSolarSystems as
(
    select ss.Id as SolarSystemId
    from SolarSystem ss
    where
        exists
        (
            select 1
            from 
				SolarSystemPower ssp
            where
                ssp.SolarSystemId = ss.Id
                and ssp.PowerId = 8 -- Jerome Archer
        ) 
        and ss.ControllingPowerId <> 8 -- Not controlled by Jerome Archer
        and exists
        (
            select 1
            from 
				Body b
				join Ring r on r.BodyId = b.Id
            where
                b.SolarSystemId = ss.Id
                and r.RingTypeId = 2 -- Rocky Rings
        )
),
EligibleStations as
(
    select
        s.Id as StationId,
        s.SolarSystemId
    from 
		Station s
		join EligibleSolarSystems ess on ess.SolarSystemId = s.SolarSystemId
    where
		(s.MediumPads > 0 or s.LargePads > 0)
		and s.PrimaryEconomyId <> 2

),
StationCommodityPrices as
(
    select
        es.StationId,
        es.SolarSystemId,
        rrc.CommodityId,
        coalesce(sc.SellPrice, rrc.AverageSellPrice) as EffectiveSellPrice,
		sc.Demand
    from 
		EligibleStations es
		cross join RockyRingCommodities rrc
		left join StationCommodities sc on sc.StationId = es.StationId and sc.CommodityId = rrc.CommodityId and demand > 20
),
StationAverages as
(
    select
        StationId,
        SolarSystemId,

        avg(scp.EffectiveSellPrice) as AverageSellPrice,

        max(case when scp.CommodityId = 96 then scp.EffectiveSellPrice end) as Alexandrite,
        max(case when scp.CommodityId = 119 then scp.EffectiveSellPrice end) as Benitoite,
        max(case when scp.CommodityId = 254 then scp.EffectiveSellPrice end) as Monazite,
        max(case when scp.CommodityId = 258 then scp.EffectiveSellPrice end) as Musgravite,
        max(case when scp.CommodityId = 308 then scp.EffectiveSellPrice end) as Serendibite,

		max(case when scp.CommodityId = 96 then scp.Demand end) as AlexandriteDemand,
		max(case when scp.CommodityId = 119 then scp.Demand end) as BenitoiteDemand,
		max(case when scp.CommodityId = 254 then scp.Demand end) as MonaziteDemand,
		max(case when scp.CommodityId = 258 then scp.Demand end) as MusgraviteDemand,
		max(case when scp.CommodityId = 308 then scp.Demand end) as SerendibiteDemand
    from 
		StationCommodityPrices scp
    group by
        scp.StationId,
        scp.SolarSystemId
),
RankedStations as
(
    select
        *,
        row_number() over
        (
            partition by SolarSystemId
            order by AverageSellPrice desc
        ) as rn
    from 
		StationAverages
),
Top50Results AS (
    select top (50)
        rs.SolarSystemId,
        st.Id as StationId,
        st.Name as StationName,
        dateadd(second, MarketUpdateTime, '1970-01-01') UpdateTime,
        rs.AverageSellPrice,
        rs.Alexandrite,
        rs.AlexandriteDemand,
        rs.Benitoite,
        rs.BenitoiteDemand,
        rs.Monazite,
        rs.MonaziteDemand,
        rs.Musgravite,
        rs.MusgraviteDemand,
        rs.Serendibite,
        rs.SerendibiteDemand,
        coalesce(
            (select se.Proportion 
             from StationEconomies se 
             join Economy e on e.Id = se.EconomyId 
             where se.StationId = st.Id and e.Name = 'Refinery'), 
            0.0
        ) as RefineryProportion,
        e.Name PrimaryEconomy,
        b.Name as BodyName
    from RankedStations rs
    join Station st on st.Id = rs.StationId
    left join Economy e on e.Id = st.PrimaryEconomyId
    left join Body b on b.Id = st.BodyId
    order by
        rs.AverageSellPrice desc, 
        UpdateTime desc
)
select
    ssn.Name as SolarSystemName,
    t50.StationId,
    t50.StationName,
    t50.UpdateTime,
    t50.AverageSellPrice,
    t50.Alexandrite,
    t50.AlexandriteDemand,
    t50.Benitoite,
    t50.BenitoiteDemand,
    t50.Monazite,
    t50.MonaziteDemand,
    t50.Musgravite,
    t50.MusgraviteDemand,
    t50.Serendibite,
    t50.SerendibiteDemand,
    t50.RefineryProportion,
    t50.PrimaryEconomy,
    t50.BodyName
from Top50Results t50
cross apply dbo.GetSectorPrefixName(t50.SolarSystemId) ssn;