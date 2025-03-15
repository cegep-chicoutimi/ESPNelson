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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            DataContext = new LoginVM();
            LoadLabels();
        }

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            label_Connection.Content = Resource.Connection;
            label_Connection2.Content = Resource.Connection;
            label_ResetPassword.Content = Resource.ResetPassword;

            //Charge aussi les labels bindé depuis la VM
            if (DataContext is LoginVM viewModel)
            {
                viewModel.LoadLabels();
            }

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).MotDePasse = ((PasswordBox)sender).SecurePassword; }
        }

        // Update the Username property each and every time the text changes
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).NomUtilisateur = ((TextBox)sender).Text; }
        }
    }
}
