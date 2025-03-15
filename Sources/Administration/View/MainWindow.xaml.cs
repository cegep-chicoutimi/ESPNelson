using Administration.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Administration.ViewModel;
using System.Windows.Media.Animation;
using Microsoft.EntityFrameworkCore;
using Administration.Resources;
using System.Configuration;
using System.Globalization;
using System.ComponentModel;

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        private Login login;
        private TableauBordView tableauBordView;
        private GestionView gestionView;    
        private RapportsView rapportsView;  

        public NavigationService NavigationService { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            // Attendre que la fenêtre soit chargée avant d'initialiser le Frame
            this.Loaded += OnWindowLoaded;

            login = new Login();
            login.DataContext = new LoginVM();

            tableauBordView = new TableauBordView();
            tableauBordView.DataContext = new TableauBordView();

            gestionView = new GestionView();
            gestionView.DataContext = new GestionView();

            rapportsView = new RapportsView();
            rapportsView.DataContext = new RapportsView();

        }


        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Initialiser le Frame après le chargement de la fenêtre
            AfficherLogin(); // Toujours afficher Login au départ
            StartFadeInAnimation(); // Déclencher l'animation initiale

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

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        private void LoadLabels()
        {
            label_Language.Content = Resource.Language;
            label_Dashboard.Text = Resource.Dashboard;  
            label_Management.Text = Resource.Management;
            label_reports.Text = Resource.reports;

            //Charge aussi les labels des autres page
            login.LoadLabels();
            tableauBordView.LoadLabels();
            gestionView.LoadLabels();
            rapportsView.LoadLabels() ;

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



                    // Mise à jour des textes hint de la VM de login
                    if (login.DataContext is LoginVM viewModel)
                    {
                        viewModel.LoadLabels();
                    }

                    //probablement pour d'autres View plus tard

                    // Enregistre la langue sélectionnée dans les paramètres de configuration
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["language"].Value = selectedLanguage;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }

        public void AfficherLogin()
        {
            MainFrame.Navigate(new Login());
            NavButtonsPanel.Visibility = Visibility.Collapsed;
            btnDeconnexion.Visibility = Visibility.Collapsed;
        }

        public void AfficherTableauBord()
        {
            TableauBordVM tableauBordVM = new TableauBordVM();
            TableauBordView tableauBordPage = new TableauBordView { DataContext = tableauBordVM };

            MainFrame.Navigate(tableauBordPage);
            NavButtonsPanel.Visibility = Visibility.Visible;
            btnDeconnexion.Visibility = Visibility.Visible;
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            App.Current.User = null;
            AfficherLogin();
        }

        private void BtnTableauBord_Click(object sender, RoutedEventArgs e)
        {
            
            NavigateToPage(new TableauBordView());
        }

        private void BtnGestion_Click(object sender, RoutedEventArgs e)
        {
            GestionVM gestionVM = new GestionVM();
            GestionView gestionPage = new GestionView { DataContext = gestionVM };

            NavigateToPage(gestionPage);
        }

        private void BtnRapports_Click(object sender, RoutedEventArgs e)
        {
            RapportsVM rapportsVM = new RapportsVM();
            RapportsView rapportsView = new RapportsView { DataContext = rapportsVM };  

            NavigateToPage(rapportsView);
        }

        public void NavigateToPage(Page newPage)
        {
            if (newPage != null && MainFrame != null) // Vérifier que MainFrame existe
            {
                MainFrame.Navigate(newPage);
                StartFadeInAnimation(); // Déclencher l'animation après le changement de page
            }
        }


        ///// <summary>
        ///// À revoir l'utilité de cette fonction dans ce cadre
        ///// </summary>
        //public void NavigateToLoginPage()
        //{
        //    Page loginPage = new Login();
        //    loginPage.DataContext = new LoginVM();

        //    MainFrame.NavigationService.RemoveBackEntry();
        //    NavigationService.Navigate(loginPage);
        //}

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
