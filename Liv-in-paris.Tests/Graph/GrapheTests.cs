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

        _graphe.ParcoursEnProfondeur();

        string result = output.ToString();
        Assert.IsTrue(result.Contains("[0 1 2"));
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