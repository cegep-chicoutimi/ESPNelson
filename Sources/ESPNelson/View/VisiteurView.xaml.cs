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
using ESPNelson.Resources;
using ESPNelson.ViewModel;

namespace ESPNelson.View
{
    /// <summary>
    /// Logique d'interaction pour VisiteurView.xaml
    /// </summary>
    public partial class VisiteurView : Page
    {
        public VisiteurView()
        {
            InitializeComponent();

            LoadLabels();

        }

        public void LoadLabels()
        {
            label_VisitorTicketGeneration.Content = Resource.VisitorTicketGeneration;
            label_GenerateTicket.Content = Resource.GenerateTicket; 
        }
    }
}
