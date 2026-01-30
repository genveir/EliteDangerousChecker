delete from SectorPrefix
delete from StationsMappedToPlaceholderFaction
delete from StationEconomies
delete from StationServices
delete from Station
delete from Ring
delete from Body
delete from SolarSystemFaction
delete from SolarSystemPower
delete from SolarSystem
delete from Faction

delete from Allegiance
delete from AtmosphereType
delete from BodySubType
delete from BodyType
delete from CarrierDockingAccess
delete from Economy
delete from FactionState
delete from Government
delete from Luminosity
delete from Material
delete from Mineral
delete from Power
delete from PowerState
delete from ReserveLevel
delete from RingType
delete from SectorPostfix
delete from SectorPrefixWord
delete from SectorSuffix
delete from Security
delete from Service
delete from SpectralClass
delete from StationState
delete from StationType
delete from TerraformingState
delete from VolcanismType

 ALTER DATABASE Elite SET RECOVERY SIMPLE
 DBCC SHRINKFILE (Elite_Log, 1)
 DBCC SHRINKFILE (Elite, 1)