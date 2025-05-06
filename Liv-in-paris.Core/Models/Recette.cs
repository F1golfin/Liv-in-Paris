using System.Data;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente une recette culinaire, avec ses ingrédients, son type, son style et ses régimes associés.
/// </summary>
public class Recette
{
    /// <summary>Identifiant unique de la recette.</summary>
    public ulong RecetteId { get; set; }

    /// <summary>Nom de la recette.</summary>
    public string NomRecette { get; set; }

    /// <summary>Type de la recette (Entrée, Plat Principal, Dessert...).</summary>
    public string Type { get; set; }

    /// <summary>Ingrédients de la recette, sous forme de chaîne (liste séparée par virgules).</summary>
    public string Ingredients { get; set; }

    /// <summary>Style de cuisine (ex. : asiatique, méditerranéen...).</summary>
    public string StyleCuisine { get; set; }

    /// <summary>Référence à une recette parente (optionnelle), utilisée pour les variantes.</summary>
    public ulong? ParentRecetteId { get; set; }

    /// <summary>Identifiants des régimes alimentaires compatibles avec cette recette.</summary>
    public List<ulong> RegimeIds { get; set; } = new();

    /// <summary>Noms des régimes compatibles avec cette recette (chargés à la lecture).</summary>
    public List<string> RegimesNoms { get; set; } = new();

    /// <summary>
    /// Ajoute la recette à la base de données, ainsi que ses liens avec les régimes alimentaires.
    /// </summary>
    public void AjouterRecette(DatabaseManager database)
    {
        string query = $@"
        INSERT INTO recettes (
            nom_recette, type, ingredients, style_cuisine, parent_recette_id
        ) VALUES (
            '{NomRecette}',
            '{Type}',
            '{Ingredients}',
            '{StyleCuisine}',
            {(ParentRecetteId != null ? ParentRecetteId.ToString() : "NULL")}
        );
    ";

        database.ExecuteNonQuery(query);

        var table = database.ExecuteQuery("SELECT LAST_INSERT_ID();");
        if (table.Rows.Count > 0)
        {
            ulong recetteId = Convert.ToUInt64(table.Rows[0][0]);
            foreach (ulong regimeId in RegimeIds)
            {
                Respecte.Ajouter(database, recetteId, regimeId);
            }
        }
        else
        {
            throw new Exception("Impossible de récupérer l'identifiant de la recette après insertion.");
        }
    }

    /// <summary>
    /// Met à jour les informations de la recette dans la base de données, ainsi que ses régimes associés.
    /// </summary>
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

    /// <summary>Supprime la recette et ses liens avec les régimes alimentaires.</summary>
    public void SupprimerRecette(DatabaseManager database)
    {
        Respecte.SupprimerParRecette(database, RecetteId);

        string query = $"DELETE FROM recettes WHERE recette_id = {RecetteId};";
        database.ExecuteNonQuery(query);
    }

    /// <summary>Récupère toutes les recettes présentes dans la base.</summary>
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
                RegimeIds = Respecte.ObtenirRegimesParRecette(db, recetteId),
                RegimesNoms = Respecte.ObtenirNomsRegimesParRecette(db, recetteId)
            };
            recettes.Add(recette);
        }

        return recettes;
    }

    /// <summary>Récupère une recette spécifique par son identifiant.</summary>
    public static Recette GetById(DatabaseManager db, ulong id)
    {
        var table = db.ExecuteQuery($"SELECT * FROM recettes WHERE recette_id = {id} LIMIT 1;");
        if (table.Rows.Count == 0) return null;

        var row = table.Rows[0];

        return new Recette
        {
            RecetteId = Convert.ToUInt64(row["recette_id"]),
            NomRecette = row["nom_recette"].ToString(),
            Type = row["type"].ToString(),
            Ingredients = row["ingredients"].ToString(),
            StyleCuisine = row["style_cuisine"].ToString(),
            ParentRecetteId = row["parent_recette_id"] == DBNull.Value ? null : Convert.ToUInt64(row["parent_recette_id"]),
            RegimeIds = Respecte.ObtenirRegimesParRecette(db, Convert.ToUInt64(row["recette_id"])),
            RegimesNoms = Respecte.ObtenirNomsRegimesParRecette(db, Convert.ToUInt64(row["recette_id"]))
        };
    }
}
