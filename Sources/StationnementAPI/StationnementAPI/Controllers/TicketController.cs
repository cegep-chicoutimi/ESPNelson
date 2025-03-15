using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;

namespace StationnementAPI.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les opérations liées aux tickets de stationnement.
    /// </summary>
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="TicketController"/>.
        /// </summary>
        /// <param name="context">Le contexte de la base de données.</param>
        public TicketController(StationnementDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Génère un ID unique pour un ticket.
        /// </summary>
        /// <returns>Un ID de ticket sous forme de chaîne de caractères.</returns>
        private string GenerateTicketId()
        {
            string guid = Guid.NewGuid().ToString("N").ToUpper(); // Supprime les tirets et met en majuscule
            return guid.Substring(0, 12); // Prend les 12 premiers caractères
        }

        /// <summary>
        /// Génère un nouveau ticket avec un ID unique et l'heure d'arrivée actuelle.
        /// </summary>
        /// <returns>Un résultat HTTP 201 (Created) avec les détails du ticket généré.</returns>
        [HttpPost("generer")]
        public async Task<ActionResult<Ticket>> GenererTicket()
        {
            // Conversion UTC vers fuseau horaire local
            var heureLocale = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            var ticket = new Ticket
            {
                Id = GenerateTicketId(),
                TempsArrive = heureLocale
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }


        /// <summary>
        /// Récupère tous les tickets disponibles dans la base de données.
        /// </summary>
        /// <returns>Une liste de tickets ou un message d'erreur si aucun ticket n'est trouvé.</returns>
        [HttpGet]
        public async Task<ActionResult<List<Ticket>>> GetAllTickets()
        {
            try
            {
                //Récupération de tous les tickets avec leurs informations associées
                var lesTickets = await _context.Tickets.ToListAsync();

                if (lesTickets == null || lesTickets.Count == 0)
                {
                    return NotFound("Aucun ticket trouvé.");
                }

                return Ok(lesTickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }


        /// <summary>
        /// Pour récupérer un ticket via son ID
        /// </summary>
        /// <param name="id">Id du ticket</param>
        /// <returns>un objet de type ticket</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            return ticket;
        }


        /// <summary>
        /// Vérifie si un ticket a été payé et retourne un statut détaillé.
        /// </summary>
        /// <param name="id">L'ID du ticket</param>
        /// <returns>un objet qui détail les infos de payement du ticket</returns>
        [HttpGet("{id}/verifier-paiement")]
        public async Task<ActionResult<TicketEstPayeResponse>> VerifierPaiementTicket(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new TicketEstPayeResponse
                {
                    Message = "⛔ L'ID du ticket est invalide."
                });
            }

            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound(new TicketEstPayeResponse
                {
                    TicketId = id,
                    Message = "❌ Ticket introuvable."
                });
            }

            // Construire la réponse détaillée
            var response = new TicketEstPayeResponse
            {
                TicketId = ticket.Id,
                EstPaye = ticket.EstPaye,
                EstConverti = ticket.EstConverti,
                Message = ticket.EstPaye ? "✅ Le ticket a déjà été payé." : "⚠️ Le ticket existe mais n'a pas encore été payé.",
            };

            return Ok(response);
        }
    }


}
