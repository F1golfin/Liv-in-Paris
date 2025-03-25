using Liv_in_paris.Core.Graph;

namespace Liv_in_paris.Tests;

/// <summary>
/// Classe de tests unitaires pour la classe générique Lien<T>.
/// </summary>
public class LienTests
{
    /// <summary>
    /// Classe de test utilisée comme type générique.
    /// </summary>
    private class DummyData
    {
        public string Nom { get; set; } = "Station";
        public override string ToString() => Nom;
    }

    /// <summary>
    /// Teste que le lien est correctement initialisé avec ses extrémités et son poids.
    /// </summary>
    [Test]
    public void Constructeur_AssigneCorrectementOrigineDestinationEtPoids()
    {
        Noeud<DummyData> noeud1 = new Noeud<DummyData>(1);
        Noeud<DummyData> noeud2 = new Noeud<DummyData>(2);
        Lien<DummyData> lien = new Lien<DummyData>(noeud1, 5, noeud2);

        Assert.That(lien.Origine, Is.EqualTo(noeud1));
        Assert.That(lien.Destination, Is.EqualTo(noeud2));
        Assert.That(lien.Poids, Is.EqualTo(5));
    }

    /// <summary>
    /// Teste que la méthode ToString affiche correctement le lien.
    /// </summary>
    [Test]
    public void ToString_RetourneFormatAttendu()
    {
        var noeud1 = new Noeud<DummyData>(1);
        var noeud2 = new Noeud<DummyData>(2);
        var lien = new Lien<DummyData>(noeud1, 3, noeud2);

        string attendu = $"{noeud1} --(3)--> {noeud2}";
        Assert.That(lien.ToString(), Is.EqualTo(attendu));
    }
}