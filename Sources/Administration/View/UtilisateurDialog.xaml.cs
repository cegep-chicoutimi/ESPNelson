using Administration.Resources;
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
using System.Windows.Shapes;

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour UtilisateurDialog.xaml
    /// </summary>
    public partial class UtilisateurDialog : Window
    {
        public UtilisateurDialog(UtilisateurDialogVM vm)
        {
            InitializeComponent();
            DataContext = vm;

            // On passe la méthode pour fermer la fenêtre à la VM
            vm.CloseDialogAction = CloseDialogWithResult;

            LoadLabels();
        }

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            label_ChangeYourPassword.Content = Resource.ChangeYourPassword;
            label_Save.Content = Resource.Save;
            label_Cancel.Content = Resource.Cancel;
        }

        private void CloseDialogWithResult(bool result)
        {
            this.DialogResult = result;
            this.Close();
        }

        private void OldPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UtilisateurDialogVM vm)
                vm.OldPassword = ((PasswordBox)sender).SecurePassword;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UtilisateurDialogVM vm)
                vm.NewPassword = ((PasswordBox)sender).SecurePassword;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UtilisateurDialogVM vm)
                vm.ConfirmPassword = ((PasswordBox)sender).SecurePassword;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }


}
