using System.Data;
namespace Liv_in_paris.Core.Models;

public class Plat
{
    public ulong PlatId { get; set; }
    public string NomPlat { get; set; }
    public int NbParts { get; set; }
    public DateTime DateFabrication { get; set; }
    public DateTime DatePeremption { get; set; }
    public decimal PrixParPersonne { get; set; }
    public byte[]? Photo { get; set; } // optionnel
    public ulong CuisinierId { get; set; }
    public ulong RecetteId { get; set; }
    public Recette Recette { get; set; }

    
    public static Plat? GetById(DatabaseManager db, ulong platId)
    {
        var table = db.ExecuteQuery($"SELECT * FROM plats WHERE plat_id = {platId} LIMIT 1;");

        if (table.Rows.Count == 0)
            return null;

        Recette recette = Recette.GetById(db, Convert.ToUInt64(table.Rows[0]["recette_id"]));
        return new Plat
        {
            PlatId = Convert.ToUInt64(table.Rows[0]["plat_id"]),
            NomPlat = table.Rows[0]["nom_plat"].ToString(),
            NbParts = Convert.ToInt32(table.Rows[0]["nb_parts"]),
            DateFabrication = Convert.ToDateTime(table.Rows[0]["date_fabrication"]),
            DatePeremption = Convert.ToDateTime(table.Rows[0]["date_peremption"]),
            PrixParPersonne = Convert.ToDecimal(table.Rows[0]["prix_par_personne"]),
            Photo = table.Rows[0]["photo"] == DBNull.Value ? null : (byte[])table.Rows[0]["photo"],
            CuisinierId = Convert.ToUInt64(table.Rows[0]["cuisinier_id"]),
            RecetteId = Convert.ToUInt64(table.Rows[0]["recette_id"]),
            Recette = recette
        };
    }
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
                NULL, -- à remplacer si tu gères les photos
                {CuisinierId},
                {RecetteId}
            );";

        database.ExecuteNonQuery(query);
    }

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

    public void SupprimerPlat(DatabaseManager database)
    {
        string query = $"DELETE FROM plats WHERE plat_id = {PlatId};";
        database.ExecuteNonQuery(query);
    }
    
    public static List<Plat> GetAll(DatabaseManager db)
    {
        var plats = new List<Plat>();
        var table = db.ExecuteQuery("SELECT * FROM plats;");

        foreach (DataRow row in table.Rows)
        {
            Recette recette = Recette.GetById(db, Convert.ToUInt64(row["recette_id"]));

            plats.Add(new Plat
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
            });
        }

        return plats;
    }
    
    public static List<Plat> GetAllByCuisinier(DatabaseManager db, ulong cuisinierId)
    {
        var plats = new List<Plat>();
        var table = db.ExecuteQuery($"SELECT * FROM plats WHERE cuisinier_id = {cuisinierId};");

        foreach (DataRow row in table.Rows)
        {
            Recette recette = Recette.GetById(db, Convert.ToUInt64(row["recette_id"]));

            Plat plat = new Plat
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
            plats.Add(plat);
        }

        return plats;
    }
    
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
            Recette recette = Recette.GetById(db, Convert.ToUInt64(row["recette_id"]));
            plats.Add(new Plat
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
            });
        }

        return plats;
    }
    
    public static List<Plat> GetByCommandeId(DatabaseManager db, ulong commandeId)
    {
        List<Plat> plats = new List<Plat>();
        string query = $@"
            SELECT p.*
            FROM plats p
            JOIN lignes_commandes lc ON lc.plat_id = p.plat_id
            WHERE lc.commande_id = {commandeId};";
        var table = db.ExecuteQuery(query);

        foreach (DataRow row in table.Rows)
        {
            Recette recette = Recette.GetById(db, Convert.ToUInt64(row["recette_id"]));

            plats.Add(new Plat
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
            });
        }

        return plats;
    }
    
}