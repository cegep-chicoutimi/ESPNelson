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
    /// Logique d'interaction pour TableauBordView.xaml
    /// </summary>
    public partial class TableauBordView : Page
    {
        public TableauBordView()
        {
            InitializeComponent();
            DataContext = new TableauBordVM();

            LoadLabels();

        }

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            label_Dashboard.Text = Resource.Dashboard;
            label_Graph7Days.Content = Resource.Graph7Days;
            label_TicketsOnly.Content = Resource.TicketsOnly;
            label_SubscriptionOnly.Content = Resource.SubscriptionsOnly;
            label_All.Content = Resource.All;
            label_PlacesOccupied.Text = Resource.PlacesAvailable;
            label_PlacesAvailable.Text = Resource.PlacesAvailable;
            label_Day.Text = Resource.Day;
            label_Hour.Text = Resource.Hour;   
            label_Refresh.ToolTip = Resource.Refresh;   
        }
    }
}
