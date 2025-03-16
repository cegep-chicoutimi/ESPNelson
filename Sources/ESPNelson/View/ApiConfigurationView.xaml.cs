using System.Windows;
using ESPNelson.ViewModel;

namespace ESPNelson.View
{
    public partial class ApiConfigurationView : Window
    {
        private ApiConfigurationViewModel _viewModel;

        BorneEntreeView borneEntreeView { get; set; }

        public ApiConfigurationView()
        {
            InitializeComponent();
            _viewModel = new ApiConfigurationViewModel();
            this.DataContext = _viewModel; // Définir le DataContext

            borneEntreeView = new BorneEntreeView();
        }

        // Gestionnaire d'événements pour le bouton
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer l'URL saisie dans la TextBox
            string apiUrl = ApiUrlTextBox.Text;

            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                // Passer l'URL au ViewModel
                _viewModel.SaveApiUrl(apiUrl);

                //ouvre la brne d'entree'
                borneEntreeView.ShowDialog(); 
            }
            else
            {
                MessageBox.Show("Veuillez saisir une URL valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}