using Administration.Data;
using Administration.Data.Context;
using Administration.Model;
using Administration.Resources;
using Administration.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Administration.ViewModel
{
    /// <summary>
    /// ViewModel pour la gestion des utilisateurs, tarifications et configurations.
    /// </summary> 
    public partial class GestionVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<Utilisateur> administrateurs = new ObservableCollection<Utilisateur>();

        [ObservableProperty]
        private Utilisateur? utilisateurSelectionne;

        [ObservableProperty]
        private ObservableCollection<Tarification> tarifications = new ObservableCollection<Tarification>();

        [ObservableProperty]
        private Tarification tarificationSelectionnee;

        [ObservableProperty]
        private bool boutonsVisible;

        [ObservableProperty]
        private ObservableCollection<Configuration> configurations = new ObservableCollection<Configuration>();

        [ObservableProperty]
        private Configuration? configurationSelectionnee;

        /// <summary>
        /// Constructeur du ViewModel.
        /// Initialise le contexte de base de données et charge les données initiales.
        /// </summary>
        public GestionVM()
        {
            AdministrationContextFactory factory = new AdministrationContextFactory();
            _dbContext = factory.CreateDbContext(new string[0]);

            ChargerUtilisateurs();
            ChargerTarifications();
            ChargerConfigurations();
        }


        #region utilisateurs

        /// <summary>
        /// Charge la liste des administrateurs depuis la base de données.
        /// </summary>
        private void ChargerUtilisateurs()
        {
            try
            {
                var admins = _dbContext.Utilisateurs
                    .Where(u => u.Role == "admin")
                    .ToList();

                Administrateurs = new ObservableCollection<Utilisateur>(admins);
                BoutonsVisible = false;
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        /// <summary>
        /// Met à jour la visibilité des boutons en fonction de la sélection d'un utilisateur.
        /// </summary>
        partial void OnUtilisateurSelectionneChanged(Utilisateur? value)
        {
            BoutonsVisible = value != null;
        }


        /// <summary>
        /// Affiche un dialogue pour modifier ou ajouter un utilisateur.
        /// </summary>
        /// <param name="utilisateur">L'utilisateur à modifier ou ajouter.</param>
        /// <returns>True si l'utilisateur a confirmé, sinon False.</returns>
        private async Task<bool> AfficherDialogUtilisateur(Utilisateur utilisateur)
        {
            var dialog = new UtilisateurDialog(new UtilisateurDialogVM(utilisateur));
            return dialog.ShowDialog() == true;
        }

        /// <summary>
        /// Ajoute un nouvel utilisateur après avoir affiché un dialogue de saisie.
        /// </summary>
        [RelayCommand]
        private async Task AjouterUtilisateur()
        {
            try
            {
                var nouvelUtilisateur = new Utilisateur { Role = "admin" };  // Par défaut
                await AfficherDialogUtilisateur(nouvelUtilisateur);

                ChargerUtilisateurs();
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Modifie l'utilisateur sélectionné après avoir affiché un dialogue de saisie.
        /// </summary>
        [RelayCommand]
        private async Task ModifierUtilisateur()
        {
            if (UtilisateurSelectionne == null)
            {
                MessageBox.Show(
                    Resource.SelectUser,
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }
            await AfficherDialogUtilisateur(UtilisateurSelectionne);

            ChargerUtilisateurs();
        }

        /// <summary>
        /// Supprime l'utilisateur sélectionné après confirmation.
        /// </summary>
        [RelayCommand]
        private void SupprimerUtilisateur()
        {
            try
            {
                if (UtilisateurSelectionne == null)
                    return;

                if (MessageBox.Show(
                    string.Format(Resource.ConfirmDeletion, UtilisateurSelectionne.NomUtilisateur),
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _dbContext.Utilisateurs.Remove(UtilisateurSelectionne);
                    _dbContext.SaveChanges();
                    ChargerUtilisateurs();
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        #endregion



        #region tarfifications

        /// <summary>
        /// Charge la liste des tarifications depuis la base de données.
        /// </summary>
        private void ChargerTarifications()
        {
            try
            {
                Tarifications = new ObservableCollection<Tarification>(_dbContext.Tarifications.ToList());
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Modifie la tarification sélectionnée après avoir affiché un dialogue de saisie.
        /// </summary>
        [RelayCommand]
        private void ModifierTarification()
        {
            try
            {
                if (TarificationSelectionnee != null)
                {
                    if (AfficherDialogTarification(TarificationSelectionnee, false))
                    {
                        _dbContext.Tarifications.Update(TarificationSelectionnee);
                        _dbContext.SaveChanges();
                        ChargerTarifications();
                    }
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Affiche un dialogue pour modifier ou ajouter une tarification.
        /// </summary>
        /// <param name="tarification">La tarification à modifier ou ajouter.</param>
        /// <param name="estNouvelle">Indique si la tarification est nouvelle.</param>
        /// <returns>True si l'utilisateur a confirmé, sinon False.</returns>
        private bool AfficherDialogTarification(Tarification tarification, bool estNouvelle)
        {
            var dialog = new TarificationDialog(tarification, estNouvelle);
            return dialog.ShowDialog() == true;
        }


        #endregion



        #region configurations

        private void ChargerConfigurations()
        {
            try
            {
                var configs = _dbContext.Configurations
                    .Include(c => c.Utilisateur)
                    .ToList();

                Configurations = new ObservableCollection<Configuration>(configs);
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Affiche un dialogue pour modifier ou ajouter une configuration.
        /// </summary>
        /// <param name="configuration">La configuration à modifier ou ajouter.</param>
        /// <returns>True si l'utilisateur a confirmé, sinon False.</returns>
        private bool AfficherDialogConfiguration(Configuration configuration)
        {
            var vm = new ConfigurationDialogVM
            {
                CapaciteMax = configuration.CapaciteMax,
                TaxeFederal = configuration.TaxeFederal,
                TaxeProvincial = configuration.TaxeProvincial
            };

            var dialog = new ConfigurationDialog(vm);
            if (dialog.ShowDialog() == true)
            {
                // Mettre à jour la configuration avec les valeurs saisies
                configuration.CapaciteMax = vm.CapaciteMax;
                configuration.TaxeFederal = vm.TaxeFederal;
                configuration.TaxeProvincial = vm.TaxeProvincial;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ajoute une nouvelle configuration après avoir affiché un dialogue de saisie.
        /// </summary>
        [RelayCommand]
        private void AjouterConfiguration()
        {
            try
            {
                var nouvelleConfiguration = new Configuration
                {
                    DateModification = DateTime.Now,
                    UtilisateurId = App.Current.User.Id
                };

                if (AfficherDialogConfiguration(nouvelleConfiguration))
                {
                    _dbContext.Configurations.Add(nouvelleConfiguration);
                    _dbContext.SaveChanges();
                    ChargerConfigurations();
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Supprime la configuration sélectionnée après confirmation.
        /// </summary>
        [RelayCommand]
        private void SupprimerConfiguration()
        {
            try
            {
                // Vérifie si aucune configuration n'est sélectionnée
                if (ConfigurationSelectionnee == null)
                {
                    MessageBox.Show(
                        Resource.SelectConfigToDelete,
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                // Vérifie si c'est la seule configuration dans la liste
                if (Configurations.Count == 1)
                {
                    MessageBox.Show(
                        Resource.CannotDeleteLastConfig,
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                // Demande une confirmation avant de supprimer
                if (MessageBox.Show(
                    Resource.ConfirmConfigDeletion,
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _dbContext.Configurations.Remove(ConfigurationSelectionnee);
                    _dbContext.SaveChanges();
                    ChargerConfigurations();
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur
                MessageBox.Show(
                    Resource.ErrorUnexpected + $" : {ex.Message}",
                    Resource.ErrorUnexpected,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        #endregion
        
    }
}

