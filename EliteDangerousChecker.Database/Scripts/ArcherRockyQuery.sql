with powerSystems as 
(select 
	ss.Id SolarSystemId
from 
	SolarSystem ss
	join Power controllingPower on controllingPower.Id = ss.ControllingPowerId
	join SolarSystemPower ssp on ssp.SolarSystemId = ss.Id
	join Power anyPower on anyPower.Id = ssp.PowerId
where
	controllingPower.Name <> 'Jerome Archer'
	and anyPower.Name = 'Jerome Archer'),
withRockyRings as
(select
	distinct ps.SolarSystemId
from
	Body b
	join powerSystems ps on ps.SolarSystemId = b.SolarSystemId
	join Ring r on r.BodyId = b.Id
	join RingType rt on rt.Id = r.RingTypeId
where
	rt.Name = 'Rocky')
select
	sp.SolarSystemId, sp.Sequence, spw.Name, sp.SectorPrefixNumber, sp.StartWithDash, sp.StartWithJ
from
	SectorPrefix sp
	join withRockyRings wrr on wrr.SolarSystemId = sp.SolarSystemId
	left join SectorPrefixWord spw on spw.Id = sp.SectorPrefixWordId
order by sp.SolarSystemId, sp.Sequence