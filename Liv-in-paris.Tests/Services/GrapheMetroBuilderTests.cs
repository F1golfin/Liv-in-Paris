using Liv_in_paris.Core.Services;
using Liv_in_paris.Core.Graph;
using Liv_in_paris.Core.Entities;

namespace Liv_in_paris.Tests.Services;

/// <summary>
/// Tests unitaires pour GrapheMetroBuilder.
/// </summary>
public class GrapheMetroBuilderTests
{
    private string _stationFile;
    private string _liaisonFile;

    [SetUp]
    public void Setup()
    {
        _stationFile = Path.GetTempFileName();
        _liaisonFile = Path.GetTempFileName();
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(_stationFile);
        File.Delete(_liaisonFile);
    }

    /// <summary>
    /// Teste la construction d’un graphe simple à partir de deux fichiers CSV.
    /// </summary>
    [Test]
    public void ConstruireDepuisCSV_ConstructGraphe_WithValidData()
    {
        // Station.csv : ID;Ligne;Nom;Longitude;Latitude;Commune;Insee
        File.WriteAllText(_stationFile,
@"ID;Ligne;Nom;Longitude;Latitude;Commune;Insee
1;1;A;2.350;48.850;Paris;75001
2;1;B;2.351;48.851;Paris;75002
3;1;C;2.352;48.852;Paris;75003");

        // Liaison.csv : ID;Nom;Precedent;Suivant;Temps;Changement
        File.WriteAllText(_liaisonFile,
@"ID;Nom;Precedent;Suivant;Temps;Changement
1;A;;2;3;
2;B;1;3;4;
3;C;2;;3;");

        Graphe<Station> graphe = GrapheMetroBuilder.ConstruireDepuisCSV(_stationFile, _liaisonFile);

        Assert.NotNull(graphe);
        // Vérifie que 3 stations sont bien dans le graphe
        graphe.AfficherListeAdjacence();
    }

    /// <summary>
    /// Teste que les liaisons sont correctement orientées dans le graphe.
    /// </summary>
    [Test]
    public void ConstruireDepuisCSV_LiensAjoutesCorrectement()
    {
        File.WriteAllText(_stationFile,
@"ID;Ligne;Nom;Longitude;Latitude;Commune;Insee
1;1;X;2.300;48.800;Paris;75010
2;1;Y;2.310;48.810;Paris;75010");

        File.WriteAllText(_liaisonFile,
@"ID;Nom;Precedent;Suivant;Temps;Changement
1;X;;2;5;");

        var graphe = GrapheMetroBuilder.ConstruireDepuisCSV(_stationFile, _liaisonFile);

        var sortie = new StringWriter();
        Console.SetOut(sortie);
        graphe.AfficherListeAdjacence();
        string resultat = sortie.ToString();

        Assert.IsTrue(resultat.Contains("1 -> 2"));
    }
}
