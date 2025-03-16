using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using BornePaiement.View;
using BornePaiement.Model;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;
using System.Windows;
using BornePaiement.Resources;
using System.Reflection.Metadata;

namespace BornePaiement.ViewModel
{
    public partial class BornePaiementVM : ObservableObject
    {
        [ObservableProperty] private bool ticketValide = false;  
        [ObservableProperty] private bool ticketInvalide = false;
        [ObservableProperty] private string ticketInfo;
        [ObservableProperty] private bool peutSabonner = false;
        [ObservableProperty] private bool peutSimuler = false;

        public string ticketScanne = ""; 

        [ObservableProperty]
        private bool paiementEffectue = false;

        [ObservableProperty]
        private bool afficherBoutonRecu = false;

        private const string PdfSavePath = "Recus";
        private static readonly string LogoPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "img", "logo_ciuss.jpg");

        [ObservableProperty]
        private decimal montantTotal;

        [ObservableProperty]
        private decimal taxes;

        [ObservableProperty]
        private decimal montantAvecTaxes;

        [ObservableProperty]
        private DateTime? tempsArrivee;

        [ObservableProperty]
        private DateTime? tempsSortie;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string typeAbonnement;

        [ObservableProperty]
        private bool afficherBoutonTicketAbonnement;

        private decimal montantTicket = 0;
        private double dureeTicket= 0;
        private string tarificationTicket = "";


        public IRelayCommand ConfirmerPaiementCommand { get; }
        public IRelayCommand GenererRecuCommand { get; }
        public IRelayCommand SouscrireAbonnementCommand { get; }

        public BornePaiementVM()
        {
            ConfirmerPaiementCommand = new RelayCommand(async () => await ConfirmerPaiement());
            GenererRecuCommand = new RelayCommand(GenererRecu);
            SouscrireAbonnementCommand = new RelayCommand(async () => await SouscrireAbonnement());
        }

        public async Task VerifierTicket(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                return;

            // Appeler l'API pour calculer le montant
            var (montant, duree, tarification, tempsArrivee, tempsSortie, dureeDepassee, estPaye, estConverti, messageErreur)
                = await TicketProcessor.CalculerMontantAsync(ticketId);

            // Afficher une MessageBox pour les cas spécifiques
            if (estPaye)
            {
                MessageBox.Show(
                     Resource.TicketAlreadyPaid,
                     "Information",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                );
                return;
            }
            else if (estConverti)
            {
                MessageBox.Show(
                         Resource.TicketAlreadyConverted,
                         "Information",
                         MessageBoxButton.OK,
                         MessageBoxImage.Information
                     );
                return;
            }

            if (!string.IsNullOrEmpty(messageErreur))
            {
                // Cas d'erreur (ticket déjà payé, déjà converti, ou autre erreur)
                TicketInfo = messageErreur;
                TicketInvalide = true;
                TicketValide = false;

                if (dureeDepassee)
                {
                    // Cas de dépassement de durée
                    TicketInfo = Resource.ParkingTimeExceeded;
                    TicketInvalide = true;
                    TicketValide = false;
                }
                else
                {
                    // Cas d'erreur inconnue
                    TicketInfo = Resource.InvalidOrNotFoundTicket;
                    TicketInvalide = true;
                    TicketValide = false;
                }
            }
            else if (montant >= 0)
            {
                // Cas normal : ticket valide
                TicketInfo = $"{Resource.Amount} : {montant:C} $\n {Resource.ArrivalTime}:" +
                    $" {tempsArrivee}\n{Resource.Duration} : {duree}h\n{Resource.Fare} : {tarification}";
                TicketValide = true;
                TicketInvalide = false;
                ticketScanne = ticketId;

                //Rendre visble les deux boutons
                PeutSimuler = true;
                PeutSabonner = true;

                //Pour le changmenet de langue
                montantTicket = montant;
                dureeTicket = duree;
                tarificationTicket = tarification;  

            }

        }


        public void UpdateTicketInfo()
        {
            if (TicketValide)
            {
                // Recalculer le contenu de TicketInfo en fonction de la langue actuelle
                TicketInfo = $"{Resource.Amount} : {montantTicket:C} $\n {Resource.ArrivalTime}:" +
                   $" {tempsArrivee}\n{Resource.Duration} : {dureeTicket}h\n{Resource.Fare} : {tarificationTicket}";
            }
            else if (TicketInvalide)
            {
                // Mettre à jour le message d'erreur en fonction de la langue actuelle
                TicketInfo = Resource.InvalidOrNotFoundTicket;
            }
        }

        private async Task ConfirmerPaiement()
        {
            // Ouvrir la fenêtre NumPad
            var numPadPopup = new NumPadPopup();
            bool? result = numPadPopup.ShowDialog();

            // Vérifier si l'utilisateur a confirmé un NIP
            if (result == true)
            {
                if (numPadPopup.EnteredPin == "999")
                {
                    // Simuler un paiement réussi
                    var (success, message, montantTotal, taxes, montantAvecTaxes, tempsArrivee, tempsSortie) = await TicketProcessor.PayerTicketAsync(ticketScanne);

                    if (success)
                    {
                        TicketInfo = string.Format(
                                    Resource.PaymentSuccess,
                                    montantAvecTaxes,
                                    taxes,
                                    Math.Round((tempsSortie - tempsArrivee).Value.TotalHours, 2)
                                );

                        PaiementEffectue = true;
                        AfficherBoutonRecu = true;

                        PeutSabonner = false; //Le payement étant effectué, il ne doit  pas avoir la possibilité de s'abonner
                        PeutSimuler = false;    //Ne pas lui donner une autre occasion de simuler 

                        // Informations pour le reçu
                        MontantTotal = montantTotal;
                        TempsArrivee = tempsArrivee;
                        TempsSortie = tempsSortie;
                    }
                    else
                    {
                        TicketInfo = string.Format(Resource.PaymentError, message);
                        PaiementEffectue = false;
                        AfficherBoutonRecu = false;

                        PeutSabonner = true;
                        PeutSimuler = true;
                    }
                }
                else
                {
                    MessageBox.Show(
                                Resource.IncorrectPIN,
                                Resource.ErrorTitle,
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                }
            }
        }



        private void GenererRecu()
        {
            if (TempsArrivee == null || TempsSortie == null)
            {
                MessageBox.Show(
                         Resource.NoPaymentInfo,
                         Resource.ErrorTitle,
                         MessageBoxButton.OK,
                         MessageBoxImage.Error
                );
                return;
            }

            if (!Directory.Exists(PdfSavePath))
                Directory.CreateDirectory(PdfSavePath);

            string pdfFilePath = Path.Combine(PdfSavePath, $"Recu_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromMillimeter(80); // Format ticket de stationnement
                page.Height = XUnit.FromMillimeter(150);
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont fontTitle = new XFont("Arial", 12);
                XFont fontNormal = new XFont("Arial", 10);

                // Dessiner le logo de l'hôpital
                if (File.Exists(LogoPath))
                {
                    XImage logo = XImage.FromFile(LogoPath);
                    gfx.DrawImage(logo, (page.Width.Point - 100) / 2, 20, 100, 100); // Ajustez la taille et la position du logo
                }

                // Titre du reçu
                gfx.DrawString("Reçu de Paiement", fontTitle, XBrushes.DarkBlue, new XPoint((page.Width.Point - gfx.MeasureString("Reçu de Paiement", fontTitle).Width) / 2, 130));

                // Informations du ticket
                gfx.DrawString($"Heure d'arrivée: {TempsArrivee:dd/MM/yyyy HH:mm:ss}", fontNormal, XBrushes.Black, new XPoint(20, 150));
                gfx.DrawString($"Heure de sortie: {TempsSortie:dd/MM/yyyy HH:mm:ss}", fontNormal, XBrushes.Black, new XPoint(20, 170));
                gfx.DrawString($"Durée du séjour: {Math.Round((TempsSortie - TempsArrivee).Value.TotalHours, 2)} heures", fontNormal, XBrushes.Black, new XPoint(20, 190));

                // Montant et taxes
                gfx.DrawString($"Montant total: {MontantTotal:C}", fontNormal, XBrushes.Black, new XPoint(20, 210));

                // Calculer les pourcentages des taxes uniquement si MontantTotal n'est pas zéro
                string taxesInfo;
                if (MontantTotal != 0)
                {
                    taxesInfo = $"Taxes (fédéral: {Taxes / MontantTotal * 100}%, provincial: {Taxes / MontantTotal * 100}%): {Taxes:C}";
                }
                else
                {
                    taxesInfo = "Taxes non applicables (montant total nul).";
                }

                // Dessiner les informations sur les taxes
                gfx.DrawString(taxesInfo, fontNormal, XBrushes.Black, new XPoint(20, 230));

                gfx.DrawString($"Montant avec taxes: {MontantAvecTaxes:C}", fontNormal, XBrushes.Black, new XPoint(20, 250));

                // Message de remerciement
                gfx.DrawString("Merci pour votre visite !", fontNormal, XBrushes.DarkGreen, new XPoint((page.Width.Point - gfx.MeasureString("Merci pour votre visite !", fontNormal).Width) / 2, 280));

                document.Save(pdfFilePath);
            }

            // Ouvrir le PDF généré
            Process.Start(new ProcessStartInfo(pdfFilePath) { UseShellExecute = true });
            MessageBox.Show(
                     Resource.ReceiptGenerated,
                     "Information",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                 );
        }

        private async Task SouscrireAbonnement()
        {
            if (string.IsNullOrEmpty(ticketScanne))
            {
                MessageBox.Show(
                     Resource.ScanTicketFirst,
                     Resource.ErrorTitle,
                     MessageBoxButton.OK,
                     MessageBoxImage.Error
                 );
                return;
            }

            // Créer et afficher la fenêtre contextuelle
            var popupVM = new AbonnementPopupVM(ticketScanne);
            var popup = new AbonnementPopup(ticketScanne)
            {
                DataContext = popupVM
            };

            popup.ShowDialog();
        }


    }
}


