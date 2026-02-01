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
				join Power p on p.Id = ssp.PowerId
            where
                ssp.SolarSystemId = ss.Id
                and p.Name = 'Jerome Archer'
        )
        and exists
        (
            select 1
            from 
				Power p
            where
                p.Id = ss.ControllingPowerId
                and p.Name <> 'Jerome Archer'
        )
        and exists
        (
            select 1
            from 
				Body b
				join Ring r on r.BodyId = b.Id
				join RingType rt on rt.Id = r.RingTypeId
            where
                b.SolarSystemId = ss.Id
                and rt.Name = 'Rocky'
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
		left join StationCommodities sc on sc.StationId = es.StationId and sc.CommodityId = rrc.CommodityId
),
StationAverages as
(
    select
        StationId,
        SolarSystemId,

        avg(scp.EffectiveSellPrice) as AverageSellPrice,

        max(case when scp.CommodityId = 11 then scp.EffectiveSellPrice end) as Alexandrite,
        max(case when scp.CommodityId = 41 then scp.EffectiveSellPrice end) as Benitoite,
        max(case when scp.CommodityId = 212 then scp.EffectiveSellPrice end) as Monazite,
        max(case when scp.CommodityId = 217 then scp.EffectiveSellPrice end) as Musgravite,
        max(case when scp.CommodityId = 287 then scp.EffectiveSellPrice end) as Serendibite,

		max(case when scp.CommodityId = 11 then scp.Demand end) as AlexandriteDemand,
		max(case when scp.CommodityId = 41 then scp.Demand end) as BenitoiteDemand,
		max(case when scp.CommodityId = 212 then scp.Demand end) as MonaziteDemand,
		max(case when scp.CommodityId = 217 then scp.Demand end) as MusgraviteDemand,
		max(case when scp.CommodityId = 287 then scp.Demand end) as SerendibiteDemand
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
)
select top (50)
    ssn.Name as SolarSystemName,
	st.Id as StationId,
    st.Name  as StationName,
	DATEADD(SECOND, MarketUpdateTime, '1970-01-01') UpdateTime,
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
	cr.Result,
	cr.Timestamp,
	b.Name

from 
	RankedStations rs
	join Station st on st.Id = rs.StationId
	cross apply dbo.GetSectorPrefixName(rs.SolarSystemId) ssn
	left join Economy e on e.Id = st.PrimaryEconomyId
	left join CheckResult cr on cr.StationId = st.Id
	left join Body b on b.Id = st.BodyId
where 
	AlexandriteDemand > 20
	and BenitoiteDemand > 20
	and MonaziteDemand > 20
	and MusgraviteDemand > 20
	and SerendibiteDemand > 20
order by 
	rs.AverageSellPrice desc, 
	UpdateTime desc;