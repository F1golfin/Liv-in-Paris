using System.Data;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente un plat proposé par un cuisinier, issu d'une recette.
/// </summary>
public class Plat
{
    /// <summary>Identifiant unique du plat.</summary>
    public ulong PlatId { get; set; }

    /// <summary>Nom du plat.</summary>
    public string NomPlat { get; set; }

    /// <summary>Nombre de parts disponibles pour ce plat.</summary>
    public int NbParts { get; set; }

    /// <summary>Date de fabrication du plat.</summary>
    public DateTime DateFabrication { get; set; }

    /// <summary>Date limite de consommation du plat.</summary>
    public DateTime DatePeremption { get; set; }

    /// <summary>Prix par personne en euros.</summary>
    public decimal PrixParPersonne { get; set; }

    /// <summary>Image du plat, stockée en tant que tableau de bytes (optionnelle).</summary>
    public byte[]? Photo { get; set; }

    /// <summary>Identifiant du cuisinier ayant préparé ce plat.</summary>
    public ulong CuisinierId { get; set; }

    /// <summary>Identifiant de la recette utilisée.</summary>
    public ulong RecetteId { get; set; }

    /// <summary>Recette associée à ce plat.</summary>
    public Recette Recette { get; set; }

    /// <summary>Indique si le plat est récent (moins de 2 jours).</summary>
    public bool EstNouveau => (DateTime.Now - DateFabrication).TotalDays < 2;

    /// <summary>Récupère un plat par son identifiant.</summary>
    public static Plat? GetById(DatabaseManager db, ulong platId)
    {
        var table = db.ExecuteQuery($"SELECT * FROM plats WHERE plat_id = {platId} LIMIT 1;");
        if (table.Rows.Count == 0) return null;

        var row = table.Rows[0];
        Recette recette = Recette.GetById(db, Convert.ToUInt64(row["recette_id"]));
        return new Plat
        {
            PlatId = Convert.ToUInt64(row["plat_id"]),
            NomPlat = row["nom_plat"].ToString(),
            NbParts = Convert.ToInt32(row["nb_parts"]),
            DateFabrication = Convert.ToDateTime(row["date_fabrication"]),
            DatePeremption = Convert.ToDateTime(row["date_peremption"]),
            PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]),
            Photo = row["photo"] == DBNull.Value ? null : (byte[])row["photo"],
            CuisinierId = Convert.ToUInt64(row["cuisinier_id"]),
            RecetteId = Convert.ToUInt64(row["recette_id"]),
            Recette = recette
        };
    }

    /// <summary>Ajoute ce plat à la base de données.</summary>
    public void AjouterPlat(DatabaseManager database)
    {
        string query = $@"
            INSERT INTO plats (
                nom_plat, nb_parts, date_fabrication, date_peremption, prix_par_personne, photo,
                cuisinier_id, recette_id
            ) VALUES (
                '{NomPlat}',
                {NbParts},
                '{DateFabrication:yyyy-MM-dd}',
                '{DatePeremption:yyyy-MM-dd}',
                {PrixParPersonne.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                NULL,
                {CuisinierId},
                {RecetteId}
            );";
        database.ExecuteNonQuery(query);
    }

    /// <summary>Met à jour les informations du plat dans la base de données.</summary>
    public void ModifierPlat(DatabaseManager database)
    {
        string query = $@"
            UPDATE plats SET
                nom_plat = '{NomPlat}',
                nb_parts = {NbParts},
                date_fabrication = '{DateFabrication:yyyy-MM-dd}',
                date_peremption = '{DatePeremption:yyyy-MM-dd}',
                prix_par_personne = {PrixParPersonne.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                photo = NULL,
                cuisinier_id = {CuisinierId},
                recette_id = {RecetteId}
            WHERE plat_id = {PlatId};";
        database.ExecuteNonQuery(query);
    }

    /// <summary>Supprime ce plat de la base de données.</summary>
    public void SupprimerPlat(DatabaseManager database)
    {
        string query = $"DELETE FROM plats WHERE plat_id = {PlatId};";
        database.ExecuteNonQuery(query);
    }

    /// <summary>Retourne tous les plats de la base.</summary>
    public static List<Plat> GetAll(DatabaseManager db)
    {
        var plats = new List<Plat>();
        var table = db.ExecuteQuery("SELECT * FROM plats;");
        foreach (DataRow row in table.Rows)
        {
            plats.Add(CreateFromRow(db, row));
        }
        return plats;
    }

    /// <summary>Retourne tous les plats créés par un cuisinier donné.</summary>
    public static List<Plat> GetAllByCuisinier(DatabaseManager db, ulong cuisinierId)
    {
        var plats = new List<Plat>();
        var table = db.ExecuteQuery($"SELECT * FROM plats WHERE cuisinier_id = {cuisinierId};");
        foreach (DataRow row in table.Rows)
        {
            plats.Add(CreateFromRow(db, row));
        }
        return plats;
    }

    /// <summary>Retourne tous les plats disponibles (pas encore dans une ligne de commande).</summary>
    public static List<Plat> GetDisponibles(DatabaseManager db, ulong? cuisinierId = null)
    {
        string query = @"
            SELECT * FROM plats 
            WHERE plat_id NOT IN (SELECT plat_id FROM lignes_commandes)";
        if (cuisinierId != null)
            query += $" AND cuisinier_id = {cuisinierId}";

        var table = db.ExecuteQuery(query);
        var plats = new List<Plat>();
        foreach (DataRow row in table.Rows)
        {
            plats.Add(CreateFromRow(db, row));
        }
        return plats;
    }

    /// <summary>Retourne les plats associés à une commande spécifique.</summary>
    public static List<Plat> GetByCommandeId(DatabaseManager db, ulong commandeId)
    {
        var plats = new List<Plat>();
        string query = $@"
            SELECT p.*
            FROM plats p
            JOIN lignes_commandes lc ON lc.plat_id = p.plat_id
            WHERE lc.commande_id = {commandeId};";

        var table = db.ExecuteQuery(query);
        foreach (DataRow row in table.Rows)
        {
            plats.Add(CreateFromRow(db, row));
        }
        return plats;
    }

    /// <summary>
    /// Construit un objet <see cref="Plat"/> à partir d’une ligne de la base.
    /// </summary>
    private static Plat CreateFromRow(DatabaseManager db, DataRow row)
    {
        Recette recette = Recette.GetById(db, Convert.ToUInt64(row["recette_id"]));
        return new Plat
        {
            PlatId = Convert.ToUInt64(row["plat_id"]),
            NomPlat = row["nom_plat"].ToString(),
            NbParts = Convert.ToInt32(row["nb_parts"]),
            DateFabrication = Convert.ToDateTime(row["date_fabrication"]),
            DatePeremption = Convert.ToDateTime(row["date_peremption"]),
            PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]),
            Photo = row["photo"] == DBNull.Value ? null : (byte[])row["photo"],
            CuisinierId = Convert.ToUInt64(row["cuisinier_id"]),
            RecetteId = Convert.ToUInt64(row["recette_id"]),
            Recette = recette
        };
    }
}
