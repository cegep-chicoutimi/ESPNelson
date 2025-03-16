using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BornePaiement.Model
{
    public static class APIHelper
    {
        public static HttpClient APIClient { get; private set; }

        static APIHelper()
        {
            InitializeClient();
        }

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
                APIClient.DefaultRequestHeaders.Add("ApiKey", "CLE_API_BORNE_PAIEMENT"); // Clé API spécifique à BornePaiement
                APIClient.DefaultRequestHeaders.Add("X-Client-Type", "BornePaiement");
                APIClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }
    }
}
