using System.Data;
using Liv_in_paris.Core.Entities;
using Liv_in_paris.Core.Graph;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris.Core;

class Program
{
    static void test()
    {
        string onglet_1 = "../../../../Files/MetroParis_onglet1.csv";
        string onglet_2 = "../../../../Files/MetroParis_onglet2.csv";


        Graphe<Station> graphe = GrapheMetroBuilder.ConstruireDepuisCSV(onglet_1, onglet_2);

        Console.WriteLine("Graphe chargé !");
        graphe.AfficherListeAdjacence();
        //graphe.AfficherMatriceAdjacence();

        DatabaseManager.CreateDatabase("localhost", "root", "root", "livin_paris");
        var db = new DatabaseManager("localhost", "livin_paris", "root", "root"); //à modifier en fonction de chancun

        try
        {
            db.TesterConnexion();
            db.CreateTablesIfNotExists();
            Console.WriteLine("🚀 Base prête !");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Erreur : " + ex.Message);
        }

        Console.WriteLine("Appuie sur Entrée pour quitter.");
        Console.ReadLine();
    }
    
    // Ici est éxécuté uniquement le projet .Core
    static async Task Main(string[] args)
    {
        var service = new AdresseService();

        Console.Write("Entrez une adresse à Paris : ");
        string query = Console.ReadLine();

        var suggestions = await service.ObtenirSuggestionsAsync(query);
        Console.WriteLine("\nSuggestions trouvées :");
        foreach (var s in suggestions)
            Console.WriteLine($"- {s}");

        if (suggestions.Count > 0)
        {
            Console.WriteLine("\nObtention des coordonnées GPS de la première suggestion...");
            var coords = await service.ObtenirCoordonneesAsync(suggestions[0]);

            if (coords != null)
                Console.WriteLine($"Latitude : {coords.Value.lat}, Longitude : {coords.Value.lon}");
            else
                Console.WriteLine("❌ Impossible de récupérer les coordonnées.");
        }

        Console.WriteLine("\nTest terminé.");
    }

}