use [master];
GO
EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'Elite'
GO
ALTER DATABASE [Elite] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
GO
DROP DATABASE [Elite]
GO

CREATE DATABASE [Elite]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Elite', FILENAME = N'D:\EliteDb\Elite.mdf' , SIZE = 8192KB , FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Elite_log', FILENAME = N'D:\EliteDb\Elite_log.ldf' , SIZE = 8192KB , FILEGROWTH = 65536KB )
 WITH LEDGER = OFF
GO
ALTER DATABASE [Elite] SET COMPATIBILITY_LEVEL = 160
GO
ALTER DATABASE [Elite] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Elite] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Elite] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Elite] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Elite] SET ARITHABORT OFF 
GO
ALTER DATABASE [Elite] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Elite] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Elite] SET AUTO_CREATE_STATISTICS ON(INCREMENTAL = OFF)
GO
ALTER DATABASE [Elite] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Elite] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Elite] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Elite] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Elite] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Elite] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Elite] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Elite] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Elite] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Elite] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Elite] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Elite] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Elite] SET  READ_WRITE 
GO
ALTER DATABASE [Elite] SET RECOVERY FULL 
GO
ALTER DATABASE [Elite] SET  MULTI_USER 
GO
ALTER DATABASE [Elite] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Elite] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Elite] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Elite]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = Off;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = Primary;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = On;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = Primary;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = Off;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = Primary;
GO
USE [Elite]
GO
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [Elite] MODIFY FILEGROUP [PRIMARY] DEFAULT
GO

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
create table SignalGenus (Id bigint primary key, Name nvarchar(64) not null);
create table SpectralClass (Id bigint primary key, Name nvarchar(32) not null);
create table StationState (Id bigint primary key, Name nvarchar(32) not null);
create table StationType (Id bigint primary key, Name nvarchar(32) not null);
create table TerraformingState (Id bigint primary key, Name nvarchar(32) not null);
create table VolcanismType (Id bigint primary key, Name nvarchar(32) not null);

-- Main Tables

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
	FactionName nvarchar(64) not null);

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
	BodyId bigint,
	RingTypeId bigint,
	Mass float,
	InnerRadius float,
	OuterRadius float);

create table BodySignalType (
	BodyId bigint not null,
	SignalTypeId bigint not null,
	Number int not null);

alter table BodySignalType add constraint PK_BodySignalType primary key (BodyId, SignalTypeId);

create table BodySignalGenus (
	BodyId bigint not null,
	SignalGenusId bigint not null);

alter table BodySignalGenus add constraint PK_BodySignalGenus primary key (BodyId, SignalGenusId);

create table RingSignalType (
	RingId bigint not null,
	SignalTypeId bigint not null,
	Number int not null);

alter table RingSignalType add constraint PK_RingSignalType primary key (RingId, SignalTypeId);

create table RingSignalGenus (
	RingId bigint not null,
	SignalGenusId bigint not null);

alter table RingSignalGenus add constraint PK_RingSignalGenus primary key (RingId, SignalGenusId);

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

alter table Station add constraint FK_Station_ControllingFaction foreign key (ControllingFactionId) references Faction (Id);
alter table Station add constraint FK_Station_ControllingFactionState foreign key (ControllingFactionStateId) references FactionState (Id);
alter table Station add constraint FK_Station_PrimaryEconomy foreign key (PrimaryEconomyId) references Economy (Id);
alter table Station add constraint FK_Station_Government foreign key (GovernmentId) references Government (Id);
alter table Station add constraint FK_Station_StationType foreign key (StationTypeId) references StationType (Id);
alter table Station add constraint FK_Station_StateId foreign key (StateId) references StationState (Id);
alter table Station add constraint FK_Station_Allegiance foreign key (AllegianceId) references Allegiance (Id);
alter table Station add constraint FK_Station_Body foreign key (BodyId) references Body (Id);
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

alter table Ring add constraint FK_Ring_Body foreign key (BodyId) references Body (Id);
alter table Ring add constraint FK_Ring_RingType foreign key (RingTypeId) references RingType (Id);

alter table BodySignalType add constraint FK_BodySignalType_Body foreign key (BodyId) references Body (Id);
alter table BodySignalType add constraint FK_BodySignalType_SignalType foreign key (SignalTypeId) references SignalType (Id);

alter table BodySignalGenus add constraint FK_BodySignalGenus_Body foreign key (BodyId) references Body (Id);
alter table BodySignalGenus add constraint FK_BodySignalGenus_SignalGenus foreign key (SignalGenusId) references SignalGenus (Id);

alter table RingSignalType add constraint FK_RingSignalType_Ring foreign key (RingId) references Ring (Id);
alter table RingSignalType add constraint FK_RingSignalType_SignalType foreign key (SignalTypeId) references SignalType (Id);

alter table RingSignalGenus add constraint FK_RingSignalGenus_Ring foreign key (RingId) references Ring (Id);
alter table RingSignalGenus add constraint FK_RingSignalGenus_SignalGenus foreign key (SignalGenusId) references SignalGenus (Id);

-- Indexes

create index IX_SolarSystem_ControllingPower on SolarSystem (ControllingPowerId)

create index IX_SolarSystemPower_Power on SolarSystemPower (PowerId);

create index IX_SolarSystemPowerConflictProgress_Power on SolarSystemPowerConflictProgress (PowerId);

create index IX_Ring_RingType on Ring (RingTypeId)
create index IX_Ring_Body on Ring (BodyId);

create index IX_Body_SolarSystem on Body (SolarSystemId);

create index IX_StationCommodities_CommodityBuy on StationCommodities (CommodityId, BuyPrice);
create index IX_StationCommodities_CommoditySell on StationCommodities (CommodityId, SellPrice);