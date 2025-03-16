using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BornePaiement.ViewModel;
using BornePaiement.Resources;

namespace BornePaiement.View
{
    public partial class AbonnementPopup : Window
    {
        public AbonnementPopup(string ticketScanne)
        {
            InitializeComponent();
            DataContext = new AbonnementPopupVM(ticketScanne);

            loadLabels();
        }

        public void loadLabels()
        {
            label_Subscription.Title = Resource.Subscription;
            label_Email.Content = Resource.Email;
            label_SubscriptionType.Content = Resource.SubscriptionType; 
            label_ConfirmAndPay.Content = Resource.ConfirmAndPay;
            label_GenerateSubscriptionTicket.Content = Resource.GenerateSubscriptionTicket; 
        }
    }
}