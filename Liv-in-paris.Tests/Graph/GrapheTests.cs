using Liv_in_paris.Core.Graph;

namespace Liv_in_paris.Tests.Graph;

/// <summary T=".">
/// Classe de tests unitaires pour la classe Graphe
/// </summary>
public class GrapheTests
{
    /// <summary>
    /// Type générique simple utilisé pour les tests.
    /// </summary>
    private class DonneeFictive
    {
        public string Nom => "X";
        public override string ToString() => Nom;
    }

    private Graphe<DonneeFictive> _graphe;

    [SetUp]
    public void Setup()
    {
        _graphe = new Graphe<DonneeFictive>();
    }

    /// <summary>
    /// Teste l’ajout d’un nœud unique dans le graphe.
    /// </summary>
    [Test]
    public void AjouterNoeud_AjouteCorrectement()
    {
        var noeud = new Noeud<DonneeFictive>(1);
        var result = _graphe.ajouterNoeud(noeud);
        Assert.That(result, Is.EqualTo(noeud));
    }

    /// <summary>
    /// Teste qu’ajouter deux nœuds avec le même Id ne duplique pas.
    /// </summary>
    [Test]
    public void AjouterNoeud_EviteDoublons()
    {
        var noeud1 = new Noeud<DonneeFictive>(1);
        var noeud2 = new Noeud<DonneeFictive>(1);
        _graphe.ajouterNoeud(noeud1);
        var result = _graphe.ajouterNoeud(noeud2);
        Assert.That(result, Is.EqualTo(noeud1)); // il doit garder le premier
    }

    /// <summary>
    /// Teste l’ajout d’un lien entre deux nœuds.
    /// </summary>
    [Test]
    public void AjouterLien_AjouteCorrectement()
    {
        var n1 = new Noeud<DonneeFictive>(1);
        var n2 = new Noeud<DonneeFictive>(2);
        var lien = new Lien<DonneeFictive>(n1, 3, n2);

        _graphe.ajouterLien(lien);
        // pas d’exception = OK
        Assert.Pass();
    }

    /// <summary>
    /// Teste qu’ajouter deux fois le même lien lève une exception.
    /// </summary>
    [Test]
    public void AjouterLien_DetecteDoublon()
    {
        var n1 = new Noeud<DonneeFictive>(1);
        var n2 = new Noeud<DonneeFictive>(2);
        var lien1 = new Lien<DonneeFictive>(n1, 3, n2);
        var lien2 = new Lien<DonneeFictive>(n1, 5, n2);

        _graphe.ajouterLien(lien1);
        Assert.Throws<Exception>(() => _graphe.ajouterLien(lien2));
    }

    /// <summary>
    /// Teste le bon fonctionnement du parcours en largeur.
    /// </summary>
    [Test]
    public void ParcoursEnLargeur_AfficheCorrectement()
    {
        var output = new StringWriter();
        Console.SetOut(output);

        var n1 = new Noeud<DonneeFictive>(0);
        var n2 = new Noeud<DonneeFictive>(1);
        var n3 = new Noeud<DonneeFictive>(2);

        _graphe.ajouterLien(new Lien<DonneeFictive>(n1, 1, n2));
        _graphe.ajouterLien(new Lien<DonneeFictive>(n2, 1, n3));

        _graphe.ParcoursEnLargeur(n1);

        string result = output.ToString();
        Assert.IsTrue(result.Contains("[0 1 2"));
    }

    /// <summary>
    /// Teste le bon fonctionnement du parcours en profondeur.
    /// </summary>
    [Test]
    public void ParcoursEnProfondeur_AfficheCorrectement()
    {
        var output = new StringWriter();
        Console.SetOut(output);

        var n1 = new Noeud<DonneeFictive>(0);
        var n2 = new Noeud<DonneeFictive>(1);
        var n3 = new Noeud<DonneeFictive>(2);

        _graphe.ajouterLien(new Lien<DonneeFictive>(n1, 1, n2));
        _graphe.ajouterLien(new Lien<DonneeFictive>(n2, 1, n3));

        _graphe.InitParcoursEnProfondeur(0);

        string result = output.ToString();
        Assert.IsTrue(result.Contains("[0 1 2]"));
    }
    [Test]
    public void Dijkstra_Trouve_Chemin_Correct()
    {
        // Arrange : Création du graphe
        Graphe<int> graphe = new Graphe<int>();

        // Création des nœuds
        Noeud<int> noeudA = new Noeud<int>(1);
        Noeud<int> noeudB = new Noeud<int>(2);
        Noeud<int> noeudC = new Noeud<int>(3);
        Noeud<int> noeudD = new Noeud<int>(4);

        // Ajout des nœuds au graphe
        graphe.ajouterNoeud(noeudA);
        graphe.ajouterNoeud(noeudB);
        graphe.ajouterNoeud(noeudC);
        graphe.ajouterNoeud(noeudD);

        // Ajout des liens pondérés
        graphe.ajouterLien(new Lien<int>(noeudA, 1, noeudB)); // A -> B (1)
        graphe.ajouterLien(new Lien<int>(noeudB, 2, noeudC)); // B -> C (2)
        graphe.ajouterLien(new Lien<int>(noeudA, 4, noeudC)); // A -> C (4)
        graphe.ajouterLien(new Lien<int>(noeudC, 1, noeudD)); // C -> D (1)

        // Act : Exécuter Dijkstra de A vers D
        List<int> chemin = graphe.Dijkstra(1, 4);

        // Assert : Vérification du chemin le plus court attendu
        List<int> cheminAttendu = new List<int> { 1, 2, 3, 4 };
        Assert.AreEqual(cheminAttendu, chemin);
    }
    [Test]

    public void Dijkstra_Aucun_Chemin_Disponible()
    {
        // Arrange : Création d'un graphe avec des nœuds non connectés
        Graphe<int> graphe = new Graphe<int>();
        Noeud<int> noeudA = new Noeud<int>(1);
        Noeud<int> noeudB = new Noeud<int>(2);

        graphe.ajouterNoeud(noeudA);
        graphe.ajouterNoeud(noeudB);

        // Act : Rechercher un chemin entre A et B (inexistant)
        List<int> chemin = graphe.Dijkstra(1, 2);

        // Assert : Vérifier que le chemin est vide
        Assert.IsEmpty  (chemin);
    }

    /// <summary>
    /// Vérifie que l'affichage de la liste d'adjacence fonctionne.
    /// </summary>
    [Test]
    public void AfficherListeAdjacence_ProduitSortie()
    {
        var output = new StringWriter();
        Console.SetOut(output);

        var n1 = new Noeud<DonneeFictive>(0);
        var n2 = new Noeud<DonneeFictive>(1);

        _graphe.ajouterLien(new Lien<DonneeFictive>(n1, 1, n2));
        _graphe.AfficherListeAdjacence();

        string result = output.ToString();
        Assert.IsTrue(result.Contains("0 -> 1"));
    }
}