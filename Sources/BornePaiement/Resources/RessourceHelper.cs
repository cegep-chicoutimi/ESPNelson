using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BornePaiement.Resources
{
    /// <summary>
    /// Classe utilitaire pour gérer les ressources linguistiques de l'application.
    /// </summary>
    internal class RessourceHelper
    {
        /// <summary>
        /// Liste des langues disponibles pour l'application.
        /// </summary>
        private static readonly List<string> availableLanguages = new List<string> { "fr", "en" };

        /// <summary>
        /// Vérifie si une langue donnée est disponible dans l'application.
        /// </summary>
        /// <param name="lang">Code de la langue (ex: "fr", "en").</param>
        /// <returns>True si la langue est disponible, sinon False.</returns>
        public static bool IsAvailableLanguage(string lang)
        {
            return availableLanguages.Where(l => l.Equals(lang)).FirstOrDefault() != null ? true : false;
        }


        /// <summary>
        /// Retourne la langue actuellement utilisée par l'application.
        /// </summary>
        /// <returns>Code de la langue en cours (ex: "fr").</returns>
        public static string GetCurrentLanguage()
        {
            return Resource.Culture.Name;
        }


        /// <summary>
        /// Retourne la langue par défaut définie pour l'application.
        /// </summary>
        /// <returns>Code de la langue par défaut (ex: "fr").</returns>
        public static string GetDefaultLanguage()
        {
            return availableLanguages[0];
        }

        /// <summary>
        /// Initialise la langue de l'application en fonction des paramètres de configuration.
        /// </summary>
        public static void SetInitialLanguage()
        {
            string lang = ConfigurationManager.AppSettings["language"];

            if (!IsAvailableLanguage(lang))
                lang = GetDefaultLanguage();

            Resource.Culture = new CultureInfo(lang);
        }
    }
}
