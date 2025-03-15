using StationnementAPI;
using StationnementAPI.Data.Context;
using StationnementAPI.Controllers;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StationnementAPI.Models;
using StationnementAPI.Models.ModelsDTO;

namespace TestStationnementAPI
{
    [TestClass]
    public class TestPaiement
    {
        private StationnementDbContext context;
        private PaiementController paiementController;
        private DatabaseHelper databaseHelper;

        /// <summary>
        /// Initialise le contexte de test avant chaque méthode de test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            databaseHelper = new DatabaseHelper();
            context = databaseHelper.CreateContext();
            paiementController = new PaiementController(context); 
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
        /// Teste la méthode CalculerMontantTicket avec un ticket valide et un tarif horaire.
        /// </summary>
        [TestMethod]
        public async Task TestCalculerMontantTicket_TarifHoraire()
        {
            context.Database.EnsureCreated();
            var ticket = new Ticket
            {
                Id = "TICKET123",
                TempsArrive = DateTime.Now.AddHours(-1), // Ticket arrivé il y a 1 heure
                EstPaye = false,
                EstConverti = false
            };
            context.Tickets.Add(ticket);

            var tarificationHoraire = new Tarification
            {
                Id = 1,
                Niveau = "Tarif horaire",
                Prix = 2.50m,
                DureeMin = 0,
                DureeMax = 2
            };
            context.Tarifications.Add(tarificationHoraire);

            var utilisateur = new Utilisateur
            {
                Id = 1,
                NomUtilisateur = "",
                MotDePasse = "",
                Role = "admin",
                Email = "",
            };
            context.Add(utilisateur);

            var configuration = new Configuration
            {
                TaxeFederal = 5.60m,
                TaxeProvincial = 7.80m,
                DateModification = DateTime.Now, 
                UtilisateurId = utilisateur.Id,
            };
            context.Configurations.Add(configuration);

            await context.SaveChangesAsync();

            var result = await paiementController.CalculerMontantTicket("TICKET123");

            result.Should().BeOfType<ActionResult<object>>(); 
            var actionResult = result as ActionResult<object>; 
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un OkObjectResult
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull(); 
            okResult.Value.Should().NotBeNull(); 


            var montantResult = okResult.Value;
            montantResult.GetType().GetProperty("MontantAvecTaxes").GetValue(montantResult).Should().Be(2.84m); // Vérifie le montant avec taxes
        }


        /// <summary>
        /// Teste la méthode CalculerMontantTicket avec un ticket valide et un tarif demi-journée.
        /// </summary>
        [TestMethod]
        public async Task TestCalculerMontantTicket_TarifDemiJournee()
        {
            context.Database.EnsureCreated();

            var ticket = new Ticket
            {
                Id = "TICKET123",
                TempsArrive = DateTime.Now.AddHours(-3), // Ticket arrivé il y a 3 heures
                EstPaye = false,
                EstConverti = false
            };
            context.Tickets.Add(ticket);

            var tarificationDemiJournee = new Tarification
            {
                Id = 2,
                Niveau = "Demi-journée",
                Prix = 6.50m,
                DureeMin = 2,
                DureeMax = 4
            };
            context.Tarifications.Add(tarificationDemiJournee);

            var utilisateur = new Utilisateur
            {
                Id = 1,
                NomUtilisateur = "",
                MotDePasse = "",
                Role = "admin",
                Email = "",
            };
            context.Add(utilisateur);

            var configuration = new Configuration
            {
                TaxeFederal = 5.60m,
                TaxeProvincial = 7.80m,
                DateModification = DateTime.Now,
                UtilisateurId = utilisateur.Id,
            };
            context.Configurations.Add(configuration);

            await context.SaveChangesAsync();

           
            var result = await paiementController.CalculerMontantTicket("TICKET123");

            
            result.Should().BeOfType<ActionResult<object>>(); 
            var actionResult = result as ActionResult<object>; 
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un OkObjectResult
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull(); 
            okResult.Value.Should().NotBeNull(); 

            var montantResult = okResult.Value;
            montantResult.GetType().GetProperty("MontantAvecTaxes").GetValue(montantResult).Should().Be(7.37m); // Vérifie le montant avec taxes
        }

        /// <summary>
        /// Teste la méthode CalculerMontantTicket avec un ticket valide et un tarif journée complète.
        /// </summary>
        [TestMethod]
        public async Task TestCalculerMontantTicket_TarifJourneeComplete()
        {
            context.Database.EnsureCreated();
            
            var ticket = new Ticket
            {
                Id = "TICKET123",
                TempsArrive = DateTime.Now.AddHours(-5), // Ticket arrivé il y a 5 heures
                EstPaye = false,
                EstConverti = false
            };
            context.Tickets.Add(ticket);

            var tarificationJournee = new Tarification
            {
                Id = 3,
                Niveau = "Journée complète",
                Prix = 10.75m,
                DureeMin = 4,
                DureeMax = 24
            };
            context.Tarifications.Add(tarificationJournee);

            var utilisateur = new Utilisateur
            {
                Id = 1,
                NomUtilisateur = "",
                MotDePasse = "",
                Role = "admin",
                Email = "",
            };
            context.Add(utilisateur);

            var configuration = new Configuration
            {
                TaxeFederal = 5.60m,
                TaxeProvincial = 7.80m,
                DateModification = DateTime.Now,
                UtilisateurId = utilisateur.Id,
            };
            context.Configurations.Add(configuration);

            await context.SaveChangesAsync();

            var result = await paiementController.CalculerMontantTicket("TICKET123");

            result.Should().BeOfType<ActionResult<object>>(); 
            var actionResult = result as ActionResult<object>; 
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un OkObjectResult
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull(); 
            okResult.Value.Should().NotBeNull(); 

            var montantResult = okResult.Value;
            montantResult.GetType().GetProperty("MontantAvecTaxes").GetValue(montantResult).Should().Be(12.19m); // Vérifie le montant avec taxes
        }

        /// <summary>
        /// Teste la méthode CalculerMontantTicket avec un ticket valide et un tarif horaire.
        /// </summary>
        /// </summary>
        [TestMethod]
        public async Task TestPayerTicket_TarifHoraire()
        {
            context.Database.EnsureCreated();
          
            var ticket = new Ticket
            {
                Id = "TICKET123",
                TempsArrive = DateTime.Now.AddHours(-1), // Ticket arrivé il y a 1 heure
                EstPaye = false,
                EstConverti = false
            };
            context.Tickets.Add(ticket);

            var tarificationHoraire = new Tarification
            {
                Id = 1,
                Niveau = "Tarif horaire",
                Prix = 2.50m,
                DureeMin = 0,
                DureeMax = 2
            };
            context.Tarifications.Add(tarificationHoraire);

            var utilisateur = new Utilisateur
            {
                Id = 1,
                NomUtilisateur = "",
                MotDePasse = "",
                Role = "admin",
                Email = "",
            };
            context.Add(utilisateur);

            var configuration = new Configuration
            {
                TaxeFederal = 5.60m,
                TaxeProvincial = 7.80m,
                DateModification = DateTime.Now,
                UtilisateurId = utilisateur.Id,
            };
            context.Configurations.Add(configuration);

            await context.SaveChangesAsync();

            var paiementDto = new PaiementDto
            {
                TicketId = "TICKET123"
            };

           
            var result = await paiementController.PayerTicket(paiementDto);

            
            result.Should().BeOfType<OkObjectResult>(); 
            var okResult = result as OkObjectResult;
            okResult.Value.Should().NotBeNull(); 

            var paiementResult = okResult.Value;
            paiementResult.GetType().GetProperty("MontantAvecTaxes").GetValue(paiementResult).Should().Be(2.84m); // Vérifie le montant avec taxes
        }
    }
}