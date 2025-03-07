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
        Noeud noeud = new Noeud(expectedId);
        Assert.That(noeud.Id, Is.EqualTo(expectedId));
    }
}