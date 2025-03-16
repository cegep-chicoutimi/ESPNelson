using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BorneSortie.Model
{
    public static class APIHelper
    {
        /// <summary>
        /// Client HTTP réutilisable pour effectuer des requêtes vers l'API.
        /// </summary>
        public static HttpClient APIClient { get; private set; }

        /// <summary>
        /// Constructeur statique initialisant le client HTTP.
        /// </summary>
        static APIHelper()
        {
            InitializeClient();
        }

        /// <summary>
        /// Initialise le client HTTP avec l'URL de base et les en-têtes requis pour l'API.
        /// </summary>
        public static void InitializeClient()
        {
            if (APIClient == null)
            {
                // Charger l'URL de l'API depuis le fichier de configuration
                var apiUrl = ConfigurationHelper.LoadApiUrl();
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    throw new InvalidOperationException("L'URL de l'API n'est pas configurée.");
                }

                APIClient = new HttpClient
                {
                    BaseAddress = new Uri(apiUrl)
                };


                APIClient.DefaultRequestHeaders.Accept.Clear();
                APIClient.DefaultRequestHeaders.Add("ApiKey", "CLE_API_BORNE_SORTIE"); // Clé API spécifique à BornePaiement
                APIClient.DefaultRequestHeaders.Add("X-Client-Type", "BorneSortie");
                APIClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }
    }
}
