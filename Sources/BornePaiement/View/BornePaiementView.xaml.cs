﻿using System;
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
using BornePaiement.ViewModel;
using System.Windows.Media.Animation;
using BornePaiement.View;

namespace BornePaiement.View
{
    /// <summary>
    /// Logique d'interaction pour BornePaiementView.xaml
    /// </summary>
    public partial class BornePaiementView : Window
    {
        private StringBuilder _scanBuffer = new StringBuilder(); // Buffeur pour collecter les données du scan
        public BornePaiementView()
        {
            InitializeComponent();
            this.DataContext = new BornePaiementVM();
        }

        private void HiddenScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Transmettre l'événement clavier à la méthode principale
            Page_KeyDown(sender, e);
        }

        private async void Page_KeyDown(object sender, KeyEventArgs e)
        {
            // Ignorer les touches spéciales
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt ||
                e.Key == Key.CapsLock || e.Key == Key.Tab ||
                e.Key == Key.Escape || e.Key == Key.Back)
            {
                return;
            }

            if (e.Key == Key.Enter) // 🎯 Lorsque l'utilisateur a scanné son ticket
            {
                await Task.Delay(100); // Délai de 100 ms pour s'assurer que le scan est complet
                // Transmettre l'ID du ticket au ViewModel
                if (DataContext is BornePaiementVM viewModel)
                {
                    viewModel.VerifierTicket(_scanBuffer.ToString());
                }
                _scanBuffer.Clear(); // Réinitialiser le buffeur après le traitement
            }
            else
            {
                // Capturer les chiffres (0-9)
                if (e.Key >= Key.D0 && e.Key <= Key.D9) // Chiffres de 0 à 9
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("D", "")); // Supprimer le préfixe "D" pour les chiffres
                }
                // Capturer les chiffres du pavé numérique (0-9)
                else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) // Chiffres du pavé numérique
                {
                    _scanBuffer.Append(e.Key.ToString().Replace("NumPad", "")); // Supprimer le préfixe "NumPad"
                }
                // Capturer les lettres (A-Z)
                else if (e.Key >= Key.A && e.Key <= Key.Z) // Lettres de A à Z
                {
                    _scanBuffer.Append(e.Key.ToString()); // Conserver la lettre telle quelle
                }
            }
        }
    }
}
