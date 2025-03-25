using Liv_in_paris.Core.Entities;

namespace Liv_in_paris.Tests.Entities;

/// <summary>
/// Classe de tests unitaires pour la classe Station.
/// </summary>
public class StationTests
{
    /// <summary>
    /// Teste que l'on peut créer une station avec des valeurs valides.
    /// </summary>
    [Test]
    public void ConstructeurEtProprietes_AssigneValeursCorrectes()
    {
        var station = new Station
        {
            Nom = "Châtelet",
            Ligne = "1",
            Longitude = 2.347,
            Latitude = 48.858,
            Commune = "Paris",
            Insee = "75056"
        };

        Assert.That(station.Nom, Is.EqualTo("Châtelet"));
        Assert.That(station.Ligne, Is.EqualTo("1"));
        Assert.That(station.Longitude, Is.EqualTo(2.347));
        Assert.That(station.Latitude, Is.EqualTo(48.858));
        Assert.That(station.Commune, Is.EqualTo("Paris"));
        Assert.That(station.Insee, Is.EqualTo("75056"));
    }

    /// <summary>
    /// Teste que ToString retourne bien une représentation lisible.
    /// </summary>
    [Test]
    public void ToString_RetourneTexteAttendu()
    {
        var station = new Station
        {
            Nom = "République",
            Ligne = "3",
            Commune = "Paris",
            Latitude = 48.867,
            Longitude = 2.363
        };

        string attendu = "République (Ligne 3) - Paris [48,867, 2,363]";
        Assert.That(station.ToString(), Is.EqualTo(attendu));
    }
}

