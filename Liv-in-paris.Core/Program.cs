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

        DatabaseManager.CreateDatabase("localhost", "root", "amandine", "livin_paris");
        var db = new DatabaseManager("localhost", "livin_paris", "root", "amandine"); // ou livin_user / livin_pass

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
static void Main(string[] args)
    {
        int[] tab = new int[5];
        foreach (int i in tab)
        {
            Console.Write(i);
        }
    }
}