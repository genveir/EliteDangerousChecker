create database Elite;
go
use Elite;
go

-- Text Tables

create table Allegiance (Id bigint primary key, Name nvarchar(32) not null);
create table AtmosphereType (Id bigint primary key, Name nvarchar(64) not null);
create table BodySubType (Id bigint primary key, Name nvarchar(64) not null);
create table BodyType (Id bigint primary key, Name nvarchar(32) not null);
create table CarrierDockingAccess (Id bigint primary key, Name nvarchar(32) not null);
create table Commodity (Id bigint primary key, CommodityCategoryId bigint, Name nvarchar(64) not null);
create table CommodityCategory (Id bigint primary key, Name nvarchar(32) not null);
create table Economy (Id bigint primary key, Name nvarchar(32) not null);
create table FactionState (Id bigint primary key, Name nvarchar(32) not null);
create table Government (Id bigint primary key, Name nvarchar(32) not null);
create table Luminosity (Id bigint primary key, Name nvarchar(32) not null);
create table Power (Id bigint primary key, Name nvarchar(32) not null);
create table PowerState (Id bigint primary key, Name nvarchar(32) not null);
create table ReserveLevel (Id bigint primary key, Name nvarchar(32) not null);
create table RingType (Id bigint primary key, Name nvarchar(32) not null);
create table SectorPostfix (Id bigint primary key, Name nvarchar(5) not null);
create table SectorPrefixWord (Id bigint primary key, Name nvarchar(32) not null);
create table SectorSuffix (Id bigint primary key, Name nvarchar(4) not null);
create table Security (Id bigint primary key, Name nvarchar(32) not null);
create table Service (Id bigint primary key, Name nvarchar(32) not null);
create table SignalType (Id bigint primary key, Name nvarchar(32) not null);
create table SignalGenus (Id bigint primary key, Name nvarchar(64) not null, Localized nvarchar(32));
create table Species (Id bigint not null primary key, Name nvarchar(32) not null, Value int, Bonus int)
create table SpectralClass (Id bigint primary key, Name nvarchar(32) not null);
create table StationState (Id bigint primary key, Name nvarchar(32) not null);
create table StationType (Id bigint primary key, Name nvarchar(32) not null);
create table TerraformingState (Id bigint primary key, Name nvarchar(32) not null);
create table VolcanismType (Id bigint primary key, Name nvarchar(32) not null);
create table ExplorationStatus (Id bigint primary key, Name nvarchar(32) not null);

-- Main Tables

create table Faction (
	Id bigint primary key,
	Name nvarchar(64) not null,
	AllegianceId bigint,
	GovernmentId bigint);

create table SolarSystem (
	Id bigint primary key,
	X float,
	Y float,
	Z float,
	AllegianceId bigint,
	GovernmentId bigint,
	PrimaryEconomyId bigint,
	SecondaryEconomyId bigint,
	SecurityId bigint,
	Population bigint,
	BodyCount int,
	ControllingFactionId bigint,
	Date bigint,
	PowerStateTimestamp bigint,
	PowersTimestamp bigint,
	ControllingPowerTimestamp bigint,
	ControllingPowerId bigint,
	PowerStateId bigint,
	PowerStateControlProgress float,
	PowerStateReinforcement float,
	PowerStateUndermining float,
	SectorPostfixId bigint,
	SectorSuffixId bigint,
	SolarSystemRegionId bigint,
	SubSector int);

create table SolarSystemFaction (
	SolarSystemId bigint not null,
	FactionId bigint not null,
	Influence float not null,
	FactionStateId bigint not null);

alter table SolarSystemFaction add constraint PK_SolarSystemFaction primary key (SolarSystemId, FactionId);

create table SolarSystemPower (
	SolarSystemId bigint not null,
	PowerId bigint not null);

alter table SolarSystemPower add constraint PK_SolarSystemPower primary key (SolarSystemId, PowerId);

create table SolarSystemPowerConflictProgress (
	SolarSystemId bigint not null,
	PowerId bigint not null,
	Progress float not null);

alter table SolarSystemPowerConflictProgress add constraint PK_SolarSystemPowerConflictProgress primary key (SolarSystemId, PowerId);

create table Body (
	SolarSystemId bigint not null,
	BodyId int not null,
	Name nvarchar(16),
	BodyTypeId bigint,
	BodySubTypeId bigint,
	DistanceToArrival float,
	Mainstar bit,
	Age int,
	SpectralClassId bigint,
	LuminosityId bigint,
	AbsoluteMagnitude float,
	SolarMasses float,
	SurfaceTemperature float,
	RotationalPeriod float,
	RotationalPeriodTidallyLocked bit,
	AxialTilt float,
	OrbitalPeriod float,
	SemiMajorAxis float,
	OrbitalEccentricity float,
	OrbitalInclination float,
	ArgOfPeriapsis float,
	MeanAnomaly float,
	AscendingNode float,
	IsLandable bit,
	Gravity float,
	EarthMasses float,
	Radius float,
	SurfacePressure float,
	VolcanismTypeId bigint,
	AtmosphereTypeId bigint,
	TerraformingStateId bigint,
	ReserveLevelId bigint,
	UpdateTime bigint,
	DistanceToArrivalTimestamp bigint,
	MeanAnomalyTimestamp bigint,
	AscendingNodeTimestamp bigint,
	SolarSystemNameIsPrefix bit,
	SignalsUpdateTime bigint,
	Carbon float,
	Iron float,
	Nickel float,
	Niobium float,
	Phosphorus float,
	Sulphur float,
	Tellurium float,
	Tungsten float,
	Vanadium float,
	Zinc float,
	Zirconium float,
	Germanium float,
	Manganese float,
	Molybdenum float,
	Selenium float,
	Yttrium float,
	Cadmium float,
	Ruthenium float,
	Arsenic float,
	Antimony float,
	Chromium float,
	Tin float,
	Mercury float,
	Technetium float,
	Polonium float,
	Discovered bigint,
	Mapped bigint,
	Landed bigint);

alter table Body add constraint PK_Body primary key (SolarSystemId, BodyId);

create table Station (
	Id bigint primary key,
	Name nvarchar(128),
	UpdateTime bigint,
	RealName nvarchar(64),
	ControllingFactionId bigint,
	ControllingFactionStateId bigint,
	DistanceToArrival float,
	PrimaryEconomyId bigint,
	GovernmentId bigint,
	StationTypeId bigint,
	StateId bigint,
	LargePads int,
	MediumPads int,
	SmallPads int,
	MarketUpdateTime bigint,
	CarrierDockingAccessId bigint,
	CarrierName nvarchar(64),
	ShipyardUpdateTime bigint,
	OutfittingUpdateTime bigint,
	AllegianceId bigint,
	Latitude float,
	Longitude float,
	BodyId int,
	SolarSystemId bigint);

create table StationCommodities (
	StationId bigint not null,
	CommodityId bigint not null,
	Demand int,
	Supply int,
	BuyPrice int,
	SellPrice int);

alter table StationCommodities add constraint PK_StationCommodities primary key (StationId, CommodityId);

create table StationEconomies (
	StationId bigint not null,
	EconomyId bigint not null,
	Proportion float);

alter table StationEconomies add constraint PK_StationEconomies primary key (StationId, EconomyId);

create table StationServices (
	StationId bigint not null,
	ServiceId bigint not null)

alter table StationServices add constraint PK_StationServices primary key (StationId, ServiceId)

create table StationsMappedToPlaceholderFaction (
	StationId bigint not null,
	FactionName nvarchar(128) not null);

create table SectorPrefix (
	SolarSystemId bigint not null,
	Sequence int not null,
	SectorPrefixWordId bigint,
	SectorPrefixNumber int,
	StartWithDash bit,
	StartWithJ bit);

alter table SectorPrefix add constraint PK_SectorPrefix primary key (SolarSystemId, Sequence);

create table Ring (
	Id bigint primary key,
	Name nvarchar(32),
	BodyNameIsPrefix bit,
	SolarSystemId bigint,
	BodyId int,
	RingTypeId bigint,
	Mass float,
	InnerRadius float,
	OuterRadius float);

create table BodySignalType (
	SolarSystemId bigint not null,
	BodyId int not null,
	SignalTypeId bigint not null,
	Number int not null);

alter table BodySignalType add constraint PK_BodySignalType primary key (SolarSystemId, BodyId, SignalTypeId);

create table BodySignalGenus (
	SolarSystemId bigint not null,
	BodyId int not null,
	SignalGenusId bigint not null,
	SpeciesId bigint not null,
	Scanned bigint not null);

alter table BodySignalGenus add constraint PK_BodySignalGenus primary key (SolarSystemId, BodyId, SignalGenusId);

create table RingSignalType (
	RingId bigint not null,
	SignalTypeId bigint not null,
	Number int not null);

alter table RingSignalType add constraint PK_RingSignalType primary key (RingId, SignalTypeId);

create table RingSignalGenus (
	RingId bigint not null,
	SignalGenusId bigint not null);

alter table RingSignalGenus add constraint PK_RingSignalGenus primary key (RingId, SignalGenusId);

create table SolarSystemRegion
(
	Id bigint not null primary key identity(1,1),
	XSector int not null,
	YSector int not null,
	ZSector int not null);

-- Custom tables

create table RockyRingCommodities (
	CommodityId bigint primary key,
	AverageSellPrice int not null
);

create table CommoditiesSold (
	Timestamp bigint not null,
	StationId bigint not null,
	CommodityId bigint not null,
	Count int not null,
	SellPrice int not null
);

create table TableCounts (
    Timestamp bigint,
    Allegiance int,
    AtmosphereType int,
    BodySubType int,
    BodyType int,
    CarrierDockingAccess int,
    Commodity int,
    CommodityCategory int,
    Economy int,
    FactionState int,
    Government int,
    Luminosity int,
    Power int,
    PowerState int,
    ReserveLevel int,
    RingType int,
    SectorPostfix int,
    SectorPrefixWord int,
    SectorSuffix int,
    Security int,
    Service int,
    SignalType int,
    SignalGenus int,
    SpectralClass int,
    StationState int,
    StationType int,
    TerraformingState int,
    VolcanismType int,
    Faction int,
    SolarSystem int,
    SolarSystemFaction int,
    SolarSystemPower int,
    SolarSystemPowerConflictProgress int,
    Body int,
    Station int,
    StationCommodities int,
    StationEconomies int,
    StationServices int,
    StationsMappedToPlaceholderFaction int,
    SectorPrefix int,
    Ring int,
    BodySignalType int,
    BodySignalGenus int,
    RingSignalType int,
    RingSignalGenus int
);

create table SolarSystemDistances
(
	LowerSolarSystemId bigint not null,
	HigherSolarSystemId bigint not null,
	Distance float not null
);

alter table SolarSystemDistances add constraint PK_SolarSystemDistances primary key (LowerSolarSystemId, HigherSolarSystemId);
alter table SolarSystemDistances add constraint CK_ColarSystemDistances_Order check (LowerSolarSystemId <= HigherSolarSystemId);

-- Foreign Keys

alter table Commodity add constraint FK_Commodity_CommodityCategory foreign key (CommodityCategoryId) references CommodityCategory (Id);

alter table Faction add constraint FK_Faction_Allegiance foreign key (AllegianceId) references Allegiance (Id);
alter table Faction add constraint FK_Faction_Government foreign key (GovernmentId) references Government (Id);

alter table SolarSystem add constraint FK_SolarSystem_Allegiance foreign key (AllegianceId) references Allegiance (Id);
alter table SolarSystem add constraint FK_SolarSystem_Government foreign key (GovernmentId) references Government (Id);
alter table SolarSystem add constraint FK_SolarSystem_PrimaryEconomy foreign key (PrimaryEconomyId) references Economy (Id);
alter table SolarSystem add constraint FK_SolarSystem_SecondaryEconomy foreign key (SecondaryEconomyId) references Economy (Id);
alter table SolarSystem add constraint FK_SolarSystem_SecurityId foreign key (SecurityId) references Security (Id);
alter table SolarSystem add constraint FK_SolarSystem_ControllingFaction foreign key (ControllingFactionId) references Faction (Id);
alter table SolarSystem add constraint FK_SolarSystem_ControllingPower foreign key (ControllingPowerId) references Power (Id);
alter table SolarSystem add constraint FK_SolarSystem_PowerState foreign key (PowerStateId) references PowerState (Id);
alter table SolarSystem add constraint FK_SolarSystem_SectorPostfix foreign key (SectorPostfixId) references SectorPostfix (Id);
alter table SolarSystem add constraint FK_SolarSystem_SectorSuffix foreign key (SectorSuffixId) references SectorSuffix (Id);

alter table SolarSystemFaction add constraint FK_SolarSystemFaction_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);
alter table SolarSystemFaction add constraint FK_SolarSystemFaction_Faction foreign key (FactionId) references Faction (Id);
alter table SolarSystemFaction add constraint FK_SolarSystemFaction_FactionState foreign key (FactionStateId) references FactionState (Id);

alter table SolarSystemPower add constraint FK_SolarSystemPower_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);
alter table SolarSystemPower add constraint FK_SolarSystemPower_Power foreign key (PowerId) references Power (Id);

alter table SolarSystemPowerConflictProgress add constraint FK_SolarSystemPowerConflictProgress_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);
alter table SolarSystemPowerConflictProgress add constraint FK_SolarSystemPowerConflictProgress_Power foreign key (PowerId) references Power (Id);

alter table Body add constraint FK_Body_BodyType foreign key (BodyTypeId) references BodyType (Id);
alter table Body add constraint FK_Body_BodySubType foreign key (BodySubTypeId) references BodySubType (Id);
alter table Body add constraint FK_Body_SpectralClass foreign key (SpectralClassId) references SpectralClass (Id);
alter table Body add constraint FK_Body_Luminosity foreign key (LuminosityId) references Luminosity (Id);
alter table Body add constraint FK_Body_VolcanismType foreign key (VolcanismTypeId) references VolcanismType (Id);
alter table Body add constraint FK_Body_AtmosphereType foreign key (AtmosphereTypeId) references AtmosphereType (Id);
alter table Body add constraint FK_Body_TerraformingState foreign key (TerraformingStateId) references TerraformingState (Id);
alter table Body add constraint FK_Body_ReserveLevel foreign key (ReserveLevelId) references ReserveLevel (Id);
alter table Body add constraint FK_Body_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);
alter table Body add constraint FK_Body_Discovered foreign key (Discovered) references ExplorationStatus (Id);
alter table Body add constraint FK_Body_Mapped foreign key (Mapped) references ExplorationStatus (Id);
alter table Body add constraint FK_Body_Landed foreign key (Landed) references ExplorationStatus (Id);

alter table Station add constraint FK_Station_ControllingFaction foreign key (ControllingFactionId) references Faction (Id);
alter table Station add constraint FK_Station_ControllingFactionState foreign key (ControllingFactionStateId) references FactionState (Id);
alter table Station add constraint FK_Station_PrimaryEconomy foreign key (PrimaryEconomyId) references Economy (Id);
alter table Station add constraint FK_Station_Government foreign key (GovernmentId) references Government (Id);
alter table Station add constraint FK_Station_StationType foreign key (StationTypeId) references StationType (Id);
alter table Station add constraint FK_Station_StateId foreign key (StateId) references StationState (Id);
alter table Station add constraint FK_Station_Allegiance foreign key (AllegianceId) references Allegiance (Id);
alter table Station add constraint FK_Station_Body foreign key (SolarSystemId, BodyId) references Body (SolarSystemId, BodyId);
alter table Station add constraint FK_Station_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);

alter table StationCommodities add constraint FK_StationCommodities_Station foreign key (StationId) references Station (Id);
alter table StationCommodities add constraint FK_StationCommodities_Commodity foreign key (CommodityId) references Commodity (Id);

alter table StationEconomies add constraint FK_StationEconomies_Station foreign key (StationId) references Station (Id);
alter table StationEconomies add constraint FK_StationEconomies_Economy foreign key (EconomyId) references Economy (Id);

alter table StationServices add constraint FK_StationServices_Station foreign key (StationId) references Station (Id);
alter table StationServices add constraint FK_StationServices_Service foreign key (ServiceId) references Service (Id);

alter table StationsMappedToPlaceholderFaction add constraint FK_StationsMappedToPlaceholderFaction_Station foreign key (StationId) references Station (Id);

alter table SectorPrefix add constraint FK_SectorPrefix_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);
alter table SectorPrefix add constraint FK_SectorPrefix_SectorPrefixWord foreign key (SectorPrefixWordId) references SectorPrefixWord (Id);

alter table Ring add constraint FK_Ring_Body foreign key (SolarSystemId, BodyId) references Body (SolarSystemId, BodyId);
alter table Ring add constraint FK_Ring_RingType foreign key (RingTypeId) references RingType (Id);

alter table BodySignalType add constraint FK_BodySignalType_Body foreign key (SolarSystemId, BodyId) references Body (SolarSystemId, BodyId);
alter table BodySignalType add constraint FK_BodySignalType_SignalType foreign key (SignalTypeId) references SignalType (Id);

alter table BodySignalGenus add constraint FK_BodySignalGenus_Body foreign key (SolarSystemId, BodyId) references Body (SolarSystemId, BodyId);
alter table BodySignalGenus add constraint FK_BodySignalGenus_SignalGenus foreign key (SignalGenusId) references SignalGenus (Id);
alter table BodySignalGenus add constraint FK_BodySignalGenus_Species foreign key (SpeciesId) references Species (Id);

alter table RingSignalType add constraint FK_RingSignalType_Ring foreign key (RingId) references Ring (Id);
alter table RingSignalType add constraint FK_RingSignalType_SignalType foreign key (SignalTypeId) references SignalType (Id);

alter table RingSignalGenus add constraint FK_RingSignalGenus_Ring foreign key (RingId) references Ring (Id);
alter table RingSignalGenus add constraint FK_RingSignalGenus_SignalGenus foreign key (SignalGenusId) references SignalGenus (Id);
alter table BodySignalGenus add constraint FK_BodySignalGenus_Scanned foreign key (Scanned) references ExplorationStatus (Id)

alter table SolarSystemDistances add constraint FK_SolarSystemDistances_Lower foreign key (LowerSolarSystemId) references SolarSystem (Id)
alter table SolarSystemDistances add constraint FK_SolarSystemDistances_Higher foreign key (HigherSolarSystemId) references SolarSystem (Id)



-- Indexes

create index IX_SolarSystem_ControllingPower on SolarSystem (ControllingPowerId)
create index IX_SolarSystemPower_Power on SolarSystemPower (PowerId);
create index IX_SolarSystem_Region on SolarSystem (SolarSystemRegionId, SubSector) include (X, Y, Z)

create index IX_SolarSystemPowerConflictProgress_Power on SolarSystemPowerConflictProgress (PowerId);

create index IX_SolarSystemRegion on SolarSystemRegion (XSector, YSector, ZSector);

create index IX_Ring_RingType on Ring (RingTypeId)
create index IX_Ring_Body on Ring (SolarSystemId, BodyId);
create index IX_Ring_RingType_Body on Ring (RingTypeId, SolarSystemId, BodyId) include (Id) where RingTypeId = 2;

create index IX_Body_SolarSystem on Body (SolarSystemId);

create index IX_Station_SolarSystem on Station (SolarSystemId);

create index IX_StationCommodities_Demand_Commodity on StationCommodities (Demand, CommodityId, StationId) include (SellPrice) where Demand > 20;
create index IX_StationCommodities_Station_Rocky on StationCommodities (StationId, Demand) include (CommodityId, SellPrice) where Demand > 20 and CommodityId in (96, 119, 254, 258, 308);

create index IX_Station_Eligibility on Station (PrimaryEconomyId, SolarSystemId) include (Id, MediumPads, LargePads);

create index IX_SectorPrefix_Numbers on SectorPrefix (Sequence, SectorPrefixNumber) include (StartWithDash, StartWithJ);
create index IX_SectorPrefix_Word on SectorPrefix (Sequence, SectorPrefixWordId) include (StartWithDash, StartWithJ);

-- Name function

go
create or alter function dbo.GetSectorPrefixName
(
    @SolarSystemId bigint
)
returns table
as
return
(
    select
        Prefix
        + coalesce(' ' + ssuf.Name, '')
        + coalesce(' ' + spost.Name, '') as Name
    from (
        select
            string_agg(
                case
                    when sp.Sequence = 0 then
                        case when sp.StartWithJ = 1 then 'J' else '' end +
                        coalesce(w.Name, convert(nvarchar(32), sp.SectorPrefixNumber))
                    else
                        case when sp.StartWithDash = 1 then '-' else ' ' end +
                        case when sp.StartWithJ = 1 then 'J' else '' end +
                        coalesce(w.Name, convert(nvarchar(32), sp.SectorPrefixNumber))
                end,
                ''
            ) within group (order by sp.Sequence) as Prefix,
            sp.SolarSystemId
        from SectorPrefix sp
        left join SectorPrefixWord w
            on w.Id = sp.SectorPrefixWordId
        where sp.SolarSystemId = @SolarSystemId
        group by sp.SolarSystemId
    ) agg
    inner join SolarSystem s on s.Id = agg.SolarSystemId
    left join SectorSuffix ssuf on ssuf.Id = s.SectorSuffixId
    left join SectorPostfix spost on spost.Id = s.SectorPostfixId
);
go

-- Merits view

create view BestMerits as
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
				Ring r
            where
                r.SolarSystemId = ss.Id
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
        and (
            select count(rrc.CommodityId)
            from RockyRingCommodities rrc
            join StationCommodities sc on sc.CommodityId = rrc.CommodityId
            where sc.StationId = s.Id and sc.Demand > 20
        ) = 5

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
		join StationCommodities sc on sc.StationId = es.StationId
        join RockyRingCommodities rrc on rrc.CommodityId = sc.CommodityId
    where
        sc.Demand > 20
), 
MineralProportions as
(
    select
        cs.Commodityid,
        cast(sum(cs.Count) as float) / nullif(sum(sum(cs.Count)) over (), 0) as Proportion
    from 
		commoditiessold cs
		join rockyringcommodities rrc on rrc.commodityid = cs.commodityid
    group by 
		cs.commodityid
),
StationAverages as
(
    select
        StationId,
        SolarSystemId,

        sum(scp.EffectiveSellPrice * mp.Proportion) as WeightedAverageSellPrice,
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
		join MineralProportions mp on mp.CommodityId = scp.CommodityId
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
            order by WeightedAverageSellPrice desc
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
        rs.WeightedAverageSellPrice,
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
    left join Body b on b.SolarSystemId = st.SolarSystemId and b.BodyId = st.BodyId
    order by
        rs.WeightedAverageSellPrice desc, 
        UpdateTime desc
)
select
    ssn.Name as SolarSystemName,
    t50.StationId,
    t50.StationName,
    t50.UpdateTime,
	t50.AverageSellPrice,
    t50.WeightedAverageSellPrice,
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
go

-- Commodity proportions view

create view CommodityProportions as
with CommodityAmounts as
(
	select
        cs.Commodityid,
		sum(cs.Count) Count,
        cast(sum(cs.Count) as float) / nullif(sum(sum(cs.Count)) over (), 0) as Proportion
    from 
		CommoditiesSold cs
		join RockyRingCommodities rrc on rrc.Commodityid = cs.Commodityid
    group by 
		cs.commodityid
)
select
	c.Name,
	ca.Count,
	ca.Proportion
from
	CommodityAmounts ca
	join Commodity c on c.Id = ca.CommodityId

-- Row counts view

go
drop proc if exists InsertTableCounts;
go

create proc InsertTableCounts as
begin
    insert into TableCounts (
        Timestamp,
        Allegiance,
        AtmosphereType,
        BodySubType,
        BodyType,
        CarrierDockingAccess,
        Commodity,
        CommodityCategory,
        Economy,
        FactionState,
        Government,
        Luminosity,
        Power,
        PowerState,
        ReserveLevel,
        RingType,
        SectorPostfix,
        SectorPrefixWord,
        SectorSuffix,
        Security,
        Service,
        SignalType,
        SignalGenus,
        SpectralClass,
        StationState,
        StationType,
        TerraformingState,
        VolcanismType,
        Faction,
        SolarSystem,
        SolarSystemFaction,
        SolarSystemPower,
        SolarSystemPowerConflictProgress,
        Body,
        Station,
        StationCommodities,
        StationEconomies,
        StationServices,
        StationsMappedToPlaceholderFaction,
        SectorPrefix,
        Ring,
        BodySignalType,
        BodySignalGenus,
        RingSignalType,
        RingSignalGenus
    )
    values (
        datediff(second, '1970-01-01', sysutcdatetime()),
        (select count(*) from Allegiance),
        (select count(*) from AtmosphereType),
        (select count(*) from BodySubType),
        (select count(*) from BodyType),
        (select count(*) from CarrierDockingAccess),
        (select count(*) from Commodity),
        (select count(*) from CommodityCategory),
        (select count(*) from Economy),
        (select count(*) from FactionState),
        (select count(*) from Government),
        (select count(*) from Luminosity),
        (select count(*) from Power),
        (select count(*) from PowerState),
        (select count(*) from ReserveLevel),
        (select count(*) from RingType),
        (select count(*) from SectorPostfix),
        (select count(*) from SectorPrefixWord),
        (select count(*) from SectorSuffix),
        (select count(*) from Security),
        (select count(*) from Service),
        (select count(*) from SignalType),
        (select count(*) from SignalGenus),
        (select count(*) from SpectralClass),
        (select count(*) from StationState),
        (select count(*) from StationType),
        (select count(*) from TerraformingState),
        (select count(*) from VolcanismType),
        (select count(*) from Faction),
        (select count(*) from SolarSystem),
        (select count(*) from SolarSystemFaction),
        (select count(*) from SolarSystemPower),
        (select count(*) from SolarSystemPowerConflictProgress),
        (select count(*) from Body),
        (select count(*) from Station),
        (select count(*) from StationCommodities),
        (select count(*) from StationEconomies),
        (select count(*) from StationServices),
        (select count(*) from StationsMappedToPlaceholderFaction),
        (select count(*) from SectorPrefix),
        (select count(*) from Ring),
        (select count(*) from BodySignalType),
        (select count(*) from BodySignalGenus),
        (select count(*) from RingSignalType),
        (select count(*) from RingSignalGenus)
    );
end
go

-- Update tables
go

create schema upd;
go

drop proc if exists RecreateUpdateTables;
go

create proc RecreateUpdateTables as
begin

drop table if exists upd.RingSignalGenus;
drop table if exists upd.RingSignalType;
drop table if exists upd.BodySignalGenus;
drop table if exists upd.BodySignalType;
drop table if exists upd.Ring;
drop table if exists upd.SectorPrefix;
drop table if exists upd.StationsMappedToPlaceholderFaction;
drop table if exists upd.StationServices;
drop table if exists upd.StationEconomies;
drop table if exists upd.StationCommodities;
drop table if exists upd.Station;
drop table if exists upd.Body;
drop table if exists upd.SolarSystemPowerConflictProgress;
drop table if exists upd.SolarSystemPower;
drop table if exists upd.SolarSystemFaction;
drop table if exists upd.SolarSystem;
drop table if exists upd.Faction;
drop table if exists upd.RingSignalType;
drop table if exists upd.RingSignalGenus;

create table upd.Faction (
	Id bigint primary key,
	Name nvarchar(64) not null,
	AllegianceId bigint,
	GovernmentId bigint);

create table upd.SolarSystem (
	Id bigint primary key,
	X float,
	Y float,
	Z float,
	AllegianceId bigint,
	GovernmentId bigint,
	PrimaryEconomyId bigint,
	SecondaryEconomyId bigint,
	SecurityId bigint,
	Population bigint,
	BodyCount int,
	ControllingFactionId bigint,
	Date bigint,
	PowerStateTimestamp bigint,
	PowersTimestamp bigint,
	ControllingPowerTimestamp bigint,
	ControllingPowerId bigint,
	PowerStateId bigint,
	PowerStateControlProgress float,
	PowerStateReinforcement float,
	PowerStateUndermining float,
	SectorPostfixId bigint,
	SectorSuffixId bigint,
	SolarSystemRegionId bigint,
	SubSector int);

create table upd.SolarSystemFaction (
	SolarSystemId bigint not null,
	FactionId bigint not null,
	Influence float not null,
	FactionStateId bigint not null);

alter table upd.SolarSystemFaction add constraint PK_SolarSystemFaction primary key (SolarSystemId, FactionId);

create table upd.SolarSystemPower (
	SolarSystemId bigint not null,
	PowerId bigint not null);

alter table upd.SolarSystemPower add constraint PK_SolarSystemPower primary key (SolarSystemId, PowerId);

create table upd.SolarSystemPowerConflictProgress (
	SolarSystemId bigint not null,
	PowerId bigint not null,
	Progress float not null);

alter table upd.SolarSystemPowerConflictProgress add constraint PK_SolarSystemPowerConflictProgress primary key (SolarSystemId, PowerId);

create table upd.Body (
	SolarSystemId bigint not null,
	BodyId int not null,
	Name nvarchar(16),
	BodyTypeId bigint,
	BodySubTypeId bigint,
	DistanceToArrival float,
	Mainstar bit,
	Age int,
	SpectralClassId bigint,
	LuminosityId bigint,
	AbsoluteMagnitude float,
	SolarMasses float,
	SurfaceTemperature float,
	RotationalPeriod float,
	RotationalPeriodTidallyLocked bit,
	AxialTilt float,
	OrbitalPeriod float,
	SemiMajorAxis float,
	OrbitalEccentricity float,
	OrbitalInclination float,
	ArgOfPeriapsis float,
	MeanAnomaly float,
	AscendingNode float,
	IsLandable bit,
	Gravity float,
	EarthMasses float,
	Radius float,
	SurfacePressure float,
	VolcanismTypeId bigint,
	AtmosphereTypeId bigint,
	TerraformingStateId bigint,
	ReserveLevelId bigint,
	UpdateTime bigint,
	DistanceToArrivalTimestamp bigint,
	MeanAnomalyTimestamp bigint,
	AscendingNodeTimestamp bigint,
	SolarSystemNameIsPrefix bit,
	SignalsUpdateTime bigint,
	Carbon float,
	Iron float,
	Nickel float,
	Niobium float,
	Phosphorus float,
	Sulphur float,
	Tellurium float,
	Tungsten float,
	Vanadium float,
	Zinc float,
	Zirconium float,
	Germanium float,
	Manganese float,
	Molybdenum float,
	Selenium float,
	Yttrium float,
	Cadmium float,
	Ruthenium float,
	Arsenic float,
	Antimony float,
	Chromium float,
	Tin float,
	Mercury float,
	Technetium float,
	Polonium float);

alter table upd.Body add constraint PK_Body primary key (SolarSystemId, BodyId);

create table upd.Station (
	Id bigint primary key,
	Name nvarchar(128),
	UpdateTime bigint,
	RealName nvarchar(64),
	ControllingFactionId bigint,
	ControllingFactionStateId bigint,
	DistanceToArrival float,
	PrimaryEconomyId bigint,
	GovernmentId bigint,
	StationTypeId bigint,
	StateId bigint,
	LargePads int,
	MediumPads int,
	SmallPads int,
	MarketUpdateTime bigint,
	CarrierDockingAccessId bigint,
	CarrierName nvarchar(64),
	ShipyardUpdateTime bigint,
	OutfittingUpdateTime bigint,
	AllegianceId bigint,
	Latitude float,
	Longitude float,
	BodyId int,
	SolarSystemId bigint);

create table upd.StationCommodities (
	StationId bigint not null,
	CommodityId bigint not null,
	Demand int,
	Supply int,
	BuyPrice int,
	SellPrice int);

alter table upd.StationCommodities add constraint PK_StationCommodities primary key (StationId, CommodityId);

create table upd.StationEconomies (
	StationId bigint not null,
	EconomyId bigint not null,
	Proportion float);

alter table upd.StationEconomies add constraint PK_StationEconomies primary key (StationId, EconomyId);

create table upd.StationServices (
	StationId bigint not null,
	ServiceId bigint not null)

alter table upd.StationServices add constraint PK_StationServices primary key (StationId, ServiceId)

create table upd.StationsMappedToPlaceholderFaction (
	StationId bigint not null,
	FactionName nvarchar(128) not null);

create table upd.SectorPrefix (
	SolarSystemId bigint not null,
	Sequence int not null,
	SectorPrefixWordId bigint,
	SectorPrefixNumber int,
	StartWithDash bit,
	StartWithJ bit);

alter table upd.SectorPrefix add constraint PK_SectorPrefix primary key (SolarSystemId, Sequence);

create table upd.Ring (
    Id bigint primary key,
    Name nvarchar(32),
    BodyNameIsPrefix bit,
    SolarSystemId bigint,
    BodyId int,
    RingTypeId bigint,
    Mass float,
    InnerRadius float,
    OuterRadius float
);

create table upd.BodySignalType (
    SolarSystemId bigint not null,
    BodyId int not null,
    SignalTypeId bigint not null,
    Number int not null
);

alter table upd.BodySignalType add constraint PK_BodySignalType primary key (SolarSystemId, BodyId, SignalTypeId);

create table upd.BodySignalGenus (
    SolarSystemId bigint not null,
    BodyId int not null,
    SignalGenusId bigint not null
);

alter table upd.BodySignalGenus add constraint PK_BodySignalGenus primary key (SolarSystemId, BodyId, SignalGenusId);

create table upd.RingSignalType (
	RingId bigint not null,
	SignalTypeId bigint not null,
	Number int not null);

alter table upd.RingSignalType add constraint PK_RingSignalType primary key (RingId, SignalTypeId);

create table upd.RingSignalGenus (
	RingId bigint not null,
	SignalGenusId bigint not null);

alter table upd.RingSignalGenus add constraint PK_RingSignalGenus primary key (RingId, SignalGenusId);

end