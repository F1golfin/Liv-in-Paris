using System.Data;
namespace Liv_in_paris.Core.Models;

public class LigneCommande
{
    public ulong LigneCommandeId { get; set; }
    public DateTime HeureLivraison { get; set; }
    public string AdresseArrivee { get; set; }
    public string Statut { get; set; }
    public ulong CommandeId { get; set; }

    public void AjouterCommande(DatabaseManager database)
    {
        string query = $@"
        INSERT INTO lignes_commandes (
            heure_livraison, adresse_arrivee, statut, commande_id
        ) VALUES (
            '{HeureLivraison:yyyy-MM-dd HH:mm:ss}',
            '{AdresseArrivee}',
            '{Statut}',
            {CommandeId}
        );
    ";
        database.ExecuteNonQuery(query);
    }

    public void ModifierCommande(DatabaseManager database)
    {
        string query = $@"UPDATE lignes_commandes SET
            heure_livraison = '{{HeureLivraison:yyyy-MM-dd HH:mm:ss}}',
            adresse_arrivee = '{{AdresseArrivee}}',
            statut = '{{Statut}}',
            commande_id = {{CommandeId}}
        WHERE ligne_commande_id = {{LigneCommandeId}};
    ";
        database.ExecuteNonQuery(query);
    }
    
    public void SupprimerCommande(DatabaseManager db)
    {
        string query = $"DELETE FROM lignes_commandes WHERE ligne_commande_id = {LigneCommandeId};";
        db.ExecuteNonQuery(query);
    }
    
    public static List<LigneCommande> GetAll(DatabaseManager db)
    {
        var lignes = new List<LigneCommande>();
        var table = db.ExecuteQuery("SELECT * FROM lignes_commandes;");

        foreach (DataRow row in table.Rows)
        {
            lignes.Add(new LigneCommande
            {
                LigneCommandeId = Convert.ToUInt64(row["ligne_commande_id"]),
                HeureLivraison = Convert.ToDateTime(row["heure_livraison"]),
                AdresseArrivee = row["adresse_arrivee"].ToString(),
                Statut = row["statut"].ToString(),
                CommandeId = Convert.ToUInt64(row["commande_id"])
            });
        }

        return lignes;
    }

    
}