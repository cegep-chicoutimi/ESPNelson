using StationnementAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Net;
using StationnementAPI.Models;
using StationnementAPI.Controllers;
using StationnementAPI.Data.Context;
using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Adapter;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Models.ModelsDTO;
using Microsoft.AspNetCore.Http;

namespace TestStationnementAPI
{
    [TestClass]
    public class TestAbonnement
    {
        private StationnementDbContext context;
        private AbonnementController abonnementController;
        private DatabaseHelper databaseHelper;

        /// <summary>
        /// Initialise le contexte de test avant chaque méthode de test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            databaseHelper = new DatabaseHelper();
            context = databaseHelper.CreateContext();
            abonnementController = new AbonnementController(context);
        }

        /// <summary>
        /// Nettoie les données de test après chaque méthode de test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            databaseHelper.DropTestTables(context);
        }

        /// <summary>
        /// Vérifier qu'un abonnement est correctement créé lorsque les données sont valides.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestSouscrireAbonnement_Success()
        {
            context.Database.EnsureCreated();
            
            var ticket = new Ticket
            {
                Id = "TICKET123",
                TempsArrive = DateTime.Now.AddHours(-1),
                EstPaye = false,
                EstConverti = false
            };
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var paiementDto = new PaiementDto
            {
                TicketId = "TICKET123",
                Email = "test@example.com",
                TypeAbonnement = "hebdomadaire"
            };

           
            var result = await abonnementController.SouscrireAbonnement(paiementDto);

            
            result.Should().BeOfType<CreatedResult>(); 
            var createdResult = result as CreatedResult;
            createdResult.Should().NotBeNull(); 
            createdResult.Value.Should().NotBeNull(); 

           
            var abonnementResult = createdResult.Value; 
            var abonnementId = abonnementResult.GetType().GetProperty("AbonnementId")?.GetValue(abonnementResult);
            abonnementId.Should().NotBeNull(); 
            abonnementId.Should().BeOfType<string>(); 

            // Vérifie que l'abonnement existe dans la base de données
            var abonnement = await context.Abonnements.FirstOrDefaultAsync(a => a.Id == abonnementId as string);
            abonnement.Should().NotBeNull(); 
        }

        /// <summary>
        /// Vérifier que la méthode retourne une erreur si le ticket a déjà été converti en abonnement.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestSouscrireAbonnement_TicketDejaConverti()
        {
            context.Database.EnsureCreated();
           
            var ticket = new Ticket
            {
                Id = "TICKET123",
                TempsArrive = DateTime.Now.AddHours(-1),
                EstPaye = false,
                EstConverti = true // Ticket déjà converti
            };
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var paiementDto = new PaiementDto
            {
                TicketId = "TICKET123",
                Email = "test@example.com",
                TypeAbonnement = "hebdomadaire"
            };

            
            var result = await abonnementController.SouscrireAbonnement(paiementDto);

            
            result.Should().BeOfType<ConflictObjectResult>(); // Vérifie que le résultat est de type ConflictObjectResult
            var conflictResult = result as ConflictObjectResult;
            conflictResult.Should().NotBeNull(); 
            conflictResult.Value.Should().Be("Ce ticket a déjà été utilisé pour un abonnement."); // Vérifie le message d'erreur
        }


        /// <summary>
        /// Vérifier que la méthode retourne une erreur si l'email est déjà associé à un utilisateur.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestSouscrireAbonnement_EmailDejaUtilise()
        {
            context.Database.EnsureCreated();

            
            var ticket = new Ticket
            {
                Id = "TICKET123",
                TempsArrive = DateTime.Now.AddHours(-1),
                EstPaye = false,
                EstConverti = false
            };
            context.Tickets.Add(ticket);

           
            var utilisateur = new Utilisateur
            {
                NomUtilisateur = "test",
                MotDePasse = "password",
                Email = "test@example.com",
                Role = "abonne"
            };
            context.Utilisateurs.Add(utilisateur);

           
            await context.SaveChangesAsync();

            //PaiementDto avec l'email déjà utilisé
            var paiementDto = new PaiementDto
            {
                TicketId = "TICKET123",
                Email = "test@example.com", // Email déjà utilisé
                TypeAbonnement = "hebdomadaire"
            };

            // Act
            var result = await abonnementController.SouscrireAbonnement(paiementDto);

            // Assert
            result.Should().BeOfType<ConflictObjectResult>(); // Vérifie que le résultat est de type ConflictObjectResult
            var conflictResult = result as ConflictObjectResult;
            conflictResult.Should().NotBeNull(); 
            conflictResult.Value.Should().Be("Cet email est déjà associé à un abonné."); // Vérifie le message d'erreur
        }


        /// <summary>
        /// Vérifier que la méthode retourne une erreur si TicketId ou Email est manquant.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestSouscrireAbonnement_DonneesManquantes()
        {
            context.Database.EnsureCreated();

           
            var paiementDto = new PaiementDto
            {
                TicketId = "", // TicketId manquant
                Email = "", // Email manquant
                TypeAbonnement = "hebdomadaire"
            };

            
            var result = await abonnementController.SouscrireAbonnement(paiementDto);

            
            result.Should().BeOfType<BadRequestObjectResult>(); // Vérifie que le résultat est de type BadRequestObjectResult
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull(); 
            badRequestResult.Value.Should().Be("TicketId est requis pour souscrire un abonnement."); 
        }


        /// <summary>
        /// Vérifier que les détails de l'abonnement sont retournés si l'abonnement est actif.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetAbonnement_Actif()
        {
            context.Database.EnsureCreated();

            var utilisateur = new Utilisateur
            {
                Id = 1,
                NomUtilisateur = "",
                MotDePasse = "",
                Role = "admin",
                Email = "",
            };
            context.Utilisateurs.Add(utilisateur);

            var abonnement = new Abonnement
            {
                Id = "ABON123",
                UtilisateurId = utilisateur.Id,
                DateDebut = DateTime.UtcNow.AddDays(-1), // Abonnement actif
                DateFin = DateTime.UtcNow.AddDays(6),
                Type = "hebdomadaire"
            };
            context.Abonnements.Add(abonnement);
            await context.SaveChangesAsync();

           
            var result = await abonnementController.GetAbonnement("ABON123");

            
            result.Should().BeOfType<ActionResult<object>>(); 
            var actionResult = result as ActionResult<object>; 
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un OkObjectResult
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull(); 
            okResult.Value.Should().NotBeNull(); 

            // Vérifie les détails de l'abonnement
            var abonnementResult = okResult.Value;
            abonnementResult.GetType().GetProperty("AbonnementId")?.GetValue(abonnementResult).Should().Be("ABON123");
            abonnementResult.GetType().GetProperty("TypeAbonnement")?.GetValue(abonnementResult).Should().Be("hebdomadaire");
        }

        /// <summary>
        /// Vérifier que la méthode retourne une erreur si l'abonnement est expiré.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetAbonnement_Expire()
        {
            context.Database.EnsureCreated();

            var utilisateur = new Utilisateur
            {
                Id = 1,
                NomUtilisateur = "",
                MotDePasse = "",
                Role = "admin",
                Email = "",
            };

            context.Utilisateurs.Add(utilisateur);

            var abonnement = new Abonnement
            {
                Id = "ABON123",
                UtilisateurId = utilisateur.Id,
                DateDebut = DateTime.UtcNow.AddDays(-30), // Abonnement expiré
                DateFin = DateTime.UtcNow.AddDays(-1),
                Type = "mensuel"
            };
            context.Abonnements.Add(abonnement);
            await context.SaveChangesAsync();

           
            var result = await abonnementController.GetAbonnement("ABON123");

            
            result.Should().BeOfType<ActionResult<object>>(); // Vérifie que le résultat est de type ActionResult<object>
            var actionResult = result as ActionResult<object>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un BadRequestObjectResult
            var badRequestResult = actionResult.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull(); 
            badRequestResult.Value.Should().Be("Cet abonnement n'est plus actif rendu à cette date"); 
        }



        /// <summary>
        /// Vérifier que la méthode retourne une erreur si l'abonnement n'existe pas
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestGetAbonnement_NonTrouve()
        {
            context.Database.EnsureCreated();

           
            // Aucun abonnement n'est ajouté à la base de données

            
            var result = await abonnementController.GetAbonnement("ABON123");
            result.Should().BeOfType<ActionResult<object>>(); // Vérifie que le résultat est de type ActionResult<object>
            var actionResult = result as ActionResult<object>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un NotFoundObjectResult
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull(); 
            notFoundResult.Value.Should().Be("Aucun abonnement existant pour ce ticket !"); 
        }
    }
}
