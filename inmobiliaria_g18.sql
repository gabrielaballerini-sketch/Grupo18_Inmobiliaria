SET FOREIGN_KEY_CHECKS = 0;

DROP DATABASE IF EXISTS `inmobiliaria_g18`;

CREATE DATABASE `inmobiliaria_g18` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `inmobiliaria_g18`;

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
  PRIMARY KEY (`IdInmueble`),
  KEY `FK_Inmuebles_Propietarios` (`IdPropietario`),
  KEY `FK_Inmuebles_TipoInmueble` (`IdTipoInmueble`),
  CONSTRAINT `FK_Inmuebles_Propietarios` FOREIGN KEY (`IdPropietario`) REFERENCES `propietarios` (`IdPropietario`),
  CONSTRAINT `FK_Inmuebles_TipoInmueble` FOREIGN KEY (`IdTipoInmueble`) REFERENCES `tipoinmueble` (`IdTipoInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO inmuebles (IdInmueble,Direccion,Capacidad,Latitud,Longitud,PrecioAlquiler,IdPropietario,IdTipoInmueble,Estado) VALUES
	 (1,'Mitre 818',2,-33.29700000,-66.33700000,35000.00,2,2,1),
	 (2,'Los Almendros 1234',2,-45.89900000,-67.67800000,55000.00,4,4,0),
	 (3,'Junin 1667',4,-56.78900000,-89.98700000,60000.00,3,1,1),
	 (4,'Heroes de Malvinas 1234',3,-87.98700000,-76.56700000,55000.00,1,2,1);


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
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO inquilinos (IdInquilino,Nombre,Apellido,Dni,Telefono,Email,Estado) VALUES 
(5,'Roman','Riquelme','23654987','266578956','roman@gmail.com',1),
(6,'Florencia','Deli','23654654','2665325487','flor@gmail.com',1),
(7,'Lucia','Perez','35754987','2665459865','perez@gmail.com',1),
(8,'Macarena','Baigorria','23654986','2665258964','maca@gmail.com',1),
(9,'Juan','Perez','38439987','2665489887','juan@test.com',1),
(10,'Yeni','Contrera','25654987','2664589878','yeni@gmail.com',1),
(12,'Carlos','Delicia','78985654','2664896532','carlos@test.com',1),
(13,'Jasmin','Peña','25456789','2665479865','esteban@gmail.com',1),
(14,'Jose','Jofre','45789693','2665487898','jofre@gmail.com',1);

CREATE TABLE `pagos` (
  `IdPago` int(11) NOT NULL AUTO_INCREMENT,
  `IdReserva` int(11) NOT NULL,
  `PagoParcial` decimal(12,2) NOT NULL,
  `PagoTotal` decimal(12,2) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `TipoPago` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdPago`),
  KEY `FK_Pagos_Reservas` (`IdReserva`),
  CONSTRAINT `FK_Pagos_Reservas` FOREIGN KEY (`IdReserva`) REFERENCES `reservas` (`IdReserva`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO propietarios (IdPropietario,Nombre,Apellido,Dni,Telefono,Email,Estado) VALUES 
(1,'Brian','Delicia','23654987','266578956','deli@gmail.com',1),
(2,'Fatima','Alcaraz','25321654','2665879632','faty@gmail.com',1),
(3,'Gabriela','Ballerini','45987652','2665326598','gabi@gmail.com',1),
(4,'Enrique','Ramos','56987741','2665854587','Ramos@gmail.com',1),
(5,'Tomas','Torres','38789456','2664322154','tomy@gmail.com',1),
(6,'Estevan','Gomez','25456123','2664589887','gomez@test.com',1);

CREATE TABLE `reservas` (
  `IdReserva` int(11) NOT NULL AUTO_INCREMENT,
  `IdInquilino` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `IdUsuario` int(11) NOT NULL,
  `MontoDiario` decimal(12,2) NOT NULL,
  `FechaInicio` datetime NOT NULL,
  `FechaFin` datetime NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdReserva`),
  KEY `FK_Reservas_Inquilinos` (`IdInquilino`),
  KEY `FK_Reservas_Inmuebles` (`IdInmueble`),
  KEY `FK_Reservas_Usuarios` (`IdUsuario`),
  CONSTRAINT `FK_Reservas_Inmuebles` FOREIGN KEY (`IdInmueble`) REFERENCES `inmuebles` (`IdInmueble`),
  CONSTRAINT `FK_Reservas_Inquilinos` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilinos` (`IdInquilino`),
  CONSTRAINT `FK_Reservas_Usuarios` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`IdUsuario`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO reservas (IdInquilino,IdInmueble,IdUsuario,MontoDiario,FechaInicio,FechaFin,Estado) VALUES
	 (8,1,1,38000.00,'2026-09-04 15:15:00','2026-09-05 10:00:00',1),
	 (13,4,1,55000.00,'2026-09-11 15:00:00','2026-09-13 10:00:00',1);

CREATE TABLE `tipoinmueble` (
  `IdTipoInmueble` int(11) NOT NULL AUTO_INCREMENT,
  `Descripcion` varchar(150) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdTipoInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO tipoinmueble (IdTipoInmueble, Descripcion, Estado) VALUES 
(1, 'Casa', 1),
(2, 'Departamento', 1),
(3, 'Monoambiente', 1),
(4, 'Loft', 1);

CREATE TABLE `usuarios` (
  `IdUsuario` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `RolUsuario` int(11) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdUsuario`),
  UNIQUE KEY `UserName` (`UserName`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO usuarios (UserName,Password,RolUsuario,Estado) VALUES
	 ('admin','admin',1,1);

SET FOREIGN_KEY_CHECKS = 1;     
