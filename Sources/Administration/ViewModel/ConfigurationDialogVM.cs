using Administration.Model;
using Administration.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace Administration.ViewModel
{
    public partial class ConfigurationDialogVM : ObservableObject
    {
        [ObservableProperty]
        private int _capaciteMax;

        [ObservableProperty]
        private decimal _taxeFederal;

        [ObservableProperty]
        private decimal _taxeProvincial;

        public Action<bool>? CloseDialogAction { get; set; }  // Action pour fermer la fenêtre

        public ConfigurationDialogVM() { }

        [RelayCommand]
        private void Valider()
        {
            // Vérifier que les valeurs sont valides
            if (CapaciteMax <= 0 || TaxeFederal < 0 || TaxeProvincial < 0)
            {
                MessageBox.Show(
                         Resource.InvalidValues,
                         Resource.ErrorTitle,
                         MessageBoxButton.OK,
                         MessageBoxImage.Error
                     );
                return;
            }

            if(!int.TryParse(CapaciteMax.ToString(), out int entier))
            {
                    MessageBox.Show(
                            Resource.CapacityMustBePositiveInteger,
                            Resource.ErrorTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                return;
            }

            // Fermer la fenêtre avec un résultat positif
            CloseDialogAction?.Invoke(true);
        }

        [RelayCommand]
        private void Annuler()
        {
            // Fermer la fenêtre avec un résultat négatif
            CloseDialogAction?.Invoke(false);
        }
    }
}