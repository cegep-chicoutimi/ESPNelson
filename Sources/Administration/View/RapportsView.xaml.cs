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
using Administration.Resources;
using Administration.ViewModel;

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour RapportsView.xaml
    /// </summary>
    public partial class RapportsView : Page
    {
        public RapportsView()
        {
            InitializeComponent();
            DataContext = new RapportsVM();

            LoadLabels();
        }

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            label_GenerateReport.Content = Resource.GenerateReport;
            label_ExportAsPDF.Content = Resource.ExportAsPDF;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
