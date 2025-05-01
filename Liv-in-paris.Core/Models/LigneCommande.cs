using System.Data;
namespace Liv_in_paris.Core.Models;

public class LigneCommande
{
    public ulong LigneCommandeId { get; set; }
    public DateTime? HeureLivraison { get; set; }
    public string? AdresseArrivee { get; set; }
    public string Statut { get; set; }
    public ulong? CommandeId { get; set; }
    public ulong PlatId { get; set; }
    
    public Plat? Plat { get; set; }

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
    
    public void SupprimerCommande(DatabaseManager db)
    {
        string query = $"DELETE FROM lignes_commandes WHERE ligne_commande_id = {LigneCommandeId};";
        db.ExecuteNonQuery(query);
    }
    
    public static void SupprimerParPlatId(DatabaseManager db, ulong platId)
    {
        string query = $"DELETE FROM lignes_commandes WHERE plat_id = {platId} AND commande_id IS NULL;";
        db.ExecuteNonQuery(query);
    }
    
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
    
    public static LigneCommande GetByPlatId(DatabaseManager db, ulong platId)
    {
        var table = db.ExecuteQuery($@"
        SELECT * FROM lignes_commandes
        WHERE plat_id = {platId}
        LIMIT 1;
    ");
        

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
    
    public void ChangerStatut(DatabaseManager db, string nouveauStatut)
    {
        string query = $@"
        UPDATE lignes_commandes
        SET statut = '{nouveauStatut}'
        WHERE plat_id = {PlatId};";

        db.ExecuteNonQuery(query);
    }
    
}