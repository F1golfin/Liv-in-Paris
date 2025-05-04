using System.Data;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente une évaluation d'un cuisinier faite par un client.
/// Une seule évaluation est autorisée par paire client-cuisinier (clé primaire composite).
/// </summary>
public class Evaluation
{
    /// <summary>
    /// Identifiant du client ayant rédigé l’évaluation.
    /// </summary>
    public ulong ClientId { get; set; }

    /// <summary>
    /// Identifiant du cuisinier évalué.
    /// </summary>
    public ulong CuisinierId { get; set; }

    /// <summary>
    /// Note donnée par le client (sur 5).
    /// </summary>
    public int Note { get; set; }

    /// <summary>
    /// Commentaire laissé par le client (optionnel).
    /// </summary>
    public string? Commentaire { get; set; }

    /// <summary>
    /// Date de l’évaluation.
    /// </summary>
    public DateTime DateEvaluation { get; set; }

    /// <summary>
    /// Insère ou met à jour une évaluation client/cuisinier dans la base.
    /// </summary>
    /// <param name="db">Gestionnaire de base de données.</param>
    public void Enregistrer(DatabaseManager db)
    {
        string query = $@"
        INSERT INTO evaluation (
            client_id, cuisinier_id, note, commentaire, date_evaluation
        ) VALUES (
            {ClientId},
            {CuisinierId},
            {Note},
            {(Commentaire != null ? $"'{Commentaire.Replace("'", "''")}'" : "NULL")},
            '{DateEvaluation:yyyy-MM-dd HH:mm:ss}'
        )
        ON DUPLICATE KEY UPDATE
            note = {Note},
            commentaire = {(Commentaire != null ? $"'{Commentaire.Replace("'", "''")}'" : "NULL")},
            date_evaluation = '{DateEvaluation:yyyy-MM-dd HH:mm:ss}';";

        db.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Supprime une évaluation existante entre un client et un cuisinier.
    /// </summary>
    /// <param name="database">Gestionnaire de base de données.</param>
    public void SupprimerEvaluation(DatabaseManager database)
    {
        string query = $@"
        DELETE FROM evaluation
        WHERE client_id = {ClientId} AND cuisinier_id = {CuisinierId};";

        database.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Récupère toutes les évaluations présentes dans la base de données.
    /// </summary>
    /// <param name="db">Gestionnaire de base de données.</param>
    /// <returns>Liste de toutes les évaluations.</returns>
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

    /// <summary>
    /// Récupère toutes les évaluations d’un cuisinier donné.
    /// </summary>
    /// <param name="db">Gestionnaire de base de données.</param>
    /// <param name="cuisinierId">Identifiant du cuisinier évalué.</param>
    /// <returns>Liste des évaluations reçues.</returns>
    public static List<Evaluation> GetByCuisinier(DatabaseManager db, ulong cuisinierId)
    {
        var evaluations = new List<Evaluation>();
        var table = db.ExecuteQuery($@"
            SELECT * FROM evaluation
            WHERE cuisinier_id = {cuisinierId};");

        foreach (DataRow row in table.Rows)
        {
            evaluations.Add(new Evaluation
            {
                ClientId = Convert.ToUInt64(row["client_id"]),
                CuisinierId = cuisinierId,
                Note = Convert.ToInt32(row["note"]),
                Commentaire = row["commentaire"]?.ToString(),
                DateEvaluation = Convert.ToDateTime(row["date_evaluation"])
            });
        }

        return evaluations;
    }
}
