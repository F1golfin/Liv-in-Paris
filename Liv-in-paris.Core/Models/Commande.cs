using System.Data;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente une commande passée par un client à un cuisinier.
/// Contient les métadonnées de la commande et ses lignes associées.
/// </summary>
public class Commande
{
    /// <summary>
    /// Identifiant unique de la commande.
    /// </summary>
    public ulong CommandeId { get; set; }

    /// <summary>
    /// Horodatage de la commande.
    /// </summary>
    public DateTime HeureCommande { get; set; }

    /// <summary>
    /// Adresse de départ (du cuisinier).
    /// </summary>
    public string AdresseDepart { get; set; }

    /// <summary>
    /// Prix total de la commande.
    /// </summary>
    public decimal PrixTotal { get; set; }

    /// <summary>
    /// Identifiant du client ayant passé la commande.
    /// </summary>
    public ulong? ClientId { get; set; }

    /// <summary>
    /// Identifiant du cuisinier qui préparera la commande.
    /// </summary>
    public ulong? CuisinierId { get; set; }

    /// <summary>
    /// Liste des lignes de commande (plat + adresse de livraison + heure).
    /// </summary>
    public List<LigneCommande> LignesCommandes { get; set; } = new();

    /// <summary>
    /// Récupère l'adresse d'un utilisateur à partir de son ID.
    /// </summary>
    public static string GetAdresseUser(DatabaseManager db, ulong userId)
    {
        var result = db.ExecuteQuery($"SELECT adresse FROM users WHERE user_id = {userId}");
        return result.Rows.Count > 0 ? result.Rows[0]["adresse"].ToString() : "";
    }

    /// <summary>
    /// Insère cette commande dans la base de données, puis récupère son ID.
    /// </summary>
    public void AjouterCommande(DatabaseManager database)
    {
        string adresseCuisinier = GetAdresseUser(database, CuisinierId ?? 0);

        string query = $@"
        INSERT INTO commandes (
            heure_commande, adresse_depart, prix_total, client_id, cuisinier_id
        ) VALUES (
            '{HeureCommande:yyyy-MM-dd HH:mm:ss}',
            '{adresseCuisinier}',
            {PrixTotal.ToString(System.Globalization.CultureInfo.InvariantCulture)},
            {(ClientId != null ? ClientId.ToString() : "NULL")},
            {(CuisinierId != null ? CuisinierId.ToString() : "NULL")}
        );";

        database.ExecuteNonQuery(query);

        var result = database.ExecuteQuery("SELECT LAST_INSERT_ID() AS id;");
        CommandeId = Convert.ToUInt64(result.Rows[0]["id"]);
    }

    /// <summary>
    /// Met à jour une commande existante dans la base de données.
    /// </summary>
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

    /// <summary>
    /// Supprime cette commande et ses lignes associées de la base de données.
    /// </summary>
    public void SupprimerCommande(DatabaseManager database)
    {
        database.ExecuteNonQuery($"DELETE FROM lignes_commandes WHERE commande_id = {CommandeId}");
        database.ExecuteNonQuery($"DELETE FROM commandes WHERE commande_id = {CommandeId};");
    }

    /// <summary>
    /// Récupère toutes les commandes enregistrées en base.
    /// </summary>
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
                CuisinierId = row["cuisinier_id"] == DBNull.Value ? null : Convert.ToUInt64(row["cuisinier_id"]),
                LignesCommandes = LigneCommande.GetByCommandeId(db, Convert.ToUInt64(row["commande_id"]))
            });
        }

        return commandes;
    }

    /// <summary>
    /// Récupère toutes les commandes associées à un cuisinier donné.
    /// </summary>
    public static List<Commande> GetByCuisinier(DatabaseManager db, ulong cuisinierId)
    {
        var commandes = new List<Commande>();
        var table = db.ExecuteQuery($@"
            SELECT * FROM commandes
            WHERE cuisinier_id = {cuisinierId};");

        foreach (DataRow row in table.Rows)
        {
            commandes.Add(new Commande
            {
                CommandeId = Convert.ToUInt64(row["commande_id"]),
                HeureCommande = Convert.ToDateTime(row["heure_commande"]),
                AdresseDepart = row["adresse_depart"].ToString(),
                PrixTotal = Convert.ToDecimal(row["prix_total"]),
                ClientId = row["client_id"] == DBNull.Value ? null : Convert.ToUInt64(row["client_id"]),
                CuisinierId = row["cuisinier_id"] == DBNull.Value ? null : Convert.ToUInt64(row["cuisinier_id"]),
                LignesCommandes = LigneCommande.GetByCommandeId(db, Convert.ToUInt64(row["commande_id"]))
            });
        }

        return commandes;
    }

    /// <summary>
    /// Récupère toutes les commandes associées à un client donné.
    /// </summary>
    public static List<Commande> GetByClient(DatabaseManager db, ulong clientId)
    {
        var commandes = new List<Commande>();
        var table = db.ExecuteQuery($@"
            SELECT * FROM commandes
            WHERE client_id = {clientId};");

        foreach (DataRow row in table.Rows)
        {
            commandes.Add(new Commande
            {
                CommandeId = Convert.ToUInt64(row["commande_id"]),
                HeureCommande = Convert.ToDateTime(row["heure_commande"]),
                AdresseDepart = row["adresse_depart"]?.ToString(),
                PrixTotal = Convert.ToDecimal(row["prix_total"]),
                ClientId = row["client_id"] == DBNull.Value ? null : Convert.ToUInt64(row["client_id"]),
                CuisinierId = row["cuisinier_id"] == DBNull.Value ? null : Convert.ToUInt64(row["cuisinier_id"]),
                LignesCommandes = LigneCommande.GetByCommandeId(db, Convert.ToUInt64(row["commande_id"]))
            });
        }

        return commandes;
    }
}
