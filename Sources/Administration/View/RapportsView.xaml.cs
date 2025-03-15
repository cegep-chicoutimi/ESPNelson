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
        }

        public void LoadLabels()
        {

        }
    }
}
