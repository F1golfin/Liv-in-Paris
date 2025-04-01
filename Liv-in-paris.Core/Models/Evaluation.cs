using System.Data;
namespace Liv_in_paris.Core.Models;

public class Evaluation
{
    public ulong ClientId { get; set; }
    public ulong CuisinierId { get; set; }
    public int Note { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DateEvaluation { get; set; }

    public void AjouterEvaluation(DatabaseManager database)
    {
        string query = $@"
        INSERT INTO evaluation (
            client_id, cuisinier_id, note, commentaire, date_evaluation
        ) VALUES (
            {ClientId},
            {CuisinierId},
            {Note},
            {(Commentaire != null ? $"'{Commentaire}'" : "NULL")},
            '{DateEvaluation:yyyy-MM-dd HH:mm:ss}'
        );
    ";
        database.ExecuteNonQuery(query);
    }

    public void ModifierEvaluation(DatabaseManager database)
    {
        string query = $@"
        UPDATE evaluation SET
            note = {Note},
            commentaire = {(Commentaire != null ? $"'{Commentaire}'" : "NULL")},
            date_evaluation = '{DateEvaluation:yyyy-MM-dd HH:mm:ss}'
        WHERE client_id = {ClientId} AND cuisinier_id = {CuisinierId};
    ";

        database.ExecuteNonQuery(query);
    }

    public void SupprimerEvaluation(DatabaseManager database)
    {
        string query = $@"
        DELETE FROM evaluation
        WHERE client_id = {ClientId} AND cuisinier_id = {CuisinierId};
    ";

        database.ExecuteNonQuery(query);
    }
    
    public static List<Evaluation> GetAll(DatabaseManager db)
    {
        var evaluations = new List<Evaluation>();
        var table = db.ExecuteQuery("SELECT * FROM evaluation;");

        foreach (DataRow row in table.Rows)
        {
            evaluations.Add(new Evaluation
            {
                ClientId = Convert.ToUInt64(row["client_id"]),
                CuisinierId = Convert.ToUInt64(row["cuisinier_id"]),
                Note = Convert.ToInt32(row["note"]),
                Commentaire = row["commentaire"]?.ToString(),
                DateEvaluation = Convert.ToDateTime(row["date_evaluation"])
            });
        }

        return evaluations;
    }

}