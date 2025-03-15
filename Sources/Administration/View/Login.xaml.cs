using Administration.Resources;
using Administration.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
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
        /// <summary>
        /// Constructeur de la classe
        /// </summary>
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

        /// <summary>
        /// Gère l'événement de changement de texte dans le PasswordBox.
        /// Met à jour la propriété MotDePasse du ViewModel avec le mot de passe saisi.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement (le PasswordBox).</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).MotDePasse = ((PasswordBox)sender).SecurePassword; }
        }



        /// <summary>
        /// Gère l'événement de changement de texte dans la TextBox.
        /// Met à jour la propriété NomUtilisateur du ViewModel avec le texte saisi.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement (la TextBox).</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).NomUtilisateur = ((TextBox)sender).Text; }
        }
    }
}
