namespace Liv_in_paris.Core.Graph;

/// <summary>
/// Représente un lien pondéré entre deux nœuds dans un graphe.
/// </summary>
/// <typeparam name="T">Le type de données contenues dans les nœuds.</typeparam>
public class Lien<T> where T : new()
{
    private readonly Noeud<T> _origine;
    private readonly Noeud<T> _destination;
    private readonly int _poids;

    /// <summary>
    /// Obtient le nœud d'origine du lien.
    /// </summary>
    public Noeud<T> Origine => _origine;

    /// <summary>
    /// Obtient le nœud de destination du lien.
    /// </summary>
    public Noeud<T> Destination => _destination;

    /// <summary>
    /// Obtient le poids associé à ce lien.
    /// </summary>
    public int Poids => _poids;

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="Lien{T}"/>.
    /// </summary>
    /// <param name="origine">Le nœud d'origine.</param>
    /// <param name="poids">Le poids du lien (par exemple une distance, un temps de trajet, etc.).</param>
    /// <param name="destination">Le nœud de destination.</param>
    public Lien(Noeud<T> origine, int poids, Noeud<T> destination)
    {
        _origine = origine;
        _destination = destination;
        _poids = poids;
    }

    /// <summary>
    /// Retourne une représentation textuelle du lien.
    /// </summary>
    /// <returns>Une chaîne représentant le lien sous la forme "origine --(poids)--> destination".</returns>
    public override string ToString()
    {
        return $"{_origine} --({_poids})--> {_destination}";
    }
}