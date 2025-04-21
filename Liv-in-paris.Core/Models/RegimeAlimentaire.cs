using System.Data;

namespace Liv_in_paris.Core.Models;

public class RegimeAlimentaire
{
    public ulong RegimeId { get; set; }
    public string Regime { get; set; }

    public void Ajouter(DatabaseManager db)
    {
        string query = $@"
            INSERT INTO regime_alimentaire (regime)
            VALUES ('{Regime}');
        ";

        db.ExecuteNonQuery(query);
    }

    public void Supprimer(DatabaseManager db)
    {
        string query = $"DELETE FROM regime_alimentaire WHERE regime_id = {RegimeId};";
        db.ExecuteNonQuery(query);
    }

    public void Modifier(DatabaseManager db)
    {
        string query = $@"
            UPDATE regime_alimentaire
            SET regime = '{Regime}'
            WHERE regime_id = {RegimeId};
        ";

        db.ExecuteNonQuery(query);
    }

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