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

        /// <summary>
        /// Charge les labels et met à jour leur texte en fonction de la langue sélectionnée.
        /// </summary>
        public void LoadLabels()
        {
            label_SystemConfigurations.Content = Resource.SystemConfigurations;
            label_Add.Content = Resource.Add;
            label_Delete.Content = Resource.Delete;
            label_Users.Text = Resource.Users;  
            label_ListDirectors.Text = Resource.ListDirectors; 
            label_SystemConfigurationsInformation.Text = Resource.SystemConfigurationsInformation;
            label_Management.Content = Resource.Management;   
            label_Username.Header = Resource.Username;  
            label_Email.Header = Resource.Email;    
            label_Add1.Content = Resource.Add;  
            label_Delete1.Content = Resource.Delete;
            label_Edit.Content = Resource.Edit; 
            label_PriceList.Text = Resource.PriceList;  
            label_PredefinedPricing.Text = Resource.PredefinedPricing;  
            label_Level.Header = Resource.Level;    
            label_Price.Header = Resource.Price;
            label_DurationMin.Header = Resource.DurationMin;
            label_DurationMax.Header = Resource.DurationMax;  
            label_Edit1.Content = Resource.Edit;
            label_MaxCapacity.Header = Resource.MaxCapacity;    
            label_FederalTax.Header = Resource.FederalTax;
            label_ProvincialTax.Header = Resource.ProvincialTax;
            label_DateOfModication.Header = Resource.DateOfModification;
            label_MadeBy.Header = Resource.MadeBy;  
        }
    }
}
