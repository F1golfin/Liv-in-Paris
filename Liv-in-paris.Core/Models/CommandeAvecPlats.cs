namespace Liv_in_paris.Core.Models;

/// <summary>
/// Structure utilisée pour afficher une commande avec ses plats liés,
/// son statut de livraison et le nom du cuisinier.
/// </summary>
public class CommandeAvecPlats
{
    public Commande Commande { get; set; }
    public List<Plat> Plats { get; set; } = new();
    public string Statut { get; set; }
    public string CuisinierNom { get; set; }
}