-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.7.20-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             9.5.0.5196
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping structure for table gta_rp.allowed_character_creator_models
CREATE TABLE IF NOT EXISTS `allowed_character_creator_models` (
  `model_name` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`model_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.allowed_character_creator_models: ~295 rows (approximately)
/*!40000 ALTER TABLE `allowed_character_creator_models` DISABLE KEYS */;
INSERT INTO `allowed_character_creator_models` (`model_name`) VALUES
	('Abigail'),
	('AfriAmer01AMM'),
	('Agent14'),
	('AlDiNapoli'),
	('AmandaTownley'),
	('Andreas'),
	('AntonB'),
	('ArmBoss01GMM'),
	('ArmGoon01GMM'),
	('ArmGoon02GMY'),
	('ArmLieut01GMM'),
	('Ashley'),
	('AutoShop01SMM'),
	('AviSchwartzman'),
	('Azteca01GMY'),
	('BallaEast01GMY'),
	('BallaOrig01GMY'),
	('Ballas01GFY'),
	('BallasOG'),
	('BallaSout01GMY'),
	('Bankman'),
	('Bankman01'),
	('Barman01SMY'),
	('Barry'),
	('Bartender01SFY'),
	('Baygor'),
	('Beach01AMO'),
	('Beach01AMY'),
	('Beach02AMY'),
	('BeachVesp01AMY'),
	('Beachvesp02AMY'),
	('Benny'),
	('BestMen'),
	('Beverly'),
	('BevHills01AFM'),
	('Bevhills01AFY'),
	('BevHills01AMM'),
	('BevHills01AMY'),
	('BevHills02AFM'),
	('BevHills02AFY'),
	('BevHills02AMM'),
	('BevHills02AMY'),
	('Bevhills03AFY'),
	('BevHills04AFY'),
	('BikerChic'),
	('BoatStaff01F'),
	('BoatStaff01M'),
	('Brad'),
	('Bride'),
	('Busicas01AMY'),
	('Business01AFY'),
	('Business01AMM'),
	('Business01AMY'),
	('Business02AFM'),
	('Business02AFY'),
	('Business02AMY'),
	('Business03AFY'),
	('Business03AMY'),
	('Business04AFY'),
	('Busker01SMO'),
	('Car3Guy1'),
	('Car3Guy2'),
	('ChiBoss01GMM'),
	('ChiGoon01GMM'),
	('ChiGoon02GMM'),
	('Chip'),
	('CntryBar01SMM'),
	('ComJane'),
	('Cyclist01'),
	('Cyclist01amy'),
	('Dale'),
	('DaveNorton'),
	('Dealer01SMY'),
	('Denise'),
	('Devin'),
	('DevinSec01SMY'),
	('DHill01AMY'),
	('DoaMan'),
	('Dom'),
	('DownTown01AFM'),
	('DownTown01AMY'),
	('Dreyfuss'),
	('DrFriedlander'),
	('EastSA01AFM'),
	('EastSA01AFY'),
	('EastSA01AMM'),
	('Eastsa01AMY'),
	('EastSA02AFM'),
	('EastSA02AFY'),
	('EastSa02AMM'),
	('EastSA02AMY'),
	('EastSA03AFY'),
	('EdToh'),
	('Epsilon01AFY'),
	('Epsilon01AMY'),
	('Epsilon02AMY'),
	('Fabien'),
	('FamCA01GMY'),
	('FamDD01'),
	('FamDNF01GMY'),
	('FamFor01GMY'),
	('Families01GFY'),
	('Farmer01AMM'),
	('FatBla01AFM'),
	('FatLatin01AMM'),
	('FatWhite01AFM'),
	('FemBarberSFM'),
	('Fitness01AFY'),
	('Fitness02AFY'),
	('Floyd'),
	('G'),
	('Gay01AMY'),
	('Gay02AMY'),
	('GenFat01AMM'),
	('GenFat02AMM'),
	('GenHot01AFY'),
	('GenStreet01AMY'),
	('GenStreet02AMY'),
	('GlenStank01'),
	('Golfer01AFY'),
	('Groom'),
	('Guido01'),
	('HairDress01SMM'),
	('Hao'),
	('HasJew01AMM'),
	('HasJew01AMY'),
	('Hiker01AFY'),
	('Hiker01AMY'),
	('HillBilly01AMM'),
	('HillBilly02AMM'),
	('Hippie01'),
	('Hippie01AFY'),
	('Hippy01AMY'),
	('Hipster01AFY'),
	('Hipster01AMY'),
	('Hipster02AFY'),
	('Hipster02AMY'),
	('Hipster03AFY'),
	('Hipster03AMY'),
	('Hipster04AFY'),
	('HotPosh01'),
	('Indian01AFO'),
	('Indian01AFY'),
	('Indian01AMM'),
	('Indian01AMY'),
	('Janet'),
	('JayNorris'),
	('JewelAss'),
	('JewelAss01'),
	('JimmyBoston'),
	('JimmyDisanto'),
	('JoeMinuteman'),
	('Josef'),
	('Josh'),
	('Juggalo01AFY'),
	('Juggalo01AMY'),
	('KerryMcintosh'),
	('KorBoss01GMM'),
	('Korean01GMY'),
	('Korean02GMY'),
	('KorLieut01GMY'),
	('KTown01AFM'),
	('KTown01AFO'),
	('KTown01AMM'),
	('KTown01AMO'),
	('KTown01AMY'),
	('KTown02AFM'),
	('KTown02AMY'),
	('LamarDavis'),
	('Latino01AMY'),
	('Lazlow'),
	('LifeInvad01'),
	('LifeInvad01SMM'),
	('LifeInvad02'),
	('Lost01GFY'),
	('Lost01GMY'),
	('Lost02GMY'),
	('Lost03GMY'),
	('Magenta'),
	('Malibu01AMM'),
	('Mani'),
	('Manuel'),
	('Markfost'),
	('Marnie'),
	('Maryann'),
	('MethHead01AMY'),
	('MexBoss01GMM'),
	('MexBoss02GMM'),
	('MexCntry01AMM'),
	('MexGang01GMY'),
	('MexGoon01GMY'),
	('MexGoon02GMY'),
	('MexGoon03GMY'),
	('Mexthug01AMY'),
	('Milton'),
	('Miranda'),
	('Motox02AMY'),
	('MovPrem01SFY'),
	('MovPrem01SMM'),
	('MPros01'),
	('MrK'),
	('Natalia'),
	('NervousRon'),
	('Nigel'),
	('OGBoss01AMM'),
	('OldMan2'),
	('Oneil'),
	('Ortega'),
	('Paige'),
	('Paparazzi01AMM'),
	('Paper'),
	('Party01'),
	('PoloGoon01GMY'),
	('PoloGoon02GMY'),
	('Polynesian01AMM'),
	('Polynesian01AMY'),
	('Popov'),
	('RampGang'),
	('RampHipster'),
	('RivalPaparazzi'),
	('RoadCyc01AMY'),
	('Runner01AFY'),
	('Runner02AMY'),
	('RurMeth01AFY'),
	('RurMeth01AMM'),
	('Salton01AFM'),
	('Salton01AFO'),
	('SalvaGoon02GMY'),
	('SalvaGoon03GMY'),
	('ScreenWriter'),
	('ShopHighSFM'),
	('ShopLowSFY'),
	('ShopMidSFY'),
	('SiemonYetarian'),
	('Skater01AFY'),
	('Skater01AMM'),
	('Skater01AMY'),
	('Skater02AMY'),
	('SoCenLat01AMM'),
	('Solomon'),
	('SouCent01AFO'),
	('SouCent01AFY'),
	('SouCent01AMM'),
	('SouCent01AMO'),
	('SouCent01AMY'),
	('SouCent02AFM'),
	('SouCent02AFO'),
	('SouCent02AFY'),
	('SouCent02AMM'),
	('SouCent02AMO'),
	('SouCent02AMY'),
	('SouCent03AFY'),
	('SouCent03AMM'),
	('SouCent04AMY'),
	('SpyActor'),
	('SpyActress'),
	('StBla01AMY'),
	('SteveHain'),
	('StLat01AMY'),
	('StLat02AMM'),
	('Stretch'),
	('StrPreach01SMM'),
	('StrPunk01GMY'),
	('StrPunk02GMY'),
	('StWhi01AMY'),
	('StWhi02AMY'),
	('SunBathe01AMY'),
	('Talina'),
	('Tanisha'),
	('TaoCheng'),
	('TaosTranslator'),
	('Tattoo01AMO'),
	('Tennis01AFY'),
	('TomEpsilon'),
	('Tonya'),
	('Tourist01AFM'),
	('Tourist01AFY'),
	('Tourist01AMM'),
	('Tourist02AFY'),
	('TracyDisanto'),
	('Vagos01GFY'),
	('VagosSpeak'),
	('VinDouche01AMY'),
	('Vinewood01AFY'),
	('VineWood01AMY'),
	('Vinewood02AFY'),
	('VineWood02AMY'),
	('Vinewood03AFY'),
	('Vinewood03AMY'),
	('Vinewood04AFY'),
	('Vinewood04AMY'),
	('WeiCheng'),
	('WillyFist'),
	('Yoga01AFY'),
	('Zimbor');
/*!40000 ALTER TABLE `allowed_character_creator_models` ENABLE KEYS */;

-- Dumping structure for table gta_rp.buildings
CREATE TABLE IF NOT EXISTS `buildings` (
  `id` int(10) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  `use_blip` int(10) NOT NULL,
  `blip_id` int(100) DEFAULT NULL,
  `blip_x` float DEFAULT NULL,
  `blip_y` float DEFAULT NULL,
  `blip_z` float DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.buildings: ~2 rows (approximately)
/*!40000 ALTER TABLE `buildings` DISABLE KEYS */;
INSERT INTO `buildings` (`id`, `name`, `use_blip`, `blip_id`, `blip_x`, `blip_y`, `blip_z`) VALUES
	(0, 'Eclipse Towers', 1, 475, -773.48, 309.5, 85.6981),
	(1, 'Middle Road', 0, NULL, NULL, NULL, NULL);
/*!40000 ALTER TABLE `buildings` ENABLE KEYS */;

-- Dumping structure for table gta_rp.characters
CREATE TABLE IF NOT EXISTS `characters` (
  `id` int(11) NOT NULL DEFAULT '0',
  `player_id` int(11) DEFAULT NULL,
  `first_name` varchar(100) DEFAULT NULL,
  `last_name` varchar(100) DEFAULT NULL,
  `faction_id` int(11) DEFAULT NULL,
  `player_model` varchar(100) DEFAULT NULL,
  `money` int(11) DEFAULT NULL,
  `job` int(11) DEFAULT NULL,
  `phone_number` varchar(11) DEFAULT NULL,
  `spawn_house_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.characters: ~3 rows (approximately)
/*!40000 ALTER TABLE `characters` DISABLE KEYS */;
INSERT INTO `characters` (`id`, `player_id`, `first_name`, `last_name`, `faction_id`, `player_model`, `money`, `job`, `phone_number`, `spawn_house_id`) VALUES
	(0, 0, 'John', 'Thomson', 1, 'Barry', 260565, 0, '4695300', 4),
	(1, 0, 'Jane', 'Cheng', 0, 'MovPrem01SFY', 1293274, 1, '1325993', -1),
	(2, 0, 'Jane', 'Thomson', 1, 'Bevhills01AFY', 50, 1, '3411699', -1);
/*!40000 ALTER TABLE `characters` ENABLE KEYS */;

-- Dumping structure for table gta_rp.houses
CREATE TABLE IF NOT EXISTS `houses` (
  `id` int(11) NOT NULL DEFAULT '0',
  `owner_id` int(11) DEFAULT NULL,
  `template_id` int(11) DEFAULT NULL,
  `name` varchar(111) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.houses: ~5 rows (approximately)
/*!40000 ALTER TABLE `houses` DISABLE KEYS */;
INSERT INTO `houses` (`id`, `owner_id`, `template_id`, `name`) VALUES
	(1, 1, 1, 'Jane\'s apartment'),
	(2, 1, 0, 'Jane\'s second house'),
	(3, 0, 0, 'John\'s home'),
	(4, 0, 0, 'Apartment 2'),
	(5, 0, 0, 'test1');
/*!40000 ALTER TABLE `houses` ENABLE KEYS */;

-- Dumping structure for table gta_rp.house_exits
CREATE TABLE IF NOT EXISTS `house_exits` (
  `id` int(11) NOT NULL DEFAULT '0',
  `house_template_id` int(11) DEFAULT NULL,
  `exitX` float DEFAULT NULL,
  `exitY` float DEFAULT NULL,
  `exitZ` float DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.house_exits: ~3 rows (approximately)
/*!40000 ALTER TABLE `house_exits` DISABLE KEYS */;
INSERT INTO `house_exits` (`id`, `house_template_id`, `exitX`, `exitY`, `exitZ`) VALUES
	(0, 0, -785.143, 323.671, 211.997),
	(1, 1, -614.86, 40.6783, 97.6001),
	(2, 2, 261.459, -998.82, -99.0086);
/*!40000 ALTER TABLE `house_exits` ENABLE KEYS */;

-- Dumping structure for table gta_rp.house_teleports
CREATE TABLE IF NOT EXISTS `house_teleports` (
  `id` int(11) NOT NULL DEFAULT '0',
  `building_id` int(11) DEFAULT NULL,
  `entranceX` float DEFAULT NULL,
  `entranceY` float DEFAULT NULL,
  `entranceZ` float DEFAULT NULL,
  `name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.house_teleports: ~4 rows (approximately)
/*!40000 ALTER TABLE `house_teleports` DISABLE KEYS */;
INSERT INTO `house_teleports` (`id`, `building_id`, `entranceX`, `entranceY`, `entranceZ`, `name`) VALUES
	(0, 0, -772.716, 311.984, 85.6981, 'Main entrance'),
	(1, 1, 419.016, -1735.89, 29.6077, 'Exit'),
	(2, 1, 285.554, -1011.85, 86.7462, 'Rooftop'),
	(3, 0, -755.102, 334.561, 230.637, 'Rooftop');
/*!40000 ALTER TABLE `house_teleports` ENABLE KEYS */;

-- Dumping structure for table gta_rp.house_teleport_links
CREATE TABLE IF NOT EXISTS `house_teleport_links` (
  `entrance_id` int(100) NOT NULL DEFAULT '0',
  `exit_id` int(100) NOT NULL DEFAULT '0',
  PRIMARY KEY (`entrance_id`,`exit_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.house_teleport_links: ~4 rows (approximately)
/*!40000 ALTER TABLE `house_teleport_links` DISABLE KEYS */;
INSERT INTO `house_teleport_links` (`entrance_id`, `exit_id`) VALUES
	(0, 0),
	(1, 2),
	(2, 2),
	(3, 0);
/*!40000 ALTER TABLE `house_teleport_links` ENABLE KEYS */;

-- Dumping structure for table gta_rp.house_template
CREATE TABLE IF NOT EXISTS `house_template` (
  `id` int(11) NOT NULL DEFAULT '0',
  `building_id` int(11) DEFAULT NULL,
  `ipl` varchar(100) DEFAULT NULL,
  `house_name` varchar(100) DEFAULT NULL,
  `spawn_positionX` float DEFAULT NULL,
  `spawn_positionY` float DEFAULT NULL,
  `spawn_positionZ` float DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.house_template: ~3 rows (approximately)
/*!40000 ALTER TABLE `house_template` DISABLE KEYS */;
INSERT INTO `house_template` (`id`, `building_id`, `ipl`, `house_name`, `spawn_positionX`, `spawn_positionY`, `spawn_positionZ`) VALUES
	(0, 0, 'no_ipl', 'Eclipse Towers 32', -776.343, 335.784, 211.397),
	(1, 0, 'no_ipl', 'Tinsel Towers 12', 0, 0, 0),
	(2, 1, 'no_ipl', 'Apartment 520', 0, 0, 0);
/*!40000 ALTER TABLE `house_template` ENABLE KEYS */;

-- Dumping structure for table gta_rp.items
CREATE TABLE IF NOT EXISTS `items` (
  `owner_id` int(11) NOT NULL,
  `item_id` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  PRIMARY KEY (`owner_id`,`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.items: ~5 rows (approximately)
/*!40000 ALTER TABLE `items` DISABLE KEYS */;
INSERT INTO `items` (`owner_id`, `item_id`, `amount`) VALUES
	(0, 0, 2),
	(0, 1, 1),
	(1, 0, 1),
	(1, 1, 1),
	(2, 1, 2);
/*!40000 ALTER TABLE `items` ENABLE KEYS */;

-- Dumping structure for table gta_rp.item_templates
CREATE TABLE IF NOT EXISTS `item_templates` (
  `id` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `description` varchar(50) NOT NULL,
  `field1` int(11) DEFAULT NULL,
  `field2` int(11) DEFAULT NULL,
  `field3` int(11) DEFAULT NULL,
  `field4` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.item_templates: ~2 rows (approximately)
/*!40000 ALTER TABLE `item_templates` DISABLE KEYS */;
INSERT INTO `item_templates` (`id`, `type`, `name`, `description`, `field1`, `field2`, `field3`, `field4`) VALUES
	(0, 1, 'SMG', 'SMG weapon', 736523883, NULL, NULL, 'asd'),
	(1, 2, 'Fishing rod', 'You can catch fish with this', 1338703913, 57005, NULL, 'amb@world_human_stand_fishing@base;base;0.225,-0.045,-0.1;-23,-33,-120;0.17,-0.07,-0.06;-24,-33,-130');
/*!40000 ALTER TABLE `item_templates` ENABLE KEYS */;

-- Dumping structure for table gta_rp.model_genders
CREATE TABLE IF NOT EXISTS `model_genders` (
  `model_name` varchar(50) NOT NULL DEFAULT '',
  `gender` int(11) DEFAULT NULL,
  PRIMARY KEY (`model_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.model_genders: ~295 rows (approximately)
/*!40000 ALTER TABLE `model_genders` DISABLE KEYS */;
INSERT INTO `model_genders` (`model_name`, `gender`) VALUES
	('Abigail', 1),
	('AfriAmer01AMM', 0),
	('Agent14', 0),
	('AlDiNapoli', 0),
	('AmandaTownley', 1),
	('Andreas', 0),
	('AntonB', 0),
	('ArmBoss01GMM', 0),
	('ArmGoon01GMM', 0),
	('ArmGoon02GMY', 0),
	('ArmLieut01GMM', 0),
	('Ashley', 1),
	('AutoShop01SMM', 0),
	('AviSchwartzman', 0),
	('Azteca01GMY', 0),
	('BallaEast01GMY', 0),
	('BallaOrig01GMY', 0),
	('Ballas01GFY', 1),
	('BallasOG', 0),
	('BallaSout01GMY', 0),
	('Bankman', 0),
	('Bankman01', 0),
	('Barman01SMY', 0),
	('Barry', 0),
	('Bartender01SFY', 1),
	('Baygor', 0),
	('Beach01AMO', 0),
	('Beach01AMY', 0),
	('Beach02AMY', 0),
	('BeachVesp01AMY', 0),
	('Beachvesp02AMY', 0),
	('Benny', 0),
	('BestMen', 0),
	('Beverly', 0),
	('BevHills01AFM', 1),
	('Bevhills01AFY', 1),
	('BevHills01AMM', 0),
	('BevHills01AMY', 0),
	('BevHills02AFM', 1),
	('BevHills02AFY', 1),
	('BevHills02AMM', 0),
	('BevHills02AMY', 0),
	('Bevhills03AFY', 1),
	('BevHills04AFY', 1),
	('BikerChic', 1),
	('BoatStaff01F', 1),
	('BoatStaff01M', 0),
	('Brad', 0),
	('Bride', 1),
	('Busicas01AMY', 0),
	('Business01AFY', 1),
	('Business01AMM', 0),
	('Business01AMY', 0),
	('Business02AFM', 1),
	('Business02AFY', 1),
	('Business02AMY', 0),
	('Business03AFY', 1),
	('Business03AMY', 0),
	('Business04AFY', 1),
	('Busker01SMO', 0),
	('Car3Guy1', 0),
	('Car3Guy2', 0),
	('ChiBoss01GMM', 0),
	('ChiGoon01GMM', 0),
	('ChiGoon02GMM', 0),
	('Chip', 0),
	('CntryBar01SMM', 0),
	('ComJane', 1),
	('Cyclist01', 0),
	('Cyclist01amy', 0),
	('Dale', 0),
	('DaveNorton', 0),
	('Dealer01SMY', 0),
	('Denise', 1),
	('Devin', 0),
	('DevinSec01SMY', 0),
	('DHill01AMY', 0),
	('DoaMan', 0),
	('Dom', 0),
	('DownTown01AFM', 1),
	('DownTown01AMY', 0),
	('Dreyfuss', 0),
	('DrFriedlander', 0),
	('EastSA01AFM', 1),
	('EastSA01AFY', 1),
	('EastSA01AMM', 0),
	('Eastsa01AMY', 0),
	('EastSA02AFM', 1),
	('EastSA02AFY', 1),
	('EastSa02AMM', 0),
	('EastSA02AMY', 0),
	('EastSA03AFY', 1),
	('EdToh', 0),
	('Epsilon01AFY', 1),
	('Epsilon01AMY', 0),
	('Epsilon02AMY', 0),
	('Fabien', 0),
	('FamCA01GMY', 0),
	('FamDD01', 0),
	('FamDNF01GMY', 0),
	('FamFor01GMY', 0),
	('Families01GFY', 1),
	('Farmer01AMM', 0),
	('FatBla01AFM', 1),
	('FatLatin01AMM', 0),
	('FatWhite01AFM', 1),
	('FemBarberSFM', 1),
	('Fitness01AFY', 1),
	('Fitness02AFY', 1),
	('Floyd', 0),
	('G', 0),
	('Gay01AMY', 0),
	('Gay02AMY', 0),
	('GenFat01AMM', 0),
	('GenFat02AMM', 0),
	('GenHot01AFY', 1),
	('GenStreet01AMY', 0),
	('GenStreet02AMY', 0),
	('GlenStank01', 0),
	('Golfer01AFY', 1),
	('Groom', 0),
	('Guido01', 0),
	('HairDress01SMM', 0),
	('Hao', 0),
	('HasJew01AMM', 0),
	('HasJew01AMY', 0),
	('Hiker01AFY', 1),
	('Hiker01AMY', 0),
	('HillBilly01AMM', 0),
	('HillBilly02AMM', 0),
	('Hippie01', 0),
	('Hippie01AFY', 1),
	('Hippy01AMY', 0),
	('Hipster01AFY', 1),
	('Hipster01AMY', 0),
	('Hipster02AFY', 1),
	('Hipster02AMY', 0),
	('Hipster03AFY', 1),
	('Hipster03AMY', 0),
	('Hipster04AFY', 1),
	('HotPosh01', 1),
	('Indian01AFO', 1),
	('Indian01AFY', 1),
	('Indian01AMM', 0),
	('Indian01AMY', 1),
	('Janet', 1),
	('JayNorris', 0),
	('JewelAss', 1),
	('JewelAss01', 1),
	('JimmyBoston', 0),
	('JimmyDisanto', 0),
	('JoeMinuteman', 0),
	('Josef', 0),
	('Josh', 0),
	('Juggalo01AFY', 1),
	('Juggalo01AMY', 0),
	('KerryMcintosh', 1),
	('KorBoss01GMM', 0),
	('Korean01GMY', 0),
	('Korean02GMY', 0),
	('KorLieut01GMY', 0),
	('KTown01AFM', 1),
	('KTown01AFO', 1),
	('KTown01AMM', 0),
	('KTown01AMO', 0),
	('KTown01AMY', 0),
	('KTown02AFM', 1),
	('KTown02AMY', 0),
	('LamarDavis', 0),
	('Latino01AMY', 0),
	('Lazlow', 0),
	('LifeInvad01', 0),
	('LifeInvad01SMM', 0),
	('LifeInvad02', 0),
	('Lost01GFY', 1),
	('Lost01GMY', 0),
	('Lost02GMY', 0),
	('Lost03GMY', 0),
	('Magenta', 1),
	('Malibu01AMM', 0),
	('Mani', 0),
	('Manuel', 0),
	('Markfost', 0),
	('Marnie', 1),
	('Maryann', 1),
	('MethHead01AMY', 0),
	('MexBoss01GMM', 0),
	('MexBoss02GMM', 0),
	('MexCntry01AMM', 0),
	('MexGang01GMY', 0),
	('MexGoon01GMY', 0),
	('MexGoon02GMY', 0),
	('MexGoon03GMY', 0),
	('Mexthug01AMY', 0),
	('Milton', 0),
	('Miranda', 1),
	('Motox02AMY', 0),
	('MovPrem01SFY', 1),
	('MovPrem01SMM', 0),
	('MPros01', 0),
	('MrK', 0),
	('Natalia', 1),
	('NervousRon', 0),
	('Nigel', 0),
	('OGBoss01AMM', 0),
	('OldMan2', 0),
	('Oneil', 0),
	('Ortega', 0),
	('Paige', 1),
	('Paparazzi01AMM', 0),
	('Paper', 0),
	('Party01', 0),
	('PoloGoon01GMY', 0),
	('PoloGoon02GMY', 0),
	('Polynesian01AMM', 0),
	('Polynesian01AMY', 0),
	('Popov', 0),
	('RampGang', 0),
	('RampHipster', 0),
	('RivalPaparazzi', 0),
	('RoadCyc01AMY', 0),
	('Runner01AFY', 1),
	('Runner02AMY', 0),
	('RurMeth01AFY', 1),
	('RurMeth01AMM', 0),
	('Salton01AFM', 1),
	('Salton01AFO', 1),
	('SalvaGoon02GMY', 0),
	('SalvaGoon03GMY', 0),
	('ScreenWriter', 1),
	('ShopHighSFM', 1),
	('ShopLowSFY', 1),
	('ShopMidSFY', 1),
	('SiemonYetarian', 0),
	('Skater01AFY', 1),
	('Skater01AMM', 0),
	('Skater01AMY', 0),
	('Skater02AMY', 0),
	('SoCenLat01AMM', 0),
	('Solomon', 0),
	('SouCent01AFO', 1),
	('SouCent01AFY', 1),
	('SouCent01AMM', 0),
	('SouCent01AMO', 0),
	('SouCent01AMY', 0),
	('SouCent02AFM', 1),
	('SouCent02AFO', 1),
	('SouCent02AFY', 1),
	('SouCent02AMM', 0),
	('SouCent02AMO', 0),
	('SouCent02AMY', 0),
	('SouCent03AFY', 1),
	('SouCent03AMM', 0),
	('SouCent04AMY', 0),
	('SpyActor', 0),
	('SpyActress', 1),
	('StBla01AMY', 0),
	('SteveHain', 0),
	('StLat01AMY', 0),
	('StLat02AMM', 0),
	('Stretch', 0),
	('StrPreach01SMM', 0),
	('StrPunk01GMY', 0),
	('StrPunk02GMY', 0),
	('StWhi01AMY', 0),
	('StWhi02AMY', 0),
	('SunBathe01AMY', 0),
	('Talina', 0),
	('Tanisha', 1),
	('TaoCheng', 0),
	('TaosTranslator', 0),
	('Tattoo01AMO', 0),
	('Tennis01AFY', 1),
	('TomEpsilon', 0),
	('Tonya', 1),
	('Tourist01AFM', 1),
	('Tourist01AFY', 1),
	('Tourist01AMM', 0),
	('Tourist02AFY', 1),
	('TracyDisanto', 1),
	('Vagos01GFY', 1),
	('VagosSpeak', 0),
	('VinDouche01AMY', 0),
	('Vinewood01AFY', 1),
	('VineWood01AMY', 0),
	('Vinewood02AFY', 1),
	('VineWood02AMY', 0),
	('Vinewood03AFY', 1),
	('Vinewood03AMY', 0),
	('Vinewood04AFY', 1),
	('Vinewood04AMY', 0),
	('WeiCheng', 0),
	('WillyFist', 0),
	('Yoga01AFY', 1),
	('Zimbor', 0);
/*!40000 ALTER TABLE `model_genders` ENABLE KEYS */;

-- Dumping structure for table gta_rp.phone_contacts
CREATE TABLE IF NOT EXISTS `phone_contacts` (
  `owner` int(11) NOT NULL DEFAULT '0',
  `name` varchar(11) DEFAULT NULL,
  `number` varchar(11) NOT NULL DEFAULT '',
  PRIMARY KEY (`owner`,`number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.phone_contacts: ~0 rows (approximately)
/*!40000 ALTER TABLE `phone_contacts` DISABLE KEYS */;
INSERT INTO `phone_contacts` (`owner`, `name`, `number`) VALUES
	(2, 'asd', '3411699');
/*!40000 ALTER TABLE `phone_contacts` ENABLE KEYS */;

-- Dumping structure for table gta_rp.players
CREATE TABLE IF NOT EXISTS `players` (
  `id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  `password` varchar(100) DEFAULT NULL,
  `admin_level` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.players: ~0 rows (approximately)
/*!40000 ALTER TABLE `players` DISABLE KEYS */;
INSERT INTO `players` (`id`, `name`, `password`, `admin_level`) VALUES
	(0, 'Test', 'cc03e747a6afbbcbf8be7668acfebee5', 0);
/*!40000 ALTER TABLE `players` ENABLE KEYS */;

-- Dumping structure for table gta_rp.police_ranks
CREATE TABLE IF NOT EXISTS `police_ranks` (
  `character_id` int(100) NOT NULL,
  `rank_id` int(100) NOT NULL,
  `modelHash` varchar(100) NOT NULL,
  PRIMARY KEY (`character_id`,`rank_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.police_ranks: ~0 rows (approximately)
/*!40000 ALTER TABLE `police_ranks` DISABLE KEYS */;
INSERT INTO `police_ranks` (`character_id`, `rank_id`, `modelHash`) VALUES
	(0, 3, 'CIASec01SMM');
/*!40000 ALTER TABLE `police_ranks` ENABLE KEYS */;

-- Dumping structure for table gta_rp.random_house_sell_templates
CREATE TABLE IF NOT EXISTS `random_house_sell_templates` (
  `templateId` int(11) NOT NULL,
  `minPrice` int(11) NOT NULL,
  `maxPrice` int(11) NOT NULL,
  PRIMARY KEY (`templateId`),
  UNIQUE KEY `templateId` (`templateId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.random_house_sell_templates: ~2 rows (approximately)
/*!40000 ALTER TABLE `random_house_sell_templates` DISABLE KEYS */;
INSERT INTO `random_house_sell_templates` (`templateId`, `minPrice`, `maxPrice`) VALUES
	(0, 100000, 250000),
	(1, 150000, 180000),
	(2, 100000, 120000);
/*!40000 ALTER TABLE `random_house_sell_templates` ENABLE KEYS */;

-- Dumping structure for table gta_rp.text_messages
CREATE TABLE IF NOT EXISTS `text_messages` (
  `id` int(11) NOT NULL DEFAULT '0',
  `sender_number` varchar(11) NOT NULL DEFAULT '',
  `receiver_number` varchar(100) NOT NULL DEFAULT '',
  `time` varchar(11) NOT NULL DEFAULT '',
  `message` varchar(1000) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.text_messages: ~2 rows (approximately)
/*!40000 ALTER TABLE `text_messages` DISABLE KEYS */;
INSERT INTO `text_messages` (`id`, `sender_number`, `receiver_number`, `time`, `message`) VALUES
	(0, '1325993', '1325993', '19:53:00', 'test'),
	(1, '3411699', '3411699', '23:34:00', 'asdadasd s'),
	(2, '3411699', '3411699', '01:18:00', 'asda213');
/*!40000 ALTER TABLE `text_messages` ENABLE KEYS */;

-- Dumping structure for table gta_rp.vehicles
CREATE TABLE IF NOT EXISTS `vehicles` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ownedId` int(11) DEFAULT NULL,
  `factionId` int(11) DEFAULT NULL,
  `model` int(11) DEFAULT NULL,
  `park_x` float(10,0) DEFAULT NULL,
  `park_y` float(10,0) DEFAULT NULL,
  `park_z` float(10,0) DEFAULT NULL,
  `park_rot_x` float(10,0) DEFAULT NULL,
  `park_rot_y` float(10,0) DEFAULT NULL,
  `park_rot_z` float(10,0) DEFAULT NULL,
  `license_plate_text` varchar(100) DEFAULT NULL,
  `license_plate_style` int(11) DEFAULT NULL,
  `color1` int(11) DEFAULT NULL,
  `color2` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.vehicles: ~8 rows (approximately)
/*!40000 ALTER TABLE `vehicles` DISABLE KEYS */;
INSERT INTO `vehicles` (`id`, `ownedId`, `factionId`, `model`, `park_x`, `park_y`, `park_z`, `park_rot_x`, `park_rot_y`, `park_rot_z`, `license_plate_text`, `license_plate_style`, `color1`, `color2`) VALUES
	(1, -1, 1, 2046537925, -1126, -865, 13, 0, 0, -140, '', 0, 111, 0),
	(2, -1, 1, 2046537925, -1123, -863, 13, 0, 0, -142, '', 0, 111, 0),
	(3, -1, 1, -1627000575, -1112, -855, 13, 1, 0, -143, '', 0, 111, 0),
	(4, -1, 1, -2007026063, -1043, -858, 5, 0, 0, -122, '', 0, 63, 63),
	(5, -1, 1, -2007026063, -1049, -864, 5, 0, -3, -122, '', 0, 63, 63),
	(6, 0, 0, 719660200, -223, -1168, 23, 1, 0, -92, 'LS6X3USS', 0, 41, 55),
	(7, 1, 0, 719660200, -679, 291, 81, -2, -3, -103, 'LSYXE325', 0, 127, 28),
	(8, 1, 0, -956048545, -223, -1168, 23, 1, 0, -92, 'LSS2HL6K', 0, 135, 89);
/*!40000 ALTER TABLE `vehicles` ENABLE KEYS */;

-- Dumping structure for table gta_rp.vehicle_prices
CREATE TABLE IF NOT EXISTS `vehicle_prices` (
  `model` varchar(100) NOT NULL DEFAULT '',
  `price` int(255) NOT NULL DEFAULT '0',
  PRIMARY KEY (`model`,`price`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Dumping data for table gta_rp.vehicle_prices: ~6 rows (approximately)
/*!40000 ALTER TABLE `vehicle_prices` DISABLE KEYS */;
INSERT INTO `vehicle_prices` (`model`, `price`) VALUES
	('Baller3', 250000),
	('Baller4', 230000),
	('BJXL', 100000),
	('Contender', 500000),
	('Ruston', 830000),
	('Taxi', 400000);
/*!40000 ALTER TABLE `vehicle_prices` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
