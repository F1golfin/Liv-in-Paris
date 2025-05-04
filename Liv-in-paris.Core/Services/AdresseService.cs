using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Liv_in_paris.Core.Services
{
    public class AdresseService
    {
        private readonly HttpClient _httpClient = new();
        private readonly Dictionary<string, List<string>> _cacheSuggestions = new();

        /// <summary>
        /// Récupère une liste d'adresses à Paris correspondant à la saisie.
        /// </summary>
        public async Task<List<string>> ObtenirSuggestionsAsync(string saisie)
        {
            if (string.IsNullOrWhiteSpace(saisie) || saisie.Length < 3)
                return new List<string>();

            string cle = saisie.Trim().ToLowerInvariant();
            if (_cacheSuggestions.ContainsKey(cle))
                return _cacheSuggestions[cle];

            string url = $"https://api-adresse.data.gouv.fr/search/?q={Uri.EscapeDataString(saisie + ", Paris")}&limit=5&autocomplete=1";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var jsonDoc = await JsonDocument.ParseAsync(stream);
                var results = new List<string>();

                foreach (var feature in jsonDoc.RootElement.GetProperty("features").EnumerateArray())
                {
                    var properties = feature.GetProperty("properties");

                    string label = properties.GetProperty("label").GetString();
                    string postcode = properties.GetProperty("postcode").GetString();

                    // On garde uniquement les adresses à Paris (code postal commence par 75) et qui ont un numéro
                    if (!postcode.StartsWith("75")) continue;
                    if (!Regex.IsMatch(label, @"^\d+")) continue;

                    results.Add(label);
                }

                _cacheSuggestions[cle] = results;
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'appel à l'API Adresse: {ex.Message}");
                return new List<string>();
            }
        }


        /// <summary>
        /// Récupère les coordonnées GPS (lat, lon) d'une adresse.
        /// </summary>
        public async Task<(double lat, double lon)?> ObtenirCoordonneesAsync(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                return null;

            string url = $"https://api-adresse.data.gouv.fr/search/?q={Uri.EscapeDataString(adresse + ", Paris")}&limit=1&autocomplete=1";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var jsonDoc = await JsonDocument.ParseAsync(stream);

                var feature = jsonDoc.RootElement.GetProperty("features")[0];
                var context = feature.GetProperty("properties").GetProperty("context").GetString();
                var label = feature.GetProperty("properties").GetProperty("label").GetString();

                // Vérifie que l'adresse est bien dans Paris (ex: "75" et "Paris" présents)
                if (!context.Contains("Paris") || !context.Contains("75") || !Regex.IsMatch(label, @"^\d+"))
                    return null;

                var coordinates = feature.GetProperty("geometry").GetProperty("coordinates");

                double lon = coordinates[0].GetDouble();
                double lat = coordinates[1].GetDouble();
                return (lat, lon);
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<bool> EstAdresseValideAsync(string adresse)
        {
            if (string.IsNullOrWhiteSpace(adresse))
                return false;

            string url = $"https://api-adresse.data.gouv.fr/search/?q={Uri.EscapeDataString(adresse)}&limit=1&autocomplete=0";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var jsonDoc = await JsonDocument.ParseAsync(stream);

                var features = jsonDoc.RootElement.GetProperty("features");

                if (features.GetArrayLength() == 0)
                    return false;

                var properties = features[0].GetProperty("properties");

                string label = properties.GetProperty("label").GetString();
                string postcode = properties.GetProperty("postcode").GetString();

                // Vérifie que le code postal commence bien par "75"
                return postcode.StartsWith("75") && Regex.IsMatch(label, @"^\d+");
            }
            catch
            {
                return false;
            }
        }

    }
}
