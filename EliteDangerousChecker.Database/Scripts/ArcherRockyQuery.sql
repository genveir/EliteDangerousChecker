with EligibleSolarSystems as
(
    select ss.Id as SolarSystemId
    from SolarSystem ss
    where
        exists
        (
            select 1
            from SolarSystemPower ssp
            join Power p on p.Id = ssp.PowerId
            where
                ssp.SolarSystemId = ss.Id
                and p.Name = 'Jerome Archer'
        )
        and exists
        (
            select 1
            from Power p
            where
                p.Id = ss.ControllingPowerId
                and p.Name <> 'Jerome Archer'
        )
        and exists
        (
            select 1
            from Body b
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
    from Station s
    join EligibleSolarSystems ess
        on ess.SolarSystemId = s.SolarSystemId
),
StationCommodityPrices as
(
    select
        es.StationId,
        es.SolarSystemId,
        rrc.CommodityId,
        coalesce(sc.SellPrice, rrc.AverageSellPrice) as EffectiveSellPrice
    from EligibleStations es
    cross join RockyRingCommodities rrc
    left join StationCommodities sc
        on sc.StationId = es.StationId
       and sc.CommodityId = rrc.CommodityId
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
        max(case when scp.CommodityId = 287 then scp.EffectiveSellPrice end) as Serendibite

    from StationCommodityPrices scp
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
    from StationAverages
)
select top (50)
    ssn.Name as SolarSystemName,
    st.Name  as StationName,
    rs.AverageSellPrice,
    rs.Alexandrite,
    rs.Benitoite,
    rs.Monazite,
    rs.Musgravite,
    rs.Serendibite
from RankedStations rs
join Station st
    on st.Id = rs.StationId
cross apply dbo.GetSectorPrefixName(rs.SolarSystemId) ssn
where rs.rn = 1
order by rs.AverageSellPrice desc;