-- création de la BD il faut que l'utilisateur ait les privilèges pour le faire
-- CREATE DATABASE IF NOT EXISTS `nomBD` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- USE `nomBD`;

SET FOREIGN_KEY_CHECKS = 0; -- pour éviter des erreurs de dépendance des tables

-- En respectant l'orde des dépendance des tables
DROP TABLE IF EXISTS paiement;
DROP TABLE IF EXISTS configuration;
DROP TABLE IF EXISTS abonnement;
DROP TABLE IF EXISTS ticket;
DROP TABLE IF EXISTS utilisateur;
DROP TABLE IF EXISTS tarification;

-- on réative les dépendance des tables
SET FOREIGN_KEY_CHECKS = 1;

-- Création des tables dans le bon ordre
CREATE TABLE `tarification` (
   `Id` int NOT NULL AUTO_INCREMENT,
   `Niveau` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   `Prix` decimal(10,2) NOT NULL,
   `DureeMin` int NOT NULL,
   `DureeMax` int NOT NULL,
   PRIMARY KEY (`Id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Insertion des tarifications prédéfinies
INSERT INTO `tarification` (Niveau, Prix, DureeMin, DureeMax)
VALUES 
('Tarif horaire', 0, 0, 2),
('Demi-journée', 6.25, 1, 4),
('Journée complète', 10.75, 4, 24);

CREATE TABLE `ticket` (
   `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   `TempsArrive` datetime(6) NOT NULL,
   `EstPaye` tinyint(1) NOT NULL,
   `TempsSortie` datetime(6) DEFAULT NULL,
   `EstConverti` tinyint(1) NOT NULL,
   PRIMARY KEY (`Id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 CREATE TABLE `utilisateur` (
   `Id` int NOT NULL AUTO_INCREMENT,
   `NomUtilisateur` varchar(205) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   `MotDePasse` varchar(205) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   `Role` varchar(205) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   `Email` varchar(205) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   PRIMARY KEY (`Id`),
   UNIQUE KEY `IX_Utilisateur_Email` (`Email`)
 ) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 -- Insertion d'un utilisateur admin
INSERT INTO `utilisateur` (Id, NomUtilisateur, MotDePasse, Role, Email)
VALUES 
(1,'admin', 'MmQzj0CFPaOv1Two6uwji4cZCMuYKY7B/Wi8myJ00UeaDob7', 'admin', 'nelsonyimou@gmail.com');
 
 CREATE TABLE `abonnement` (
   `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   `DateDebut` datetime(6) NOT NULL,
   `DateFin` datetime(6) NOT NULL,
   `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
   `UtilisateurId` int NOT NULL,
   PRIMARY KEY (`Id`),
   KEY `IX_Abonnement_UtilisateurId` (`UtilisateurId`),
   CONSTRAINT `FK_Abonnement_Utilisateur_UtilisateurId` FOREIGN KEY (`UtilisateurId`) REFERENCES `utilisateur` (`Id`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 CREATE TABLE `paiement` (
   `Id` int NOT NULL AUTO_INCREMENT,
   `TicketId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
   `AbonnementId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
   `Montant` decimal(10,2) NOT NULL,
   `DatePaiement` datetime(6) NOT NULL,
   `TarificationDureeMax` int DEFAULT NULL,
   `TarificationDureeMin` int DEFAULT NULL,
   `TarificationNiveau` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
   `TarificationPrix` decimal(10,2) DEFAULT NULL,
   PRIMARY KEY (`Id`),
   UNIQUE KEY `IX_Paiement_AbonnementId` (`AbonnementId`),
   UNIQUE KEY `IX_Paiement_TicketId` (`TicketId`),
   CONSTRAINT `FK_Paiement_Abonnement_AbonnementId` FOREIGN KEY (`AbonnementId`) REFERENCES `abonnement` (`Id`) ON DELETE CASCADE,
   CONSTRAINT `FK_Paiement_Ticket_TicketId` FOREIGN KEY (`TicketId`) REFERENCES `ticket` (`Id`) ON DELETE CASCADE
 ) ENGINE=InnoDB AUTO_INCREMENT=551 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 CREATE TABLE `configuration` (
   `Id` int NOT NULL AUTO_INCREMENT,
   `CapaciteMax` int NOT NULL,
   `TaxeFederal` decimal(10,2) NOT NULL,
   `TaxeProvincial` decimal(10,2) NOT NULL,
   `DateModification` datetime(6) NOT NULL,
   `UtilisateurId` int NOT NULL,
   PRIMARY KEY (`Id`),
   KEY `IX_Configuration_UtilisateurId` (`UtilisateurId`),
   CONSTRAINT `FK_Configuration_Utilisateur_UtilisateurId` FOREIGN KEY (`UtilisateurId`) REFERENCES `utilisateur` (`Id`) ON DELETE CASCADE
 ) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 -- Insertion des tarifications prédéfinies
INSERT INTO `configuration` (CapaciteMax, TaxeFederal, TaxeProvincial, DateModification, UtilisateurId)
VALUES 
(250, 5.00, 6.00, '2025-03-10', 1);

 
 
 