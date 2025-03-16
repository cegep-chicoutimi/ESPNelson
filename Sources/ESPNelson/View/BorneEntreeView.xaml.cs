using ESPNelson.Resources;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ESPNelson.ViewModel;
using System.Configuration;
using System.ComponentModel;
using ESPNelson.Model;


namespace ESPNelson.View
{
    public partial class BorneEntreeView : Window
    {
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

        private AbonneView abonneView;
        private VisiteurView visiteurView;


        public BorneEntreeView()
        {
            InitializeComponent();

            // Attendre que la fenêtre soit chargée avant d'initialiser le Frame
            this.Loaded += OnWindowLoaded;

            abonneView = new AbonneView();
            abonneView.DataContext = new AbonneVM();

            visiteurView = new VisiteurView();
            visiteurView.DataContext = new VisiteurVM();    


            RessourceHelper.SetInitialLanguage();
            LoadLabels();

            //Direct à l'ouverture de la fenêtre !
            Language = ConfigurationManager.AppSettings["language"];
            SelectLanguage();
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


                  
                    // Mise à jour des textes de la VM de AbonneView
                    if (abonneView.DataContext is AbonneVM viewModel)
                    {
                        viewModel.AbonnementInfo = Resource.ValidTicketSubscription;

                        //voir aussi le dynamisme dans la traduction du message d'ouverture de la barrière

                        viewModel.UpdateTicketInfoLanguage();
                    }

                    // Enregistre la langue sélectionnée dans les paramètres de configuration
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["language"].Value = selectedLanguage;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
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

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        private void LoadLabels()
        {
            label_Language.Content = Resource.Language;
            label_EntryStation.Content = Resource.EntryStation;
            label_Visitor.Content = Resource.Visitor;
            label_Subscriber.Content = Resource.Subscriber;

            //Charge aussi les labels des autres page
            abonneView.LoadLabels();
            visiteurView.LoadLabels();

        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Initialiser le Frame après le chargement de la fenêtre
            MainFrame.Navigate(visiteurView); // Afficher la vue Visiteur par défaut
            StartFadeInAnimation(); // Déclencher l'animation initiale
        }

        private void BtnVisiteur_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la vue "Visiteur" avec animation
            NavigateToPage(visiteurView);
        }

        private void BtnAbonne_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la vue "Abonné" avec animation
            NavigateToPage(abonneView);
        }

        private void NavigateToPage(Page newPage)
        {
            if (newPage != null && MainFrame != null) // Vérifier que MainFrame existe
            {
                MainFrame.Navigate(newPage);
                StartFadeInAnimation(); // Déclencher l'animation après le changement de page
            }
        }

        private void StartFadeInAnimation()
        {
            // Récupérer l'animation depuis les ressources
            Storyboard fadeInAnimation = (Storyboard)FindResource("FadeInAnimation");
            if (fadeInAnimation != null && MainFrame != null) // Vérifier que MainFrame n'est pas null
            {
                fadeInAnimation.Begin(MainFrame); // Déclencher l'animation
            }
        }
    }
}