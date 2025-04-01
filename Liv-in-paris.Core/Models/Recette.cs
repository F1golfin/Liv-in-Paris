using System.Data;
namespace Liv_in_paris.Core.Models;

public class Recette
{
    public ulong RecetteId { get; set; }
    public string NomRecette { get; set; }
    public string Type { get; set; } 
    public string Ingredients { get; set; }
    public int StyleCuisine { get; set; } 
    public string? RegimeAlimentaire { get; set; }
    public ulong? ParentRecetteId { get; set; }

    public void AjouterRecette(DatabaseManager database)
    {
        string query = $@"
        INSERT INTO recettes (
            nom_recette, type, ingredients, style_cuisine, regime_alimentaire, parent_recette_id
        ) VALUES (
            '{NomRecette}',
            '{Type}',
            '{Ingredients}',
            {StyleCuisine},
            {(RegimeAlimentaire != null ? $"'{RegimeAlimentaire}'" : "NULL")},
            {(ParentRecetteId != null ? ParentRecetteId.ToString() : "NULL")}
        );
    ";
        database.ExecuteNonQuery(query);
    }

    public void ModifierRecette(DatabaseManager database)
    {
        string query = $@"
            UPDATE users SET
               nom_recette = '{NomRecette}',
               type = '{Type}',
               ingredients = '{Ingredients}',
               style_cuisine = {StyleCuisine},
               regime_alimentaire = '{RegimeAlimentaire}',
               parent_recette_id = {ParentRecetteId},
            WHERE recette_id = {RecetteId};
        ";
        database.ExecuteNonQuery(query);
    }
    public void SupprimerRecette(DatabaseManager database)
    {
        string query = $"DELETE FROM recettes WHERE recette_id = {RecetteId};";
        database.ExecuteNonQuery(query);
    }
    
    public static List<Recette> GetAll(DatabaseManager db)
    {
        var recettes = new List<Recette>();
        var table = db.ExecuteQuery("SELECT * FROM recettes;");

        foreach (DataRow row in table.Rows)
        {
            recettes.Add(new Recette
            {
                RecetteId = Convert.ToUInt64(row["recette_id"]),
                NomRecette = row["nom_recette"].ToString(),
                Type = row["type"].ToString(),
                Ingredients = row["ingredients"].ToString(),
                StyleCuisine = Convert.ToInt32(row["style_cuisine"]),
                RegimeAlimentaire = row["regime_alimentaire"]?.ToString(),
                ParentRecetteId = row["parent_recette_id"] == DBNull.Value ? null : Convert.ToUInt64(row["parent_recette_id"])
            });
        }

        return recettes;
    }

    

}