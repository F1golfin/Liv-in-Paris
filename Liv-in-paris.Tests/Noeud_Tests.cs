using Liv_in_paris.Core;
namespace Liv_in_paris.Tests;

public class Tests
{
    /// <summary>
    /// Vérifie que le constructeur de Noeud stocke correctement l'ID.
    /// </summary>
    [Test]
    public void Noeud_ShouldStoreIdCorrectly()
    {
        int expectedId = 42;
        string expecteddata = "Nom";
        Noeud<int> noeud = new Noeud<int>(expectedId, expecteddata);
        Assert.That(noeud.Id, Is.EqualTo(expectedId));
        Assert.That(noeud.Data, Is.EqualTo(expecteddata));
    }
}