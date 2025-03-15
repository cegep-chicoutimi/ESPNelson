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
    /// Logique d'interaction pour GestionView.xaml
    /// </summary>
    public partial class GestionView : Page
    {
        public GestionView()
        {
            InitializeComponent();
            DataContext = new GestionVM();

            LoadLabels();
        }

        public void LoadLabels()
        {
            label_ListDirectors.Content = Resource.ListDirectors;
            label_Add.Content = Resource.Add;
            label_Delete.Content = Resource.Delete;
            label_Users.Text = Resource.Users;  
        }
    }
}
