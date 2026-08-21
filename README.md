#GRUPO5 GESTION ALQUILERES TEMPORALES
Este proyecto trata de una inmobiliaria que se encarga de gestionar alquileres temporales de  inmuebles, teniedo trato con dueños y inquilinos 

-------LISTA DE INTEGRANTES-------
1-Delicia Brian
2-Ballerini Gabriela
3-Alcaraz Fatima

-------DIAGRAMA DE CLASES-------------
https://app.diagrams.net/#G18Scs2MIZVE_8W8LoGQm6b49wo-_AWy0t#%7B%22pageId%22%3A%22cxBcQCD3t9tabTOSOMA3%22%7D

--------BASE DE DATOS MYSQL----------
create database inmobiliaria_g18;
use inmobiliaria_g18;
select database();

CREATE TABLE `propietarios` (
  `IdPropietario` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Telefono` varchar(50) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdPropietario`),
  UNIQUE KEY `Dni` (`Dni`));

CREATE TABLE `inquilinos` (
  `IdInquilino` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Telefono` varchar(50) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdInquilino`),
  UNIQUE KEY `Dni` (`Dni`));

CREATE TABLE `usuarios` (
  `IdUsuario` int NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `RolUsuario` int NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdUsuario`),
  UNIQUE KEY `UserName` (`UserName`));

CREATE TABLE `inmuebles` (
  `IdInmueble` int NOT NULL AUTO_INCREMENT,
  `Direccion` varchar(200) NOT NULL,
  `Capacidad` int NOT NULL,
  `TipoInmueble` int NOT NULL,
  `Coordenadas` float DEFAULT NULL,
  `PrecioAlquiler` decimal(12,2) NOT NULL,
  `IdPropietario` int NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdInmueble`),
  KEY `FK_Inmuebles_Propietarios` (`IdPropietario`),
  CONSTRAINT `FK_Inmuebles_Propietarios` FOREIGN KEY (`IdPropietario`) REFERENCES `propietarios` (`IdPropietario`));

CREATE TABLE `reservas` (
  `IdReserva` int NOT NULL AUTO_INCREMENT,
  `IdInquilino` int NOT NULL,
  `IdInmueble` int NOT NULL,
  `IdUsuario` int NOT NULL,
  `MontoDiario` decimal(12,2) NOT NULL,
  `FechaInicio` datetime NOT NULL,
  `FechaFin` datetime NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdReserva`),
  KEY `FK_Reservas_Inquilinos` (`IdInquilino`),
  KEY `FK_Reservas_Inmuebles` (`IdInmueble`),
  KEY `FK_Reservas_Usuarios` (`IdUsuario`),
  CONSTRAINT `FK_Reservas_Inmuebles` FOREIGN KEY (`IdInmueble`) REFERENCES `inmuebles` (`IdInmueble`),
  CONSTRAINT `FK_Reservas_Inquilinos` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilinos` (`IdInquilino`),
  CONSTRAINT `FK_Reservas_Usuarios` FOREIGN KEY (`IdUsuario`) REFERENCES `usuarios` (`IdUsuario`));

CREATE TABLE `pagos` (
  `IdPago` int NOT NULL AUTO_INCREMENT,
  `IdReserva` int NOT NULL,
  `PagoParcial` decimal(12,2) NOT NULL,
  `PagoTotal` decimal(12,2) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `TipoPago` int NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdPago`),
  KEY `FK_Pagos_Reservas` (`IdReserva`),
  CONSTRAINT `FK_Pagos_Reservas` FOREIGN KEY (`IdReserva`) REFERENCES `reservas` (`IdReserva`));


------CREACION DE INQUILINOS Y PROPIETARIOS--------

use inmobiliaria_g18;
select database();

insert into Propietarios(Nombre,Apellido,Dni,Telefono,Email,Estado)
values ("Brian","Delicia",23654987,"266578956","deli@gmail.com",true),
("fatima","alcaraz",25321654,"2665879632","faty@gmail.com",true),
("gabriela","ballerini",45987652,"2665326598","gabi@gmail.com",true),
("Enrique","Ramos",56987741,"2665854587","Ramos@gmail.com",true),
("tomas","Torres",38789456,"2664322154","tomy@gmail.com",true);

insert into inquilinos(Nombre,Apellido,Dni,Telefono,Email,Estado)
values ("Roman","Riquelme",23654987,"266578956","roman@gmail.com",true),
("florencia","deli",23654654,"2665325487","flor@gmail.com",true),
("lucia","perez",35754987,"2665459865","perez@gmail.com",true),
("macarena","Baigorria",23654986,"2665258964","maca@gmail.com",true);




------DATOS PARA LA CONECCION A LA BASE----------
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost; Port=3306; Database=inmobiliaria_g18; User=root;Password=;"
  }
  
-----PARA PODER REVISAR EN EL NAVEGADOR ESCRIBIR-----
http://localhost:5159/Inquilino

http://localhost:5159/Propietario
