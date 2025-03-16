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
                APIClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:5000/api/")
                };
                APIClient.DefaultRequestHeaders.Accept.Clear();
                APIClient.DefaultRequestHeaders.Add("ApiKey", "CLE_API_BORNE_PAIEMENT"); // Clé API spécifique à BornePaiement
                APIClient.DefaultRequestHeaders.Add("X-Client-Type", "BornePaiement");
                APIClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }
    }
}
