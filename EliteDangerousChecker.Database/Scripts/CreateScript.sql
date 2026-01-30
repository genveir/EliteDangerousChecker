begin transaction

create table Allegiance (Id bigint primary key, Name nvarchar(32) not null);
create table AtmosphereType (Id bigint primary key, Name nvarchar(64) not null);
create table BodySubType (Id bigint primary key, Name nvarchar(64) not null);
create table BodyType (Id bigint primary key, Name nvarchar(32) not null);
create table CarrierDockingAccess (Id bigint primary key, Name nvarchar(32) not null);
create table Economy (Id bigint primary key, Name nvarchar(32) not null);
create table FactionState (Id bigint primary key, Name nvarchar(32) not null);
create table Government (Id bigint primary key, Name nvarchar(32) not null);
create table Luminosity (Id bigint primary key, Name nvarchar(32) not null);
create table Material (Id bigint primary key, Name nvarchar(32) not null);
create table Mineral (Id bigint primary key, Name nvarchar(32) not null);
create table Power (Id bigint primary key, Name nvarchar(32) not null);
create table PowerState (Id bigint primary key, Name nvarchar(32) not null);
create table ReserveLevel (Id bigint primary key, Name nvarchar(32) not null);
create table RingType (Id bigint primary key, Name nvarchar(32) not null);
create table SectorPostfix (Id bigint primary key, Name nvarchar(5) not null);
create table SectorPrefixWord (Id bigint primary key, Name nvarchar(32) not null);
create table SectorSuffix (Id bigint primary key, Name nvarchar(4) not null);
create table Security (Id bigint primary key, Name nvarchar(32) not null);
create table Service (Id bigint primary key, Name nvarchar(32) not null);
create table SpectralClass (Id bigint primary key, Name nvarchar(32) not null);
create table StationState (Id bigint primary key, Name nvarchar(32) not null);
create table StationType (Id bigint primary key, Name nvarchar(32) not null);
create table TerraformingState (Id bigint primary key, Name nvarchar(32) not null);
create table VolcanismType (Id bigint primary key, Name nvarchar(32) not null);

create table Faction (
	Id bigint primary key,
	Name nvarchar(64) not null,
	AllegianceId bigint not null,
	GovernmentId bigint not null);

create table SolarSystem (
	Id bigint primary key,
	X int,
	Y int,
	Z int,
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
	SectorSuffixId bigint);

create table SolarSystemFaction (
	SolarSystemId bigint not null,
	FactionId bigint not null,
	Influence float not null,
	FactionStateId bigint not null);

alter table SolarSystemFaction add constraint PK_SolarSystemFaction primary key (SolarSystemId, FactionId);

create table Body (
	Id bigint primary key,
	BodyId int,
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
	SolarSystemId bigint,
	SolarSystemNameIsPrefix bit);

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
	BodyId bigint,
	SolarSystemId bigint);

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
	FactionName nvarchar(64) not null);

create table SectorPrefix (
	SolarSystemId bigint not null,
	Sequence int not null,
	SectorPrefixWordId bigint,
	SectorPrefixNumber int,
	StartWithDash bit,
	StartWithJ bit);

create table Ring (
	Id bigint primary key,
	Name nvarchar(32),
	BodyNameIsPrefix bit,
	BodyId bigint,
	RingTypeId bigint,
	Mass float,
	InnerRadius float,
	OuterRadius float);

alter table SectorPrefix add constraint PK_SectorPrefix primary key (SolarSystemId, Sequence);

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

alter table Body add constraint FK_Body_BodyType foreign key (BodyTypeId) references BodyType (Id);
alter table Body add constraint FK_Body_BodySubType foreign key (BodySubTypeId) references BodySubType (Id);
alter table Body add constraint FK_Body_SpectralClass foreign key (SpectralClassId) references SpectralClass (Id);
alter table Body add constraint FK_Body_Luminosity foreign key (LuminosityId) references Luminosity (Id);
alter table Body add constraint FK_Body_VolcanismType foreign key (VolcanismTypeId) references VolcanismType (Id);
alter table Body add constraint FK_Body_AtmosphereType foreign key (AtmosphereTypeId) references AtmosphereType (Id);
alter table Body add constraint FK_Body_TerraformingState foreign key (TerraformingStateId) references TerraformingState (Id);
alter table Body add constraint FK_Body_ReserveLevel foreign key (ReserveLevelId) references ReserveLevel (Id);
alter table Body add constraint FK_Body_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);

alter table Station add constraint FK_Station_ControllingFaction foreign key (ControllingFactionId) references Faction (Id);
alter table Station add constraint FK_Station_ControllingFactionState foreign key (ControllingFactionState) references FactionState (Id);
alter table Station add constraint FK_Station_PrimaryEconomy foreign key (PrimaryEconomyId) references Economy (Id);
alter table Station add constraint FK_Station_Government foreign key (GovernmentId) references Government (Id);
alter table Station add constraint FK_Station_StationType foreign key (StationTypeId) references StationType (Id);
alter table Station add constraint FK_Station_StateId foreign key (StateId) references StationState (Id);
alter table Station add constraint FK_Station_Allegiance foreign key (AllegianceId) references Allegiance (Id);
alter table Station add constraint FK_Station_Body foreign key (BodyId) references Body (Id);
alter table Station add constraint FK_Station_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);

alter table StationEconomies add constraint FK_StationEconomies_Station foreign key (StationId) references Station (Id);
alter table StationEconomies add constraint FK_StationEconomies_Economy foreign key (EconomyId) references Economy (Id);

alter table StationServices add constraint FK_StationServices_Station foreign key (StationId) references Station (Id);
alter table StationServices add constraint FK_StationServices_Service foreign key (ServiceId) references Service (Id);

alter table StationsMappedToPlaceholderFaction add constraint FK_StationsMappedToPlaceholderFaction_Station foreign key (StationId) references Station (Id);

alter table SectorPrefix add constraint FK_SectorPrefix_SolarSystem foreign key (SolarSystemId) references SolarSystem (Id);
alter table SectorPrefix add constraint FK_SectorPrefix_SectorPrefixWord foreign key (SectorPrefixWordId) references SectorPrefixWord (Id);

alter table Ring add constraint FK_Ring_Body foreign key (BodyId) references Body (Id);
alter table Ring add constraint FK_Ring_RingType foreign key (RingTypeId) references RingType (Id);

commit transaction