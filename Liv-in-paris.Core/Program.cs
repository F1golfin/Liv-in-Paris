using System.Data;
using Liv_in_paris.Core.Entities;
using Liv_in_paris.Core.Graph;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris.Core;

class Program
{
    // Ici est éxécuté uniquement le projet .Core
    static void Main(string[] args)
    {
        string onglet_1 = "../../../../Files/MetroParis_test_onglet1.csv";
        string onglet_2 = "../../../../Files/MetroParis_test_onglet2.csv";


        Graphe<Station> graphe = GrapheMetroBuilder.ConstruireDepuisCSV(onglet_1, onglet_2);

        Console.WriteLine("Graphe chargé !");
        graphe.AfficherListeAdjacence();
        //graphe.AfficherMatriceAdjacence();

        //Test lien bdd et code
        var dbPath = "C:\\Users\\chris\\RiderProjects\\Liv-in-Paris\\identifier.sqlite";
        var db = new DatabaseManager(dbPath);
        
        var clients = db.ExecuteQuery("SELECT * FROM Clients");

        foreach (System.Data.DataRow row in clients.Rows )
        {
            Console.WriteLine($"{row["Nom"]} - {row["Email"]} - {row["Prenom"]}");
        }
    }





}