﻿using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ESPNelson.Model
{

    /// <summary>
    /// Initialisation du client HTTP au démarrage, définition de l'URL de base de StationnmentAPI et ajout des en-têtes JSON
    /// </summary>
    public static class APIHelper
    {
        public static HttpClient APIClient { get; private set; }

        static APIHelper()
        {
            InitializeClient(); // Initialisation automatique
        }

        public static void InitializeClient()
        {
            if (APIClient == null)
            {
                APIClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:7185/api/")
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
