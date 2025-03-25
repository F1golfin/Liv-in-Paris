using Liv_in_paris.Core.Graph;

namespace Liv_in_paris.Tests;

/// <summary>
/// Classe de tests unitaires pour la classe générique Noeud<T>.
/// </summary>
public class NoeudTests
{
    /// <summary>
    /// Classe fictive pour les tests, avec redéfinition de ToString().
    /// </summary>
    private class ExempleData
    {
        public override string ToString() => "TestData";
    }

    /// <summary>
    /// Teste que le constructeur initialise correctement l'identifiant.
    /// </summary>
    [Test]
    public void Constructeur_AssigneIdCorrectement()
    {
        Noeud<ExempleData> noeud = new Noeud<ExempleData>(42);
        Assert.That(noeud.Id, Is.EqualTo(42));
    }

    /// <summary>
    /// Teste que le champ Data est bien instancié via le constructeur générique.
    /// </summary>
    [Test]
    public void Constructeur_InitialiseData()
    {
        Noeud<ExempleData> noeud = new Noeud<ExempleData>(1);
        Assert.IsNotNull(noeud.Data);
    }

    /// <summary>
    /// Teste que ToString retourne la bonne chaîne formatée avec Data.
    /// </summary>
    [Test]
    public void ToString_RetourneIdEtData()
    {
        Noeud<ExempleData> noeud = new Noeud<ExempleData>(7);
        string resultat = noeud.ToString();
        Assert.That(resultat, Is.EqualTo("7 [TestData]"));
    }

    /// <summary>
    /// Teste que ToString fonctionne même si la classe Data n’a pas une redéfinition explicite de ToString().
    /// </summary>
    private class ClasseSansToString { }

    [Test]
    public void ToString_AvecClasseSansToString()
    {
        Noeud<ClasseSansToString> noeud = new Noeud<ClasseSansToString>(10);
        string resultat = noeud.ToString();
        Assert.IsTrue(resultat.StartsWith("10 ["));
    }
}
