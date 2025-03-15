using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationnementAPI.Data.Context;
using StationnementAPI.Models;
using StationnementAPI.Models.ModelsDTO;
using System;
using System.Threading.Tasks;

namespace StationnementAPI.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les opérations liées aux paiements des tickets de stationnement.
    /// </summary>
    [Route("api/paiements")]
    [ApiController]
    public class PaiementController : ControllerBase
    {
        private readonly StationnementDbContext _context;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PaiementController"/>.
        /// </summary>
        /// <param name="context">Le contexte de la base de données.</param>
        public PaiementController(StationnementDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Calcule le montant du paiement pour un ticket donné sans effectuer le paiement.
        /// </summary>
        /// <param name="ticketId">L'ID du ticket pour lequel calculer le montant.</param>
        /// <returns>Un objet contenant les détails du calcul du montant, y compris les taxes et la durée de stationnement.</returns>
        [HttpGet("calculer-montant/{ticketId}")]
        public async Task<ActionResult<object>> CalculerMontantTicket(string ticketId)
        {
            // Recherche le ticket dans la base de données
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound(new
                {
                    Message = "Ticket non trouvé",
                    TicketId = ticketId
                });
            }


            //la durée de stationnement
            var tempsSortie = DateTime.Now; 
            var dureeStationnement = (tempsSortie - ticket.TempsArrive).TotalHours;

            // Cas spécial : dépassement des 24h
            if (dureeStationnement > 24)
            {
                return StatusCode(403, new
                {
                    Message = "⛔ La durée de stationnement dépasse les 24h autorisées. Veuillez contacter l'administration.",
                    DureeStationnement = Math.Round(dureeStationnement, 2),
                    TempsArrive = ticket.TempsArrive,
                    TempsSortie = tempsSortie
                });
            }

            // Vérifie la tarification applicable
            var tarification = await _context.Tarifications
                .FirstOrDefaultAsync(t => dureeStationnement >= t.DureeMin && dureeStationnement <= t.DureeMax);

            if (tarification == null)
            {
                return BadRequest(new
                {
                    Message = "Aucune tarification trouvée pour la durée du stationnement.",
                    DureeStationnement = Math.Round(dureeStationnement, 2)
                });
            }


            //les taxes les plus récentes
            var configuration = await _context.Configurations
                .OrderByDescending(c => c.DateModification)
                .FirstOrDefaultAsync();

            if (configuration == null)
            {
                return BadRequest(new
                {
                    Message = "Aucune configuration trouvée pour les taxes."
                });
            }

            // Calcule le montant total avec taxes
            decimal montantTotal = tarification.Prix;
            decimal taxes = montantTotal * (configuration.TaxeFederal + configuration.TaxeProvincial) / 100;
            decimal montantAvecTaxes = Math.Round(montantTotal + taxes, 2, MidpointRounding.AwayFromZero); 

            // les informations de calcul du montant
            return Ok(new
            {
                Montant = montantTotal,
                Taxes = taxes,
                MontantAvecTaxes = montantAvecTaxes,
                DureeStationnement = Math.Round(dureeStationnement, 2),
                TarificationAppliquee = tarification.Niveau,
                TarificationPrix = tarification.Prix,
                TarificationDureeMin = tarification.DureeMin,
                TarificationDureeMax = tarification.DureeMax,
                TempsArrivee = ticket.TempsArrive,
                TempsSortie = tempsSortie,
                EstPaye = ticket.EstPaye,
                EstConverti = ticket.EstConverti,
                TaxeFederal = configuration.TaxeFederal,
                TaxeProvincial = configuration.TaxeProvincial
            });
        }



        /// <summary>
        /// Effectue le paiement d'un ticket en utilisant les informations fournies dans le DTO.
        /// </summary>
        /// <param name="paiementDto">Les informations de paiement, y compris l'ID du ticket.</param>
        /// <returns>Un résultat HTTP indiquant le succès ou l'échec du paiement.</returns>
        [HttpPost("payer-ticket")]
        public async Task<ActionResult> PayerTicket([FromBody] PaiementDto paiementDto)
        {
            if (string.IsNullOrEmpty(paiementDto.TicketId))
                return BadRequest("TicketId est requis pour un paiement de ticket.");

            var ticket = await _context.Tickets.FindAsync(paiementDto.TicketId);
            if (ticket == null)
                return NotFound("Ticket non trouvé");

            if (ticket.EstPaye)
                return BadRequest("Ce ticket a déjà été payé.");

            if (ticket.EstConverti)
                return BadRequest("Ce ticket a déjà été converti en abonnement.");

            
            var montantResponse = await CalculerMontantTicket(ticket.Id);
            if (montantResponse.Result is BadRequestObjectResult || montantResponse.Result is NotFoundObjectResult)
                return montantResponse.Result; // Retourne l'erreur appropriée


            var montantResult = ((OkObjectResult)montantResponse.Result).Value;
            decimal montantTotal = (decimal)montantResult.GetType().GetProperty("Montant")?.GetValue(montantResult);
            decimal taxes = (decimal)montantResult.GetType().GetProperty("Taxes")?.GetValue(montantResult);
            decimal montantAvecTaxes = (decimal)montantResult.GetType().GetProperty("MontantAvecTaxes")?.GetValue(montantResult);

            string tarificationNiveau = (string)montantResult.GetType().GetProperty("TarificationAppliquee")?.GetValue(montantResult);
            decimal tarificationPrix = (decimal)montantResult.GetType().GetProperty("TarificationPrix")?.GetValue(montantResult);
            int tarificationDureeMin = (int)montantResult.GetType().GetProperty("TarificationDureeMin")?.GetValue(montantResult);
            int tarificationDureeMax = (int)montantResult.GetType().GetProperty("TarificationDureeMax")?.GetValue(montantResult);

            // Mets  jour le statut du ticket et enregistrer le paiement
            ticket.EstPaye = true;
            ticket.TempsSortie = DateTime.UtcNow;

            var paiement = new Paiement
            {
                TicketId = ticket.Id,
                Montant = montantAvecTaxes,
                DatePaiement = ticket.TempsSortie.Value,
                TarificationNiveau = tarificationNiveau,
                TarificationPrix = tarificationPrix,
                TarificationDureeMin = tarificationDureeMin,
                TarificationDureeMax = tarificationDureeMax
            };

            _context.Paiements.Add(paiement);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Paiement du ticket effectué. Montant : {montantAvecTaxes}$",
                MontantTotal = montantTotal,
                Taxes = taxes,
                MontantAvecTaxes = montantAvecTaxes,
                TempsArrivee = ticket.TempsArrive,
                TempsSortie = ticket.TempsSortie,
                TarificationAppliquee = tarificationNiveau
            });
        }
    }
}
