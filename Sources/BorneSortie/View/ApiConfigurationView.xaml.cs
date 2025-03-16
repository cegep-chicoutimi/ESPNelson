using System.Windows;
using BorneSortie.ViewModel;

namespace BorneSortie.View
{
    public partial class ApiConfigurationView : Window
    {
        private ApiConfigurationViewModel _viewModel;

        BorneSortieView borneSortieView { get; set; }

        public ApiConfigurationView()
        {
            InitializeComponent();
            _viewModel = new ApiConfigurationViewModel();
            this.DataContext = _viewModel; // Définir le DataContext

            borneSortieView = new BorneSortieView();
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
                borneSortieView.ShowDialog(); 
            }
            else
            {
                MessageBox.Show("Veuillez saisir une URL valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}