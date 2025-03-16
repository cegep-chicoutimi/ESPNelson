using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BornePaiement.ViewModel;
using System.Windows.Media.Animation;
using BornePaiement.View;
using BornePaiement.Resources;
using System.Globalization;
using System.ComponentModel;
using System.Configuration;

namespace BornePaiement.View
{
    /// <summary>
    /// Logique d'interaction pour BornePaiementView.xaml
    /// </summary>
    public partial class BornePaiementView : Window
    {
        private StringBuilder _scanBuffer = new StringBuilder(); // Buffeur pour collecter les données du scan

        /// <summary>
        /// Langue actuelle de l'application.
        /// </summary>
        private string _language;

        /// <summary>
        /// Obtient ou définit la langue de l'application; Met à jour les ressources linguistiques et recharge les labels.
        /// </summary>
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged(nameof(Language));
                    Resource.Culture = new CultureInfo(value);
                    LoadLabels(); //à chaque chargement de langue on charge les labels directement

                }
            }
        }

        NumPadPopup numPadPopup;
        AbonnementPopup abonnementPopup;

        public BornePaiementView()
        {
            InitializeComponent();
            this.DataContext = new BornePaiementVM();

            numPadPopup = new NumPadPopup();

            if(this.DataContext is BornePaiementVM vm)
            {
                abonnementPopup = new AbonnementPopup(vm.ticketScanne);
            }
           

            //Direct à l'ouverture de la fenêtre !
            RessourceHelper.SetInitialLanguage();

            Language = ConfigurationManager.AppSettings["language"];
            SelectLanguage();

            //C'est apres tout ceci qu'on Load les labels
            LoadLabels();
        }

        private void HiddenScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Transmettre l'événement clavier à la méthode principale
            Page_KeyDown(sender, e);
        }

        private async void Page_KeyDown(object sender, KeyEventArgs e)
        {
            // Ignorer les touches spéciales
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.CapsLock || e.Key == Key.Tab ||
                e.Key == Key.Escape || e.Key == Key.Back)
            {
                return;
            }

            if (e.Key == Key.Enter) // 🎯 Lorsque l'utilisateur a scanné son ticket
            {
                await Task.Delay(100); // Délai de 100 ms pour s'assurer que le scan est complet
                // Transmettre l'ID du ticket au ViewModel
                if (DataContext is BornePaiementVM viewModel)
                {
                    viewModel.VerifierTicket(_scanBuffer.ToString());
                }
                _scanBuffer.Clear(); // Réinitialiser le buffeur après le traitement
            }
            else
            {
                // Capturer les chiffres (0-9)
                if (e.Key >= Key.D0 && e.Key <= Key.D9) // Chiffres de 0 à 9
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("D", "")); // Supprimer le préfixe "D" pour les chiffres
                }
                // Capturer les chiffres du pavé numérique (0-9)
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) // Chiffres du pavé numérique
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("NumPad", "")); // Supprimer le préfixe "NumPad"
                }
                // Capturer les lettres (A-Z)
                else if (e.Key >= Key.A && e.Key <= Key.Z) // Lettres de A à Z
                {
                    _scanBuffer.Append(e.Key.ToString()); // Conserver la lettre telle quelle
                }
            }
        }


        /// <summary>
        ///Sélectionne automatiquement la langue configurée dans `App.Config` au démarrage de l'application.
        /// </summary>
        private void SelectLanguage()
        {
            string lang = ConfigurationManager.AppSettings["language"];

            foreach (ComboBoxItem item in languageComboBox.Items)
            {
                if (item.Tag is string tag && tag == lang)
                {
                    languageComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        private void LoadLabels()
        {
            label_PayementStation.Content = Resource.PayementStation;
            label_PayementStation1.Title = Resource.PayementStation;
            label_Language.Content = Resource.Language;
            label_ScanningInstructions.Text = Resource.ScanningInstructions;  
            label_SimulatePayment.Content = Resource.SimulatePayment;
            label_Subscribe.Content = Resource.Subscribe;
            label_GenerateReceipt.Content = Resource.GenerateReceipt; 
            label_ValidTicket.Content = Resource.ValidTicket;
            label_InValidTicket.Content = Resource.InvalidTicket;

            //Charge aussi les labels des autres pages
            numPadPopup.LoadLabels();
            abonnementPopup.loadLabels();



        }

        /// <summary>
        /// Événement permettant de notifier un changement de propriété.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifie un changement de propriété pour la mise à jour de l'interface.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //Le code de la fonction suivante n'est pas de moi à 100%...source: ChatGPT
        /// <summary>
        ///cette fonction met à jour la langue de l'application en fonction de la sélection de l'utilisateur dans le ComboBox
        /// </summary>
        /// <param name="sender">Objet source de l'événement</param>
        /// <param name="e">Arguments de l'événement</param>
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Tag is string selectedLanguage)
                {
                    // Affectation propre sans provoquer d'exception
                    _language = selectedLanguage;
                    OnPropertyChanged(nameof(Language));

                    // Mise à jour des ressources linguistiques
                    Resource.Culture = new CultureInfo(selectedLanguage);
                    LoadLabels();

                    if(this.DataContext is BornePaiementVM vM)
                    {
                        vM.UpdateTicketInfo();
                    }


                    // Enregistre la langue sélectionnée dans les paramètres de configuration
                    System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["language"].Value = selectedLanguage;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }
    }
}
