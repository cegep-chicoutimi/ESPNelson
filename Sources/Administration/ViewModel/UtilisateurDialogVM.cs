using Administration.Data;
using Administration.Data.Context;
using Administration.Helpers;
using Administration.Model;
using Administration.Resources;
using Administration.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Security;
using System.Windows;

namespace Administration.ViewModel
{
    public partial class UtilisateurDialogVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        public Action<bool>? CloseDialogAction { get; set; }  // <- Action injectée depuis la vue


        [ObservableProperty]
        private Utilisateur utilisateur;

        [ObservableProperty]
        private bool peutModifierSonMotDePasse;

        [ObservableProperty]
        private bool afficherChangementMotDePasse;

        public bool EstNouveau { get; }
        public SecureString? OldPassword { get; set; }
        public SecureString? NewPassword { get; set; }
        public SecureString? ConfirmPassword { get; set; }

        public string TitreDialog;

        // Propriétés pour les textes dynamiques
        [ObservableProperty]
        private string? _nomUtilisateurHint;

        [ObservableProperty]
        private string? _emailHint;

        [ObservableProperty]
        private string? _oldPasswordHint;

        [ObservableProperty]
        private string? _newPasswordHint;
        [ObservableProperty]
        private string? _confirmPasswordHint;


        public string TexteBouton => EstNouveau ? Resource.Add : Resource.Edit;

        public UtilisateurDialogVM(Utilisateur utilisateur)
        {
            _dbContext = new AdministrationContextFactory().CreateDbContext(new string[0]);
            Utilisateur = utilisateur;
            EstNouveau = utilisateur.Id == 0;
            PeutModifierSonMotDePasse = !EstNouveau && App.Current.User?.Id == utilisateur.Id;

            // Charge les labels avec la langue sélectionnée
            LoadLabels();
        }

        /// <summary>
        /// Charge les labels en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            TitreDialog = EstNouveau ? Resource.AddAdministrator : Resource.ModifyAdministrator;
            TitreDialog = EstNouveau ? Resource.Add : Resource.Edit;

            NomUtilisateurHint = Resource.Username;
            EmailHint = Resource.Email;

            OldPasswordHint = Resource.OldPassword;
            NewPasswordHint = Resource.NewPassword;
            ConfirmPasswordHint = Resource.ConfirmPassword; 

        }

        [RelayCommand]
        private void ToggleMotDePasse() => AfficherChangementMotDePasse = !AfficherChangementMotDePasse;

        [RelayCommand]
        private void Enregistrer()
        {
            if(Utilisateur.Email == null || !EmailHelper.IsValidEmail(Utilisateur.Email))
            {
                MessageBox.Show(
                     Resource.InvalidEmail,
                     Resource.ErrorTitle,
                     MessageBoxButton.OK,
                     MessageBoxImage.Error
                );
                return;
            }

            // Vérifier l'unicité de l'email (autre que lui-même s'il s'agit d'une modification)
            bool emailDejaPris = _dbContext.Utilisateurs
                .Any(u => u.Email == Utilisateur.Email);

            if (emailDejaPris)
            {
                MessageBox.Show(
                     Resource.EmailAlreadyUsed,
                     Resource.ErrorTitle,
                     MessageBoxButton.OK,
                     MessageBoxImage.Warning
                 );
                return;
            }

            // Vérifier l'unicité de username (autre que lui-même s'il s'agit d'une modification)
            bool usernameDejaPris = _dbContext.Utilisateurs
                .Any(u => u.NomUtilisateur == Utilisateur.NomUtilisateur);

            if (usernameDejaPris)
            {
                MessageBox.Show(
                     Resource.UsernameAlreadyUsed,
                     Resource.ErrorTitle,
                     MessageBoxButton.OK,
                     MessageBoxImage.Warning
                 );
                return;
            }

            if (EstNouveau)
            {
               //On lui donne un mot de passe par défaut, il sera invité à le changer plus tard
                Utilisateur.MotDePasse = CryptographyHelper.HashPassword("admin");
                _dbContext.Utilisateurs.Add(Utilisateur);

                MessageBox.Show(
                     Resource.UserCreatedWithTempPassword,
                     "Information",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                 );
            }
            else
            {
                if (AfficherChangementMotDePasse)
                {
                    var oldPassword = ConvertHelper.SecureStringToString(OldPassword);
                    var newPassword = ConvertHelper.SecureStringToString(NewPassword);
                    var confirmPassword = ConvertHelper.SecureStringToString(ConfirmPassword);

                    if (!CryptographyHelper.ValidateHashedPassword(oldPassword, Utilisateur.MotDePasse))
                    {
                        MessageBox.Show(
                        Resource.IncorrectOldPassword,
                        Resource.ErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                        return;
                    }

                    if (newPassword != confirmPassword)
                    {
                        MessageBox.Show(
                            Resource.PasswordsDoNotMatch,
                            Resource.ErrorTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }

                    Utilisateur.MotDePasse = CryptographyHelper.HashPassword(newPassword);
                }
                _dbContext.Utilisateurs.Update(Utilisateur);
            }

            _dbContext.SaveChanges();

            // Appeler l'action passée depuis la fenêtre
            CloseDialogAction?.Invoke(true);

            
        }

        [RelayCommand]
        private void Annuler()
        {
            CloseDialogAction?.Invoke(false);
        }


    }

}
