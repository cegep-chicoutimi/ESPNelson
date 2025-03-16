using System.Windows;
using System.Windows.Controls;
using BornePaiement.Resources;

namespace BornePaiement.View
{
    public partial class NumPadPopup : Window
    {
        private const string CorrectPin = "999"; // Le NIP valide

        public string EnteredPin { get; private set; } = "";

        public NumPadPopup()
        {
            InitializeComponent();


            LoadLabels();
        }

        private void NumPad_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && PinBox.Password.Length < 3)
            {
                PinBox.Password += button.Content.ToString();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (PinBox.Password.Length > 0)
            {
                PinBox.Password = PinBox.Password.Substring(0, PinBox.Password.Length - 1);
            }
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (PinBox.Password == CorrectPin)
            {
                EnteredPin = PinBox.Password;
                DialogResult = true; // Ferme la fenêtre avec succès
            }
            else
            {
                MessageBox.Show(
                        Resource.IncorrectPIN,
                        Resource.ErrorTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                PinBox.Password = ""; // Réinitialiser la saisie
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void LoadLabels()
        {
            label_EnterYourPIN.Title = Resource.EnterYourPIN;
            label_EnterYourPIN1.Content = Resource.EnterYourPIN;
            label_Cancel.Content = Resource.Cancel; 
        }
    }
}

