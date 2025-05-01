using System.Data;

namespace Liv_in_paris.Core.Models;

public class Respecte
{
    /// <summary>
    /// Ajoute une liaison recette-régime dans la table `possede`.
    /// </summary>
    public static void Ajouter(DatabaseManager db, ulong recetteId, ulong regimeId)
    {
        string query = $"INSERT INTO respecte (recette_id, regime_id) VALUES ({recetteId}, {regimeId});";
        db.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Supprime toutes les liaisons pour une recette donnée.
    /// </summary>
    public static void SupprimerParRecette(DatabaseManager db, ulong recetteId)
    {
        string query = $"DELETE FROM respecte WHERE recette_id = {recetteId};";
        db.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Récupère tous les ID de régimes associés à une recette.
    /// </summary>
    public static List<ulong> ObtenirRegimesParRecette(DatabaseManager db, ulong recetteId)
    {
        var regimes = new List<ulong>();
        var table = db.ExecuteQuery($"SELECT regime_id FROM respecte WHERE recette_id = {recetteId};");

        foreach (DataRow row in table.Rows)
        {
            regimes.Add(Convert.ToUInt64(row["regime_id"]));
        }

        return regimes;
    }
    
    /// <summary>
    /// Récupère tous les noms des régimes associés à une recette.
    /// </summary>
    public static List<string> ObtenirNomsRegimesParRecette(DatabaseManager db, ulong recetteId)
    {
        var noms = new List<string>();
        var query = @"
        SELECT ra.regime 
        FROM respecte r
        JOIN regime_alimentaire ra ON r.regime_id = ra.regime_id
        WHERE r.recette_id = " + recetteId + ";";

        var table = db.ExecuteQuery(query);
        foreach (DataRow row in table.Rows)
        {
            noms.Add(row["regime"].ToString());
        }

        return noms;
    }
}