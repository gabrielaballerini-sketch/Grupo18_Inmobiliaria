-- MySQL dump 10.13  Distrib 26.7.0, for macos15 (arm64)
--
-- Host: 127.0.0.1    Database: inmobiliaria_g18
-- ------------------------------------------------------
-- Server version 5.5.5-10.4.28-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--

DROP TABLE IF EXISTS `tipoinmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipoinmueble` (
  `IdTipoInmueble` int(11) NOT NULL AUTO_INCREMENT,
  `Descripcion` varchar(150) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdTipoInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `propietarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietarios` (
  `IdPropietario` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Telefono` varchar(50) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdPropietario`),
  UNIQUE KEY `Dni` (`Dni`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `inquilinos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilinos` (
  `IdInquilino` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Telefono` varchar(50) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdInquilino`),
  UNIQUE KEY `Dni` (`Dni`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `IdUsuario` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `RolUsuario` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `Avatar` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`IdUsuario`),
  UNIQUE KEY `UserName` (`UserName`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

DROP TABLE IF EXISTS `imagenes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `imagenes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `inmueble_id` int(11) NOT NULL,
  `url` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `inmueble_id` (`inmueble_id`),
  CONSTRAINT `imagenes_ibfk_1` FOREIGN KEY (`inmueble_id`) REFERENCES `inmuebles` (`IdInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `inmuebles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmuebles` (
  `IdInmueble` int(11) NOT NULL AUTO_INCREMENT,
  `Direccion` varchar(200) NOT NULL,
  `Capacidad` int(11) NOT NULL,
  `Latitud` decimal(10,8) DEFAULT NULL,
  `Longitud` decimal(11,8) DEFAULT NULL,
  `PrecioAlquiler` decimal(12,2) NOT NULL,
  `IdPropietario` int(11) NOT NULL,
  `IdTipoInmueble` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `PorcentajeReserva` decimal(10,0) NOT NULL,
  `ImagenUrl` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`IdInmueble`),
  KEY `FK_Inmuebles_Propietarios` (`IdPropietario`),
  KEY `FK_Inmuebles_TipoInmueble` (`IdTipoInmueble`),
  CONSTRAINT `FK_Inmuebles_Propietarios` FOREIGN KEY (`IdPropietario`) REFERENCES `propietarios` (`IdPropietario`),
  CONSTRAINT `FK_Inmuebles_TipoInmueble` FOREIGN KEY (`IdTipoInmueble`) REFERENCES `tipoinmueble` (`IdTipoInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;




DROP TABLE IF EXISTS `reservas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservas` (
  `IdReserva` int(11) NOT NULL AUTO_INCREMENT,
  `IdInquilino` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `MontoDiario` decimal(12,2) NOT NULL,
  `FechaInicio` datetime NOT NULL,
  `FechaFin` datetime NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `Multa` decimal(10,0) DEFAULT NULL,
  `FechaCancelacion` datetime DEFAULT NULL,
  `IdUsuarioCancelacion` int(11) DEFAULT NULL,
  PRIMARY KEY (`IdReserva`),
  KEY `FK_Reservas_Inquilinos` (`IdInquilino`),
  KEY `FK_Reservas_Inmuebles` (`IdInmueble`),
  KEY `FK_Reservas_Usuarios` (`IdUsuario`),
  KEY `FK_Reservas_Usuarios_Cancelacion` (`IdUsuarioCancelacion`),
  CONSTRAINT `FK_Reservas_Inmuebles` FOREIGN KEY (`IdInmueble`) REFERENCES `inmuebles` (`IdInmueble`),
  CONSTRAINT `FK_Reservas_Inquilinos` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilinos` (`IdInquilino`),
  CONSTRAINT `FK_Reservas_Usuarios` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`IdUsuario`),
  CONSTRAINT `FK_Reservas_Usuarios_Cancelacion` FOREIGN KEY (`IdUsuarioCancelacion`) REFERENCES `usuarios` (`IdUsuario`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `pagos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pagos` (
  `IdPago` int(11) NOT NULL AUTO_INCREMENT,
  `IdReserva` int(11) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  `ConceptoPago` int(11) NOT NULL,
  `MedioPago` int(11) NOT NULL,
  `Importe` decimal(10,0) DEFAULT NULL,
  `IdUsuarioCreador` int(11) NOT NULL,
  `idUsuarioAnulador` int(11) DEFAULT NULL,
  PRIMARY KEY (`IdPago`),
  KEY `FK_Pagos_Reservas` (`IdReserva`),
  CONSTRAINT `FK_Pagos_Reservas` FOREIGN KEY (`IdReserva`) REFERENCES `reservas` (`IdReserva`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


LOCK TABLES `tipoinmueble` WRITE;
/*!40000 ALTER TABLE `tipoinmueble` DISABLE KEYS */;
INSERT INTO `tipoinmueble` VALUES (1,'Casa',1),(2,'Departamento',1),(3,'Loft',1),(4,'MonoAmbiente',1),(5,'Cabaña',1);
/*!40000 ALTER TABLE `tipoinmueble` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `propietarios` WRITE;
/*!40000 ALTER TABLE `propietarios` DISABLE KEYS */;
INSERT INTO `propietarios` VALUES (1,'Gabriela','Ballerini','31234567','2665789876','gabriela@test.com',1),(2,'Brian','Delicia','38765432','2664398765','brian@test.com',1),(3,'Fatima','Alcaraz','34899899','2664202034','fattima@test.com',1),(4,'Martin','Machado','34234567','2664009988','martin@test.com',1);
/*!40000 ALTER TABLE `propietarios` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `inquilinos` WRITE;
/*!40000 ALTER TABLE `inquilinos` DISABLE KEYS */;
INSERT INTO `inquilinos` VALUES (1,'Jose','Lopez','29876543','2664556677','jose@test.com',1),(2,'Lucia','Diaz','32789098','2664123456','luchi@test.com',1),(3,'Carlos','Alvaraz','25678901','2665987654','carlos@test.com',1),(4,'Florencia','Fernandez','35678876','2665430987','florencia@test.com',1),(5,'Lucas','Robles','36789987','2665122123','lucas@test.com',1);
/*!40000 ALTER TABLE `inquilinos` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'admin@inmobiliaria.com','+dd8qkgMBJyCvAyMHeU0M0XnK0Kj0eWEYR9jsIHzsLQ=',1,1,'/uploads/avatars/b42bdd78-044d-4a39-ace3-4176ed2d70db.jpg'),(2,'empleado@inmobiliaria.com','xb+3hzyR8qa1cwRblWJpVGNz42rrYjKCCL25kErBmE0=',0,1,NULL);
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `inmuebles` WRITE;
/*!40000 ALTER TABLE `inmuebles` DISABLE KEYS */;
INSERT INTO `inmuebles` VALUES (1,'Avenida España 1200',4,-33.29397100,-66.34552800,45000.00,3,1,1,15,'/Uploads/Inmuebles/1/c8b9a7cb-7cff-4b15-9d1f-127118387f42.jpg'),(2,'Junin 1667',2,-33.30362000,-66.34770100,28000.00,2,2,1,15,'/Uploads/Inmuebles/2/6943d676-e883-4f53-8a1c-a239d7a44e48.jpg'),(3,'Lamadrid 456',6,-33.29438800,-66.32641300,50000.00,1,1,1,20,'/Uploads/Inmuebles/3/d2c93baa-e72c-46d5-90ee-474460fd442b.jpg'),(4,'Los Pejes 456',3,-33.28216500,-66.26632900,34000.00,2,5,1,15,'/Uploads/Inmuebles/4/06794d9a-3753-45ae-8f14-583edf0391f8.jpg'),(5,'Mitre 818',2,-33.30260300,-66.34060700,32000.00,4,2,1,10,'/Uploads/Inmuebles/5/2afe1c8b-a615-4531-9ef9-03d56b337de4.jpg'),(6,'Concaran 312',3,-33.27828700,-66.24369900,30000.00,4,5,1,20,'/Uploads/Inmuebles/6/8b158348-1c99-4e50-9f6f-ec91b539968b.jpg');
/*!40000 ALTER TABLE `inmuebles` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `imagenes` WRITE;
/*!40000 ALTER TABLE `imagenes` DISABLE KEYS */;
INSERT INTO `imagenes` VALUES (1,1,'/Uploads/Inmuebles/1/c8b9a7cb-7cff-4b15-9d1f-127118387f42.jpg'),(2,2,'/Uploads/Inmuebles/2/6943d676-e883-4f53-8a1c-a239d7a44e48.jpg'),(3,3,'/Uploads/Inmuebles/3/d2c93baa-e72c-46d5-90ee-474460fd442b.jpg'),(4,4,'/Uploads/Inmuebles/4/06794d9a-3753-45ae-8f14-583edf0391f8.jpg'),(5,5,'/Uploads/Inmuebles/5/2afe1c8b-a615-4531-9ef9-03d56b337de4.jpg'),(6,6,'/Uploads/Inmuebles/6/8b158348-1c99-4e50-9f6f-ec91b539968b.jpg');
/*!40000 ALTER TABLE `imagenes` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `reservas` WRITE;
/*!40000 ALTER TABLE `reservas` DISABLE KEYS */;
INSERT INTO `reservas` VALUES (1,3,4,1,34000.00,'2026-09-18 15:00:00','2026-09-20 10:00:00',1,NULL,NULL,NULL),(2,1,6,1,30000.00,'2026-09-18 15:00:00','2026-09-22 10:00:00',1,NULL,NULL,NULL),(3,2,1,1,45000.00,'2026-09-23 15:00:00','2026-09-25 10:00:00',1,NULL,NULL,NULL),(4,3,2,1,28000.00,'2026-10-01 15:00:00','2026-10-04 10:00:00',1,NULL,NULL,NULL),(5,4,2,1,28000.00,'2026-09-24 15:00:00','2026-09-26 10:00:00',1,NULL,NULL,NULL),(6,5,2,1,28000.00,'2026-09-27 15:00:00','2026-09-30 10:00:00',1,NULL,NULL,NULL),(7,3,3,1,50000.00,'2026-09-25 15:00:00','2026-09-30 10:00:00',1,NULL,NULL,NULL),(8,1,1,1,45000.00,'2026-09-18 15:00:00','2026-09-21 10:00:00',0,NULL,NULL,NULL),(9,1,4,1,34000.00,'2026-09-23 15:00:00','2026-09-29 10:00:00',1,NULL,NULL,NULL);
/*!40000 ALTER TABLE `reservas` ENABLE KEYS */;
UNLOCK TABLES;


LOCK TABLES `pagos` WRITE;
/*!40000 ALTER TABLE `pagos` DISABLE KEYS */;
INSERT INTO `pagos` VALUES (1,1,'2026-09-17 22:40:53',1,1,2,10200,1,NULL),(2,2,'2026-09-17 22:41:28',1,1,4,24000,1,NULL),(3,3,'2026-09-17 22:42:04',1,1,1,13500,1,NULL),(4,4,'2026-09-17 22:42:29',1,1,3,12600,1,NULL),(5,5,'2026-09-17 22:44:41',1,1,1,8400,1,NULL),(6,6,'2026-09-17 22:45:16',1,1,2,12600,1,NULL),(7,7,'2026-09-17 22:46:01',1,1,3,50000,1,NULL),(8,8,'2026-09-17 22:46:21',1,1,1,20250,1,NULL),(9,9,'2026-09-17 22:47:04',1,1,1,30600,1,NULL);
/*!40000 ALTER TABLE `pagos` ENABLE KEYS */;
UNLOCK TABLES;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-17 23:38:57