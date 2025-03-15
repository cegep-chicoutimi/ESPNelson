using Administration.Model;
using Administration.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace Administration.ViewModel
{
    public partial class TarificationDialogVM : ObservableObject
    {
        [ObservableProperty]
        private Tarification tarification;

        public Action<bool> CloseDialogAction { get; }

        public TarificationDialogVM(Tarification tarification, Action<bool> closeDialogAction)
        {
            Tarification = tarification;
           
            CloseDialogAction = closeDialogAction;
        }

        [RelayCommand]
        private void Enregistrer()
        {
            if (Tarification.Prix < 0 || Tarification.DureeMin < 0 || Tarification.DureeMax <= 0)
            {
                MessageBox.Show(
                          Resource.InvalidValues,
                          Resource.ErrorTitle,
                          MessageBoxButton.OK,
                          MessageBoxImage.Error
                      );
                return;
            }

            if (Tarification.DureeMin >= Tarification.DureeMax)
            {
                MessageBox.Show(
                     Resource.MinDurationLessThanMax,
                     Resource.ErrorTitle,
                     MessageBoxButton.OK,
                     MessageBoxImage.Error
                 );
                return;
            }

            CloseDialogAction?.Invoke(true); // Clôture avec succès
        }

        [RelayCommand]
        private void Annuler()
        {
            CloseDialogAction?.Invoke(false); // Clôture sans succès
        }
    }
}
