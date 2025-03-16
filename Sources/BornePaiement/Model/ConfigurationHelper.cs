using Newtonsoft.Json;
using System.IO;

namespace BornePaiement.Model
{
    /// <summary>
    /// Classe utilitaire pour gérer la configuration de l'application.
    /// </summary>
    public static class ConfigurationHelper
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");


        public static void SaveApiUrl(string apiUrl)
        {
            // Charger les configurations existantes
            dynamic config = LoadConfig();

            // Modifier uniquement la valeur de BaseUrl
            if (config != null)
            {
                config.ApiSettings.BaseUrl = apiUrl;
            }
            else
            {
                // Si le fichier n'existe pas ou est vide, créer une nouvelle configuration
                config = new
                {
                    ApiSettings = new
                    {
                        BaseUrl = apiUrl
                    }
                };
            }

            // Options de formatage pour le JSON
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented // Ajoute des indentations et des sauts de ligne
            };

            // Réécrire le fichier de configuration avec les nouvelles configurations
            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(config, settings));
        }

        /// <summary>
        /// Charge les configurations existantes depuis le fichier appsettings.json.
        /// </summary>
        /// <returns>Les configurations sous forme d'objet dynamique, ou null si le fichier n'existe pas ou est vide.</returns>
        private static dynamic LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                // Lire le fichier de configuration
                var json = File.ReadAllText(ConfigFilePath);
                return JsonConvert.DeserializeObject<dynamic>(json);
            }
            return null;
        }

        public static string LoadApiUrl()
        {
            if (File.Exists(ConfigFilePath))
            {
                // Lire le fichier de configuration
                var json = File.ReadAllText(ConfigFilePath);
                var config = JsonConvert.DeserializeObject<dynamic>(json);
                return config?.ApiSettings?.BaseUrl;
            }
            return null; // Retourne null si le fichier n'existe pas ou si l'URL n'est pas configurée
        }
    }
}