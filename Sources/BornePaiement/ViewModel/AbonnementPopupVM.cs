using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BornePaiement.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using ZXing.Common;
using ZXing;
using BornePaiement.View;
using BornePaiement.Helpers;
using BornePaiement.Resources;
using Microsoft.Win32;

namespace BornePaiement.ViewModel
{
    public partial class AbonnementPopupVM : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string typeAbonnement;
        private DateTime dateDebut;
        private DateTime dateFin;
        private string abonnementId;
        private const string PdfSavePath = "Abonnments";
        private static readonly string LogoPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "img", "logo_ciuss.jpg");
        
        [ObservableProperty]
        private ObservableCollection<string> typesAbonnement = new ObservableCollection<string>();

        [ObservableProperty]
        private bool afficherBoutonTicketAbonnement = false;

        private string _ticketScanne;

        [ObservableProperty] private bool infoAbonnementVisible = true;  
        [ObservableProperty] private bool peutSimuler = true;  
        [ObservableProperty] private bool peutAfficherBoutonGenerer = false;  

        [ObservableProperty]
        private BitmapImage barcodeImage;

        public AbonnementPopupVM(string ticketScanne)
        {
            _ticketScanne = ticketScanne;
            ChargerTypesAbonnement();
        }

        public AbonnementPopupVM()
        {
            // Constructeur sans paramètre requis pour l'inialisation via XAML
            ChargerTypesAbonnement();
        }

        public ICommand ConfirmerCommand => new RelayCommand(Confirmer);
        public ICommand GenererTicketAbonnementCommand => new RelayCommand(GenererTicketAbonnement);

        private void ChargerTypesAbonnement()
        {
            TypesAbonnement.Clear(); // Vide la liste existante avant d'ajouter de nouveaux éléments
            TypesAbonnement.Add("hebdomadaire");
            TypesAbonnement.Add("mensuel");
            OnPropertyChanged(nameof(TypesAbonnement)); // Notifier l'UI d'une mise à jour
        }


        private async void Confirmer()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(TypeAbonnement))
            {
                MessageBox.Show(
                        Resource.EnterEmailAndSelectSubscription,
                        Resource.ErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                );      
                return;
            }

            if(!EmailHelper.IsValidEmail(Email))
            {
                MessageBox.Show(
                          Resource.InvalidEmailFormat,
                          Resource.ErrorTitle,
                          MessageBoxButton.OK,
                          MessageBoxImage.Error
                      );
                return;
            }

            // Simuler le paiement via le NIP
            var numPadPopup = new NumPadPopup();
            bool? result = numPadPopup.ShowDialog();

            if (result == true && numPadPopup.EnteredPin == "999")
            {
                var (success, message, abonnement) = await AbonnementProcessor.SouscrireAbonnementAsync(_ticketScanne, Email, TypeAbonnement);

                if (success)
                {
                    MessageBox.Show(
                            string.Format(Resource.SubscriptionSuccess,
                                abonnement.TypeAbonnement,
                                abonnement.DateDebut,
                                abonnement.DateFin,
                                abonnement.MontantPaye),
                            Resource.SuccessTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );

                    PeutAfficherBoutonGenerer = true;
                    PeutSimuler = false;
                    InfoAbonnementVisible = false;

                    // Stocker les informations pour générer le ticket
                    dateDebut = abonnement.DateDebut;
                    dateFin = abonnement.DateFin;
                    abonnementId = abonnement.AbonnementId;

                    // Générer le code-barres
                    Bitmap barcodeBitmap = GenerateBarcode(abonnementId);
                    if (barcodeBitmap != null)
                    {
                        BarcodeImage = ConvertBitmapToBitmapImage(barcodeBitmap);
                    }
                }
                else
                {
                    MessageBox.Show(
                            string.Format(Resource.SubscriptionError, message),
                            Resource.ErrorTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                }
            }
            else
            {
                
            }
        }

        private Bitmap GenerateBarcode(string text)
        {
            try
            {
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions { Width = 200, Height = 100 }
                };
                var pixelData = writer.Write(text);
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    bitmap.UnlockBits(bitmapData);
                    return new Bitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la génération du code-barres : {ex.Message}");
                return null;
            }
        }


        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(memory.ToArray());
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private void GenererTicketAbonnement()
        {

            // Chemin de sauvegarde du PDF
            string pdfSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TicketsAbonnement");
            if (!Directory.Exists(pdfSavePath))
            {
                Directory.CreateDirectory(pdfSavePath);
            }

            // Nom du fichier PDF
            string pdfFilePath = Path.Combine(PdfSavePath, $"Abonnement_{abonnementId}.pdf");

            // Créer le document PDF
            using (MemoryStream stream = new MemoryStream())
            {  

                PdfDocument document = new PdfDocument();

                // Ajouter une page au document
                PdfPage page = document.AddPage();
                page.Width = XUnit.FromMillimeter(80); // Format ticket (80 mm de largeur)
                page.Height = XUnit.FromMillimeter(150); // Hauteur du ticket

                // Créer un objet XGraphics pour dessiner sur la page
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Définir les polices
                XFont fontTitle = new XFont("Arial", 14);
                XFont fontNormal = new XFont("Arial", 10);
                XFont fontSmall = new XFont("Arial", 8);

                // Dessiner le logo de l'hôpital
                if (File.Exists(LogoPath))
                {
                    XImage logo = XImage.FromFile(LogoPath);
                    gfx.DrawImage(logo, (page.Width.Point - 80) / 2, 20, 80, 80); // Centrer le logo
                }

                // Titre du ticket
                gfx.DrawString("Ticket d'Abonnement", fontTitle, XBrushes.DarkBlue, new XPoint((page.Width.Point - gfx.MeasureString("Ticket d'Abonnement", fontTitle).Width) / 2, 110));

                // Informations de l'hôpital
                gfx.DrawString("Hôpital de Chicoutimi", fontNormal, XBrushes.Black, new XPoint(20, 130));

                // Dessiner le code-barres au mileu du ticket
                using (MemoryStream memory = new MemoryStream())
                {
                    BarcodeImage.StreamSource.Position = 0;
                    BarcodeImage.StreamSource.CopyTo(memory);
                    memory.Position = 0;
                    XImage barcodeXImage = XImage.FromStream(memory);
                    gfx.DrawImage(barcodeXImage, (page.Width.Point - 300) / 2, 140, 300, 100); // Positionner le code-barres
                }


                // Informations de l'abonnement (déplacées après le code-barres)
                gfx.DrawString($"Type d'abonnement: {TypeAbonnement}", fontNormal, XBrushes.Black, new XPoint(20, 250));
                gfx.DrawString($"Date de début: {dateDebut:dd/MM/yyyy}", fontNormal, XBrushes.Black, new XPoint(20, 270));
                gfx.DrawString($"Date de fin: {dateFin:dd/MM/yyyy}", fontNormal, XBrushes.Black, new XPoint(20, 290));

                // Message de remerciement
                gfx.DrawString("Merci pour votre confiance !", fontNormal, XBrushes.DarkGreen, new XPoint((page.Width.Point - gfx.MeasureString("Merci pour votre confiance !", fontNormal).Width) / 2, 330));

                // Sauvegarder le document dans le MemoryStream
                document.Save(stream, false);

                // Proposer à l'utilisateur de sauvegarder le fichier
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Fichiers PDF (*.pdf)|*.pdf", // Filtre pour les fichiers PDF
                    FileName = $"Rapport_{dateDebut:yyyyMMdd}_{dateFin:yyyyMMdd}.pdf", // Nom par défaut du fichier
                    Title = "Enregistrer le rapport PDF" // Titre de la boîte de dialogue
                };

                if (saveFileDialog.ShowDialog() == true) // Si l'utilisateur clique sur "Enregistrer"
                {
                    // Écrire le PDF dans le fichier sélectionné
                    File.WriteAllBytes(saveFileDialog.FileName, stream.ToArray());

                    // Ouvrir le fichier PDF avec le programme par défaut
                    Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                }
            }

        }
    }
}