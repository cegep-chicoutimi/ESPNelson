using Administration.Model;
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
using Administration.ViewModel;
using Administration.Resources;

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour TarificationDialog.xaml
    /// </summary>
    public partial class TarificationDialog : Window
    {
        public TarificationDialog(Tarification tarification, bool estNouvelle)
        {
            InitializeComponent();
            DataContext = new TarificationDialogVM(tarification, CloseDialogWithResult);
            LoadLabels();
        }

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            label_Price.Content = Resource.Price;
            label_DurationMin.Content = Resource.DurationMin;
            label_DurationMax.Content = Resource.DurationMax;
            label_Save.Content = Resource.Save;
            label_Cancel.Content = Resource.Cancel;
            label_PricingManagement.Title = Resource.PricingManagement; 
        }

        public void CloseDialogWithResult(bool result)
        {
            this.DialogResult = result;
            this.Close();
        }

    }
}
