namespace Liv_in_paris.Core.Entities;

public class Station
{
    /// <summary>
    /// Nom de la station.
    /// </summary>
    public string Nom { get; set; }

    /// <summary>
    /// Nom ou numéro de la ligne de métro à laquelle appartient la station.
    /// </summary>
    public string Ligne { get; set; }

    /// <summary>
    /// Longitude géographique de la station (en degrés décimaux).
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Latitude géographique de la station (en degrés décimaux).
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Commune dans laquelle se trouve la station.
    /// </summary>
    public string Commune { get; set; }

    /// <summary>
    /// Code INSEE de la commune.
    /// </summary>
    public string Insee { get; set; }
    
    /// <summary>
    /// Retourne une représentation textuelle de la station.
    /// </summary>
    public override string ToString()
    {
        return $"{Nom} (Ligne {Ligne})";  //- {Commune} [{Latitude}, {Longitude}]
    }
}