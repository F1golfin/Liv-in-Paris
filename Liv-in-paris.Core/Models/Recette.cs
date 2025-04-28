using System.Data;
namespace Liv_in_paris.Core.Models;

public class Recette
{
    public ulong RecetteId { get; set; }
    public string NomRecette { get; set; }
    public string Type { get; set; } 
    public string Ingredients { get; set; }
    public string StyleCuisine { get; set; } 
    public ulong? ParentRecetteId { get; set; }
    
    public List<ulong> RegimeIds { get; set; } = new();

    public void AjouterRecette(DatabaseManager database)
    {
        string query = $@"
        INSERT INTO recettes (
            nom_recette, type, ingredients, style_cuisine, parent_recette_id
        ) VALUES (
            '{NomRecette}',
            '{Type}',
            '{Ingredients}',
            {StyleCuisine},
            {(ParentRecetteId != null ? ParentRecetteId.ToString() : "NULL")}
        );
    ";
        database.ExecuteNonQuery(query);
        
        ulong recetteId = Convert.ToUInt64(database.ExecuteQuery("SELECT LAST_INSERT_ID();"));
        foreach (ulong regimeId in RegimeIds)
        {
            Respecte.Ajouter(database, recetteId, regimeId);
        }
        
    }

    public void ModifierRecette(DatabaseManager database)
    {
        string query = $@"
            UPDATE recettes SET
                nom_recette = '{NomRecette}',
                type = '{Type}',
                ingredients = '{Ingredients}',
                style_cuisine = {StyleCuisine},
                parent_recette_id = {(ParentRecetteId != null ? ParentRecetteId.ToString() : "NULL")}
            WHERE recette_id = {RecetteId};
        ";

        database.ExecuteNonQuery(query);

        Respecte.SupprimerParRecette(database, RecetteId);
        
        foreach (ulong regimeId in RegimeIds)
        {
            Respecte.Ajouter(database, RecetteId, regimeId);
        }
    }
    
    public void SupprimerRecette(DatabaseManager database)
    {
        Respecte.SupprimerParRecette(database, RecetteId);

        string query = $"DELETE FROM recettes WHERE recette_id = {RecetteId};";
        database.ExecuteNonQuery(query);
    }
    
    public static List<Recette> GetAll(DatabaseManager db)
    {
        var recettes = new List<Recette>();
        var table = db.ExecuteQuery("SELECT * FROM recettes;");

        foreach (DataRow row in table.Rows)
        {
            var recetteId = Convert.ToUInt64(row["recette_id"]);

            var recette = new Recette
            {
                RecetteId = recetteId,
                NomRecette = row["nom_recette"].ToString(),
                Type = row["type"].ToString(),
                Ingredients = row["ingredients"].ToString(),
                StyleCuisine = row["style_cuisine"].ToString(),
                ParentRecetteId = row["parent_recette_id"] == DBNull.Value ? null : Convert.ToUInt64(row["parent_recette_id"]),
                RegimeIds = Respecte.ObtenirRegimesParRecette(db, recetteId)
            };

            recettes.Add(recette);
        }

        return recettes;
    }

    public static Recette GetById(DatabaseManager db, ulong id)
    {
        var table = db.ExecuteQuery($"SELECT * FROM recettes WHERE recette_id = {id} LIMIT 1;");

        if (table.Rows.Count == 0)
            return null;

        var row = table.Rows[0];

        return new Recette
        {
            RecetteId = Convert.ToUInt64(row["recette_id"]),
            NomRecette = row["nom_recette"].ToString(),
            Type = row["type"].ToString(),
            Ingredients = row["ingredients"].ToString(),
            StyleCuisine = row["style_cuisine"].ToString(),
            ParentRecetteId = row["parent_recette_id"] == DBNull.Value ? null : Convert.ToUInt64(row["parent_recette_id"]),
            RegimeIds = Respecte.ObtenirRegimesParRecette(db, Convert.ToUInt64(row["recette_id"]))
        };
    }
}