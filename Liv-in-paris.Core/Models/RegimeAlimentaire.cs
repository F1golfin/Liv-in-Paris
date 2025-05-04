using System.Data;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente un régime alimentaire (ex. : végétarien, sans gluten...).
/// </summary>
public class RegimeAlimentaire
{
    /// <summary>Identifiant unique du régime alimentaire.</summary>
    public ulong RegimeId { get; set; }

    /// <summary>Nom du régime (végétarien, vegan, etc.).</summary>
    public string Regime { get; set; }

    /// <summary>Ajoute un nouveau régime alimentaire à la base de données.</summary>
    public void Ajouter(DatabaseManager db)
    {
        string query = $@"
            INSERT INTO regime_alimentaire (regime)
            VALUES ('{Regime}');
        ";
        db.ExecuteNonQuery(query);
    }

    /// <summary>Supprime ce régime alimentaire de la base de données.</summary>
    public void Supprimer(DatabaseManager db)
    {
        string query = $"DELETE FROM regime_alimentaire WHERE regime_id = {RegimeId};";
        db.ExecuteNonQuery(query);
    }

    /// <summary>Modifie le nom de ce régime dans la base de données.</summary>
    public void Modifier(DatabaseManager db)
    {
        string query = $@"
            UPDATE regime_alimentaire
            SET regime = '{Regime}'
            WHERE regime_id = {RegimeId};
        ";
        db.ExecuteNonQuery(query);
    }

    /// <summary>Retourne la liste de tous les régimes alimentaires enregistrés.</summary>
    public static List<RegimeAlimentaire> GetAll(DatabaseManager db)
    {
        var result = new List<RegimeAlimentaire>();
        var table = db.ExecuteQuery("SELECT * FROM regime_alimentaire;");

        foreach (DataRow row in table.Rows)
        {
            result.Add(new RegimeAlimentaire
            {
                RegimeId = Convert.ToUInt64(row["regime_id"]),
                Regime = row["regime"].ToString()
            });
        }

        return result;
    }

    /// <summary>Recherche un régime alimentaire par son identifiant.</summary>
    /// <param name="db">Instance de connexion à la base.</param>
    /// <param name="id">Identifiant du régime recherché.</param>
    /// <returns>Le régime correspondant, ou null s'il n'existe pas.</returns>
    public static RegimeAlimentaire? GetById(DatabaseManager db, ulong id)
    {
        var table = db.ExecuteQuery($"SELECT * FROM regime_alimentaire WHERE regime_id = {id} LIMIT 1;");

        if (table.Rows.Count == 0)
            return null;

        var row = table.Rows[0];

        return new RegimeAlimentaire
        {
            RegimeId = Convert.ToUInt64(row["regime_id"]),
            Regime = row["regime"].ToString()
        };
    }
}
