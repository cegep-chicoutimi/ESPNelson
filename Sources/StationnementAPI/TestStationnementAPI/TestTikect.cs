using StationnementAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StationnementAPI.Models;
using StationnementAPI.Data.Context;

namespace TestStationnementAPI
{
    [TestClass]
    public class TestTicket
    {
        private StationnementDbContext context;
        private TicketController ticketController;
        private DatabaseHelper databaseHelper;

        /// <summary>
        /// Initialise le contexte de test avant chaque méthode de test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            databaseHelper = new DatabaseHelper();
            context = databaseHelper.CreateContext();
            ticketController = new TicketController(context); 
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
        /// Teste la méthode GenererTicket pour vérifier qu'un ticket est correctement généré.
        /// </summary>
        [TestMethod]
        public async Task TestGenererTicket_Success()
        {
            
            context.Database.EnsureCreated(); 

            var result = await ticketController.GenererTicket();

            result.Should().BeOfType<ActionResult<Ticket>>(); // Vérifie que le résultat est de type ActionResult<Ticket>
            var actionResult = result as ActionResult<Ticket>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un CreatedAtActionResult
            var createdResult = actionResult.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull(); 
            createdResult.Value.Should().NotBeNull(); 

            // Vérifie que le ticket a été correctement créé
            var ticket = createdResult.Value as Ticket;
            ticket.Should().NotBeNull(); 
            ticket.Id.Should().NotBeNullOrEmpty(); 
            ticket.TempsArrive.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1)); 
        }


        /// <summary>
        /// Teste la méthode GetAllTickets lorsqu'il n'y a aucun ticket dans la base de données.
        /// </summary>
        [TestMethod]
        public async Task TestGetAllTickets_AucunTicket()
        {
            context.Database.EnsureCreated(); 

            //Aucun ticket présent dans la BD

            var result = await ticketController.GetAllTickets();

            result.Should().BeOfType<ActionResult<List<Ticket>>>(); 
            var actionResult = result as ActionResult<List<Ticket>>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un NotFoundObjectResult
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull(); 
            notFoundResult.Value.Should().Be("Aucun ticket trouvé."); 
        }

        /// <summary>
        /// Teste la méthode GetAllTickets lorsqu'il y a des tickets dans la base de données.
        /// </summary>
        [TestMethod]
        public async Task TestGetAllTickets_TicketsExistants()
        {
            context.Database.EnsureCreated(); 

            var ticket1 = new Ticket { Id = "TICKET1", TempsArrive = DateTime.Now, EstPaye = false, EstConverti = false };
            var ticket2 = new Ticket { Id = "TICKET2", TempsArrive = DateTime.Now, EstPaye = true, EstConverti = false };
            context.Tickets.Add(ticket1);
            context.Tickets.Add(ticket2);
            await context.SaveChangesAsync();

            var result = await ticketController.GetAllTickets();

            result.Should().BeOfType<ActionResult<List<Ticket>>>(); // Vérifie que le résultat est de type ActionResult<List<Ticket>>
            var actionResult = result as ActionResult<List<Ticket>>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un OkObjectResult
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull(); 

            // Vérifie que la liste des tickets est retournée
            var tickets = okResult.Value as List<Ticket>;
            tickets.Should().NotBeNull();
            tickets.Count.Should().Be(2); // Vérifie qu'il y a 2 tickets dans la liste
        }

        /// <summary>
        /// Teste la méthode GetTicket lorsque le ticket n'existe pas.
        /// </summary>
        [TestMethod]
        public async Task TestGetTicket_TicketNonTrouve()
        {
            context.Database.EnsureCreated(); 

            var result = await ticketController.GetTicket("TICKET123");

            result.Should().BeOfType<ActionResult<Ticket>>(); // Vérifie que le résultat est de type ActionResult<Ticket>
            var actionResult = result as ActionResult<Ticket>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un NotFoundResult
            var notFoundResult = actionResult.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull(); 
        }


        /// <summary>
        /// Teste la méthode GetTicket lorsque le ticket existe.
        /// </summary>
        [TestMethod]
        public async Task TestGetTicket_TicketTrouve()
        {
            context.Database.EnsureCreated(); 

            // Ajoute un ticket à la base de données
            var ticket = new Ticket { Id = "TICKET123", TempsArrive = DateTime.Now, EstPaye = false, EstConverti = false };
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

           
            var result = await ticketController.GetTicket("TICKET123");

           
            result.Should().BeOfType<ActionResult<Ticket>>(); 
            var actionResult = result as ActionResult<Ticket>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le ticket retourné est correct
            var ticketResult = actionResult.Value;
            ticketResult.Should().NotBeNull(); 
            ticketResult.Id.Should().Be("TICKET123"); 
        }


        /// <summary>
        /// Teste la méthode VerifierPaiementTicket lorsque le ticket n'existe pas.
        /// </summary>
        [TestMethod]
        public async Task TestVerifierPaiementTicket_TicketNonTrouve()
        {
            context.Database.EnsureCreated(); 

            var result = await ticketController.VerifierPaiementTicket("TICKET123");

            
            result.Should().BeOfType<ActionResult<TicketEstPayeResponse>>(); 
            var actionResult = result as ActionResult<TicketEstPayeResponse>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un NotFoundObjectResult
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull(); 
            notFoundResult.Value.Should().BeOfType<TicketEstPayeResponse>(); 
            var response = notFoundResult.Value as TicketEstPayeResponse;
            response.Message.Should().Be("❌ Ticket introuvable."); 
        }

        /// <summary>
        /// Teste la méthode VerifierPaiementTicket lorsque le ticket n'a pas été payé.
        /// </summary>
        [TestMethod]
        public async Task TestVerifierPaiementTicket_TicketNonPaye()
        {
            context.Database.EnsureCreated(); 

            //un ticket non payé à la base de données
            var ticket = new Ticket { Id = "TICKET123", TempsArrive = DateTime.Now, EstPaye = false, EstConverti = false };
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var result = await ticketController.VerifierPaiementTicket("TICKET123");

            result.Should().BeOfType<ActionResult<TicketEstPayeResponse>>(); // Vérifie que le résultat est de type ActionResult<TicketEstPayeResponse>
            var actionResult = result as ActionResult<TicketEstPayeResponse>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un OkObjectResult
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull(); 
            okResult.Value.Should().BeOfType<TicketEstPayeResponse>(); 
            var response = okResult.Value as TicketEstPayeResponse;
            response.Message.Should().Be("⚠️ Le ticket existe mais n'a pas encore été payé."); 
            response.EstPaye.Should().BeFalse(); // Vérifie que le ticket n'est pas payé
        }

        /// <summary>
        /// Teste la méthode VerifierPaiementTicket lorsque le ticket a été payé.
        /// </summary>
        [TestMethod]
        public async Task TestVerifierPaiementTicket_TicketPaye()
        {
            context.Database.EnsureCreated(); 

            var ticket = new Ticket { Id = "TICKET123", TempsArrive = DateTime.Now, EstPaye = true, EstConverti = false };
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            
            var result = await ticketController.VerifierPaiementTicket("TICKET123");

           
            result.Should().BeOfType<ActionResult<TicketEstPayeResponse>>(); 
            var actionResult = result as ActionResult<TicketEstPayeResponse>;
            actionResult.Should().NotBeNull(); 

            // Vérifie que le résultat est un OkObjectResult
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeOfType<TicketEstPayeResponse>(); 
            var response = okResult.Value as TicketEstPayeResponse;
            response.Message.Should().Be("✅ Le ticket a déjà été payé."); 
            response.EstPaye.Should().BeTrue(); 
        }
    }
}