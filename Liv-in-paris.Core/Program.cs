using Liv_in_paris.Core.Entities;
using Liv_in_paris.Core.Graph;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris.Core;

class Program
{
    // Ici est éxécuté uniquement le projet .Core
    static void Main(string[] args)
    {
        string onglet_1 = "../../../../Files/MetroParis_onglet1.csv";
        string onglet_2 = "../../../../Files/MetroParis_onglet2.csv";
        

        Graphe<Station> graphe = GrapheMetroBuilder.ConstruireDepuisCSV(onglet_1, onglet_2);

        Console.WriteLine("Graphe chargé !");
        graphe.AfficherListeAdjacence();
        //graphe.AfficherMatriceAdjacence();
    }
    
}