using BorneSortie.View;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace BorneSortie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        }

       
        protected override void OnStartup(StartupEventArgs e)
        {
         

            // Activer les messages de débogage pour le binding
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.All;
            PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());

            base.OnStartup(e);


            // Afficher la fenêtre de configuration
            var configWindow = new ApiConfigurationView();
            configWindow.ShowDialog();


        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ESPNelson_ErrorLog.txt");
            string errorMessage = $"Erreur non gérée : {e.Exception.Message}\nStack Trace : {e.Exception.StackTrace}";

            File.WriteAllText(logPath, errorMessage);
            MessageBox.Show($"Une erreur s'est produite. Voir le fichier de log : {logPath}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true; // Empêche l'application de planter
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"Erreur non gérée : {ex?.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }



}
