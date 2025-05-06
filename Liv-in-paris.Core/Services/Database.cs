using Liv_in_paris.Core.Models;

namespace Liv_in_paris.Core.Services;

/// <summary>
/// Fournit un accès unique (singleton) à l'instance de <see cref="DatabaseManager"/> 
/// utilisée dans toute l'application pour interagir avec la base de données.
/// </summary>
public static class Database
{
    /// <summary>
    /// Instance unique de <see cref="DatabaseManager"/>, initialisée avec les paramètres de connexion.
    /// </summary>
    private static readonly DatabaseManager _instance = new DatabaseManager("localhost", "livin_paris", "root", "root");

    /// <summary>
    /// Accès à l'instance partagée de <see cref="DatabaseManager"/>.
    /// </summary>
    public static DatabaseManager Instance => _instance;
}