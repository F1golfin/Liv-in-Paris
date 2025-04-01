using System.Data;
namespace Liv_in_paris.Core.Models;

public class Commande
{
    public ulong CommandeId { get; set; }
    public DateTime HeureCommande { get; set; }
    public string AdresseDepart { get; set; }
    public decimal PrixTotal { get; set; }
    public ulong? ClientId { get; set; }
    public ulong? CuisinierId { get; set; }

    public void AjouterCommande(DatabaseManager database)
    {
        string query = $@"
        INSERT INTO commandes (
            heure_commande, adresse_depart, prix_total, client_id, cuisinier_id
        ) VALUES (
            '{HeureCommande:yyyy-MM-dd HH:mm:ss}',
            '{AdresseDepart}',
            {PrixTotal.ToString(System.Globalization.CultureInfo.InvariantCulture)},
            {(ClientId != null ? ClientId.ToString() : "NULL")},
            {(CuisinierId != null ? CuisinierId.ToString() : "NULL")}
        );
    ";

        database.ExecuteNonQuery(query);
    }

    public void ModifierCommande(DatabaseManager database)
    {
        string query = $@"
        UPDATE commandes SET
            heure_commande = '{HeureCommande:yyyy-MM-dd HH:mm:ss}',
            adresse_depart = '{AdresseDepart}',
            prix_total = {PrixTotal.ToString(System.Globalization.CultureInfo.InvariantCulture)},
            client_id = {(ClientId != null ? ClientId.ToString() : "NULL")},
            cuisinier_id = {(CuisinierId != null ? CuisinierId.ToString() : "NULL")}
        WHERE commande_id = {CommandeId};
    ";

        database.ExecuteNonQuery(query);
    }

    public void SupprimerCommande(DatabaseManager database)
    {
        string query = $"DELETE FROM commandes WHERE commande_id = {CommandeId};";
        database.ExecuteNonQuery(query);
    }
    
    public static List<Commande> GetAll(DatabaseManager db)
    {
        var commandes = new List<Commande>();
        var table = db.ExecuteQuery("SELECT * FROM commandes;");

        foreach (DataRow row in table.Rows)
        {
            commandes.Add(new Commande
            {
                CommandeId = Convert.ToUInt64(row["commande_id"]),
                HeureCommande = Convert.ToDateTime(row["heure_commande"]),
                AdresseDepart = row["adresse_depart"].ToString(),
                PrixTotal = Convert.ToDecimal(row["prix_total"]),
                ClientId = row["client_id"] == DBNull.Value ? null : Convert.ToUInt64(row["client_id"]),
                CuisinierId = row["cuisinier_id"] == DBNull.Value ? null : Convert.ToUInt64(row["cuisinier_id"])
            });
        }

        return commandes;
    }

    
    
}