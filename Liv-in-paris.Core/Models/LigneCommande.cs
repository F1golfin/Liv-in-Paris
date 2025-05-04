using System.Data;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente une ligne de commande, c’est-à-dire un plat commandé dans une commande donnée,
/// avec ses informations spécifiques (adresse, heure, statut).
/// </summary>
public class LigneCommande
{
    /// <summary>
    /// Identifiant unique de la ligne de commande.
    /// </summary>
    public ulong LigneCommandeId { get; set; }

    /// <summary>
    /// Heure prévue de la livraison.
    /// </summary>
    public DateTime? HeureLivraison { get; set; }

    /// <summary>
    /// Adresse de livraison du plat.
    /// </summary>
    public string? AdresseArrivee { get; set; }

    /// <summary>
    /// Statut de la ligne de commande (ex. : Commandee, Preparee, Livree...).
    /// </summary>
    public string Statut { get; set; }

    /// <summary>
    /// Identifiant de la commande à laquelle cette ligne est rattachée.
    /// Peut être null si la ligne est encore dans le panier.
    /// </summary>
    public ulong? CommandeId { get; set; }

    /// <summary>
    /// Identifiant du plat concerné.
    /// </summary>
    public ulong PlatId { get; set; }

    /// <summary>
    /// Objet Plat associé (chargé via GetById).
    /// </summary>
    public Plat? Plat { get; set; }

    /// <summary>
    /// Liste des statuts disponibles pour l’interface (ComboBox).
    /// </summary>
    public List<string> StatutsDisponibles => new() { "Panier", "Commandee", "Preparee", "En cours", "Livree", "Annulee" };

    /// <summary>
    /// Insère cette ligne dans la base avec toutes les informations complètes.
    /// </summary>
    public void AjouterCommande(DatabaseManager database)
    {
        string query = $@"
            INSERT INTO lignes_commandes (
                heure_livraison, adresse_arrivee, statut, commande_id, plat_id
            ) VALUES (
                '{HeureLivraison:yyyy-MM-dd HH:mm:ss}',
                '{AdresseArrivee}',
                '{Statut}',
                {CommandeId},
                {PlatId}
            );";

        database.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Insère cette ligne dans la base sans horaire ni adresse (panier temporaire).
    /// </summary>
    public void AjouterCommande_tps(DatabaseManager db)
    {
        string query = $@"
        INSERT INTO lignes_commandes (
            heure_livraison, adresse_arrivee, statut, commande_id, plat_id
        ) VALUES (
            NULL,
            NULL,
            '{Statut}',
            NULL,
            {PlatId}
        );";

        db.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Met à jour cette ligne dans la base avec ses nouvelles informations.
    /// </summary>
    public void ModifierCommande(DatabaseManager database)
    {
        string query = $@"
        UPDATE lignes_commandes SET
            heure_livraison = '{HeureLivraison:yyyy-MM-dd HH:mm:ss}',
            adresse_arrivee = '{AdresseArrivee}',
            statut = '{Statut}',
            commande_id = {CommandeId}
        WHERE plat_id = {PlatId};";

        database.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Supprime cette ligne de commande de la base.
    /// </summary>
    public void SupprimerCommande(DatabaseManager db)
    {
        string query = $"DELETE FROM lignes_commandes WHERE ligne_commande_id = {LigneCommandeId};";
        db.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Supprime toutes les lignes correspondant à un plat donné,
    /// uniquement si elles ne sont pas encore rattachées à une commande.
    /// </summary>
    public static void SupprimerParPlatId(DatabaseManager db, ulong platId)
    {
        string query = $"DELETE FROM lignes_commandes WHERE plat_id = {platId} AND commande_id IS NULL;";
        db.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Récupère toutes les lignes de commandes de la base.
    /// </summary>
    public static List<LigneCommande> GetAll(DatabaseManager db)
    {
        var lignes = new List<LigneCommande>();
        var table = db.ExecuteQuery("SELECT * FROM lignes_commandes;");

        foreach (DataRow row in table.Rows)
        {
            ulong platId = Convert.ToUInt64(row["plat_id"]);
            lignes.Add(new LigneCommande
            {
                LigneCommandeId = Convert.ToUInt64(row["ligne_commande_id"]),
                HeureLivraison = Convert.ToDateTime(row["heure_livraison"]),
                AdresseArrivee = row["adresse_arrivee"].ToString(),
                Statut = row["statut"].ToString(),
                CommandeId = Convert.ToUInt64(row["commande_id"]),
                PlatId = platId,
                Plat = Plat.GetById(db, platId)
            });
        }

        return lignes;
    }

    /// <summary>
    /// Récupère toutes les lignes associées à une commande donnée.
    /// </summary>
    public static List<LigneCommande> GetByCommandeId(DatabaseManager db, ulong commandeId)
    {
        var lignes = new List<LigneCommande>();
        var table = db.ExecuteQuery($"SELECT * FROM lignes_commandes WHERE commande_id = {commandeId};");

        foreach (DataRow row in table.Rows)
        {
            ulong platId = Convert.ToUInt64(row["plat_id"]);
            lignes.Add(new LigneCommande
            {
                LigneCommandeId = Convert.ToUInt64(row["ligne_commande_id"]),
                HeureLivraison = Convert.ToDateTime(row["heure_livraison"]),
                AdresseArrivee = row["adresse_arrivee"].ToString(),
                Statut = row["statut"].ToString(),
                CommandeId = Convert.ToUInt64(row["commande_id"]),
                PlatId = platId,
                Plat = Plat.GetById(db, platId)
            });
        }

        return lignes;
    }

    /// <summary>
    /// Récupère la ligne associée à un plat (supposée unique ou en cours).
    /// </summary>
    public static LigneCommande GetByPlatId(DatabaseManager db, ulong platId)
    {
        var table = db.ExecuteQuery($@"
        SELECT * FROM lignes_commandes
        WHERE plat_id = {platId}
        LIMIT 1;");

        var row = table.Rows[0];

        return new LigneCommande
        {
            LigneCommandeId = row["ligne_commande_id"] == DBNull.Value ? 0 : Convert.ToUInt64(row["ligne_commande_id"]),
            HeureLivraison = row["heure_livraison"] == DBNull.Value ? null : Convert.ToDateTime(row["heure_livraison"]),
            AdresseArrivee = row["adresse_arrivee"] == DBNull.Value ? null : row["adresse_arrivee"].ToString(),
            Statut = row["statut"] == DBNull.Value ? null : row["statut"].ToString(),
            CommandeId = row["commande_id"] == DBNull.Value ? null : Convert.ToUInt64(row["commande_id"]),
            PlatId = platId
        };
    }

    /// <summary>
    /// Met à jour uniquement le statut d'une ligne de commande (utilisé par les cuisiniers).
    /// </summary>
    public void MettreAJourStatut(DatabaseManager db, string nouveauStatut)
    {
        var query = $"UPDATE lignes_commandes SET statut = '{nouveauStatut}' WHERE ligne_commande_id = {LigneCommandeId};";
        db.ExecuteNonQuery(query);
        Statut = nouveauStatut;
    }
}
