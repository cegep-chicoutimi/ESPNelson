using Administration.Data.Context;
using Administration.Data;
using Administration.Helpers;
using Administration.Model;
using Administration.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Administration.Resources;

namespace Administration.ViewModel
{
    public partial class LoginVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        [ObservableProperty]
        private string? _nomUtilisateur;

        /// <summary>
        /// Mot de passe
        /// </summary>
        [ObservableProperty]
        private SecureString _motDePasse;

        // Propriétés pour les textes dynamiques
        [ObservableProperty]
        private string? _nomUtilisateurHint;

        [ObservableProperty]
        private string? _motDePasseHint;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        public LoginVM()
        {
            // Initialize the database context
            AdministrationContextFactory factory = new AdministrationContextFactory();
            _dbContext = factory.CreateDbContext(new string[0]);

            // Charge les labels avec la langue sélectionnée
            LoadLabels();
        }

        /// <summary>
        /// Charge les labels en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            NomUtilisateurHint = Resource.Username; // "Nom d'utilisateur"
            MotDePasseHint = Resource.Password;   // "Mot de passe"
        }


        /// <summary>
        /// Commande de connexion
        /// </summary>
        [RelayCommand]
        public void Login()
        {
            try
            {
                if (MotDePasse == null || string.IsNullOrWhiteSpace(NomUtilisateur))
                {
                    MessageBox.Show(
                        Resource.MissingCredentials,
                        Resource.MissingCredentialsTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                Utilisateur utilisateur = _dbContext.Utilisateurs.FirstOrDefault(u => u.NomUtilisateur == NomUtilisateur);
                //Check if the username and password are correct
                if (utilisateur == null || !CryptographyHelper.ValidateHashedPassword(ConvertHelper.SecureStringToString(MotDePasse), utilisateur.MotDePasse))
                {
                    MessageBox.Show(
                        Resource.MissingCredentials,
                        Resource.MissingCredentialsTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                //Vérifie s'il s'agit d'un admin
                if (utilisateur.Role != "admin")
                {
                    MessageBox.Show(
                         Resource.AdminRequired,
                         Resource.AdminRequiredTitle,
                         MessageBoxButton.OK,
                         MessageBoxImage.Information
                    );
                    return;
                }

                //après un Login réussi
                App.Current.User = utilisateur; //Stocke les informations de l'utilisateur connecté

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.AfficherTableauBord();
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur et masque le diagramme
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Commande qui permet de réinitialiser le mot de passe via un courriel
        /// </summary>
        [RelayCommand]
        public async Task ForgotPassword()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(NomUtilisateur))
                {
                    MessageBox.Show(
                        Resource.EnterUsername,
                        Resource.EnterUsernameTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }
                Utilisateur? utilisateur = _dbContext.Utilisateurs.FirstOrDefault(e => e.NomUtilisateur == NomUtilisateur);

                if (utilisateur == null)
                {
                    MessageBox.Show(
                         Resource.NoEmailAssociated,
                         Resource.InvalidInfoTitle,
                         MessageBoxButton.OK,
                         MessageBoxImage.Information
                     );

                    return;
                }

                bool result = await PasswordHelper.ResetPassword(utilisateur.Email, utilisateur, _dbContext);
                if (!result)
                {
                    return;
                }

                MessageBox.Show(
                        Resource.EmailSent,
                        Resource.EmailSentTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                 );
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur et masque le diagramme
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
