using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ESPNelson.Model
{

    /// <summary>
    /// Classe permettant d'initialiser un client HTTP pour interagir avec l'API de stationnement.
    /// </summary>
    public static class APIHelper
    {
        /// <summary>
        /// Client HTTP utilisé pour envoyer des requêtes à l'API.
        /// </summary>
        public static HttpClient APIClient { get; private set; }

        /// <summary>
        /// Constructeur statique initialisant automatiquement le client HTTP.
        /// </summary>
        static APIHelper()
        {
            InitializeClient(); 
        }

        /// <summary>
        /// Initialise le client HTTP avec l'URL de base et les en-têtes nécessaires.
        /// </summary>
        public static void InitializeClient()
        {
            if (APIClient == null)
            {
                APIClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:5000/api/")
                };
                APIClient.DefaultRequestHeaders.Accept.Clear();

                //Ajout de l'APIKey + Identifiant du programme
                APIClient.DefaultRequestHeaders.Add("ApiKey", "CLE_API_BORNE_ENTREE");
                APIClient.DefaultRequestHeaders.Add("X-Client-Type", "BorneEntree");
                APIClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Debug.WriteLine($"Clé API envoyée : {APIClient.DefaultRequestHeaders.GetValues("ApiKey").FirstOrDefault()}");
            }
        }
    }
}
