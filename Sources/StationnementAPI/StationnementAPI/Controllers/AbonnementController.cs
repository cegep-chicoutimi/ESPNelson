using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using StationnementAPI.Models.ModelsDTO;
using System;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace StationnementAPI.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les opérations liées aux abonnements de stationnement.
    /// </summary>
    [Route("api/abonnements")]
    [ApiController]
    public class AbonnementController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AbonnementController"/>.
        /// </summary>
        /// <param name="context">Le contexte de la base de données.</param>
        public AbonnementController(StationnementDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Souscrit un nouvel abonnement en utilisant les informations fournies dans le DTO.
        /// </summary>
        /// <param name="paiementDto">Les informations de paiement, y compris l'ID du ticket et l'email de l'utilisateur.</param>
        /// <returns>Un résultat HTTP indiquant le succès ou l'échec de la souscription.</returns>
        [HttpPost("souscrire")]
        public async Task<ActionResult> SouscrireAbonnement([FromBody] PaiementDto paiementDto)
        {
            if (string.IsNullOrEmpty(paiementDto.TicketId))
                return BadRequest("TicketId est requis pour souscrire un abonnement.");

            if (string.IsNullOrEmpty(paiementDto.Email))
                return BadRequest("L'email est requis pour enregistrer un nouvel abonné.");

            var ticket = await _context.Tickets.FindAsync(paiementDto.TicketId);
            if (ticket == null)
                return NotFound("Ticket non trouvé.");
            if (ticket.EstConverti)
                return Conflict("Ce ticket a déjà été utilisé pour un abonnement.");

            var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == paiementDto.Email);
            if (existingUser != null)
                return Conflict("Cet email est déjà associé à un abonné.");

            var utilisateur = new Utilisateur
            {
                NomUtilisateur = paiementDto.Email.Split('@')[0],
                MotDePasse = "MotDePasseTemporaire123!",
                Email = paiementDto.Email,
                Role = "abonne"
            };
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            // Déterminer la durée et le montant en fonction du type d'abonnement
            int dureeJours = paiementDto.TypeAbonnement.ToLower() == "hebdomadaire" ? 7 : 30;

            var abonnement = new Abonnement
            {
                Id = GenerateAbonnmentId(),
                UtilisateurId = utilisateur.Id,
                DateDebut = DateTime.UtcNow,
                DateFin = DateTime.UtcNow.AddDays(dureeJours),
                Type = paiementDto.TypeAbonnement.ToLower()
            };
            _context.Abonnements.Add(abonnement);
            await _context.SaveChangesAsync();

            decimal montant = paiementDto.TypeAbonnement.ToLower() == "mensuel" ? 50 : 15;

            var paiement = paiementDto.DtoToPaiement(montant, abonnement.Id);
            ticket.EstConverti = true;


            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            try
            {
                await _context.SaveChangesAsync();  
            }
            catch (DbUpdateException ex)
            {
                // Log l'exception interne pour plus de détails
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
                return StatusCode(500, "Une erreur s'est produite lors de l'enregistrement en base de données.");
            }

            return Created($"api/abonnements/{abonnement.Id}", new
            {
                Message = "Abonnement souscrit avec succès.",
                AbonnementId = abonnement.Id,
                TypeAbonnement = abonnement.Type,
                DateDebut = abonnement.DateDebut,
                DateFin = abonnement.DateFin,
                MontantPaye = paiement.Montant
            });
        }


        /// <summary>
        /// Récupère les détails d'un abonnement actif en utilisant son ID.
        /// </summary>
        /// <param name="id">L'ID de l'abonnement à récupérer.</param>
        /// <returns>Un objet contenant les détails de l'abonnement ou un message d'erreur si l'abonnement n'est pas actif.</returns>
        [HttpGet("actifs/{id}")]
        public async Task<ActionResult<object>> GetAbonnement(string id)
        {
            var abonnement = await _context.Abonnements.FindAsync(id);
            if (abonnement == null)
                return NotFound("Aucun abonnement existant pour ce ticket !");

            if (abonnement.DateFin < DateTime.Now)
                return BadRequest("Cet abonnement n'est plus actif rendu à cette date");

            return Ok(new
            {
                Message = "",
                AbonnementId = abonnement.Id,
                TypeAbonnement = abonnement.Type,
                DateDebut = abonnement.DateDebut,
                DateFin = abonnement.DateFin,
                MontantPaye = 0
            });
        }

        /// <summary>
        /// Génère un ID unique pour un abonnement.
        /// </summary>
        /// <returns>Un ID d'abonnement sous forme de chaîne de caractères.</returns>
        private string GenerateAbonnmentId()
        {
            string guid = Guid.NewGuid().ToString("N").ToUpper(); // Supprime les tirets et met en majuscule
            return guid.Substring(0, 6); // Prend les 10 premiers caractères
        }

    }
}
