namespace Liv_in_paris.Core.Graph;

/// <summary>
/// Représente un nœud générique dans un graphe, identifié par un identifiant unique et contenant des données de type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Le type de données associé au nœud. Ce type doit avoir un constructeur sans paramètre.</typeparam>
public class Noeud<T> where T : new()
{
    private readonly int _id;
    private T _data;

    /// <summary>
    /// Obtient l'identifiant unique du nœud.
    /// </summary>
    public int Id => _id;

    /// <summary>
    /// Obtient les données contenues dans le nœud.
    /// </summary>
    public T Data => _data;

    /// <summary>
    /// Initialise une nouvelle instance de la classe <see cref="Noeud{T}"/> avec un identifiant donné.
    /// </summary>
    /// <param name="id">L'identifiant unique du nœud.</param>
    public Noeud(int id)
    {
        _id = id;
        _data = new T();
    }

    /// <summary>
    /// Retourne une représentation textuelle du nœud, incluant l'identifiant et, si possible, les données.
    /// </summary>
    /// <returns>Une chaîne représentant le nœud sous la forme "id [data]" ou simplement "id" si les données ne sont pas disponibles.</returns>
    public override string ToString()
    {
        if (typeof(T).GetMethod("ToString") != null && _data != null)
        {
            return $"{_id} [{_data.ToString()}]";
        }
        else
        {
            return $"{_id}";
        }
    }
}
