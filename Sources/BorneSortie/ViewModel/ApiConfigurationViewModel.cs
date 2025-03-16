using CommunityToolkit.Mvvm.ComponentModel;
using BorneSortie.Model;
using BorneSortie.View;
using System.Windows;

namespace BorneSortie.ViewModel
{
    public partial class ApiConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _apiUrl; // Ajoutez cette propriété

        // Méthode pour sauvegarder l'URL
        public void SaveApiUrl(string apiUrl)
        {
            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                // Enregistrer l'URL (par exemple, dans un fichier de configuration)
                ConfigurationHelper.SaveApiUrl(apiUrl);

                // Afficher un message de confirmation
                MessageBox.Show("URL sauvegardée : " + apiUrl);

                // Redémarrer l'application ou naviguer vers la vue principale
                var mainWindow = new BorneSortieView();
                mainWindow.Show();
                Application.Current.Windows[0]?.Close(); // Fermer la fenêtre de configuration
            }
            else
            {
                MessageBox.Show("Veuillez saisir une URL valide.");
            }
        }
    }
}