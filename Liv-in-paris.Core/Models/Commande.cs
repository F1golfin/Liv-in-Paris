using System.Data;

namespace Liv_in_paris.Core.Models
{
    public class Commande
    {
        public ulong CommandeId { get; set; }
        public DateTime HeureCommande { get; set; }
        public string AdresseDepart { get; set; }
        public decimal PrixTotal { get; set; }
        public ulong? ClientId { get; set; }
        public ulong? CuisinierId { get; set; }
        public string AdresseArrivee { get; set; }
        public List<Plat> Plats { get; set; } = new();

        public static string GetAdresseUser(DatabaseManager db, ulong userId)
        {
            var result = db.ExecuteQuery($"SELECT adresse FROM users WHERE user_id = {userId}");
            return result.Rows.Count > 0 ? result.Rows[0]["adresse"].ToString() : "";
        }

        public void AjouterCommande(DatabaseManager database)
        {
            // Récupère automatiquement l'adresse du cuisinier
            string adresseCuisinier = GetAdresseUser(database, CuisinierId ?? 0);

            string query = $@"
        INSERT INTO commandes (
            heure_commande, adresse_depart, prix_total, client_id, cuisinier_id
        ) VALUES (
            '{HeureCommande:yyyy-MM-dd HH:mm:ss}',
            '{adresseCuisinier}',
            {PrixTotal.ToString(System.Globalization.CultureInfo.InvariantCulture)},
            {(ClientId != null ? ClientId.ToString() : "NULL")},
            {(CuisinierId != null ? CuisinierId.ToString() : "NULL")}
        );
    ";

            database.ExecuteNonQuery(query);
        }


        public void ModifierCommande(DatabaseManager database)
        {
            string query = $@"
                UPDATE commandes SET
                    heure_commande = '{HeureCommande:yyyy-MM-dd HH:mm:ss}',
                    adresse_depart = '{AdresseDepart}',
                    prix_total = {PrixTotal.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                    client_id = {(ClientId != null ? ClientId.ToString() : "NULL")},
                    cuisinier_id = {(CuisinierId != null ? CuisinierId.ToString() : "NULL")}
                WHERE commande_id = {CommandeId};
            ";

            database.ExecuteNonQuery(query);
        }

        public void SupprimerCommande(DatabaseManager database)
        {
            string query = $"DELETE FROM commandes WHERE commande_id = {CommandeId};";
            database.ExecuteNonQuery(query);
        }

        public static List<Commande> GetAll(DatabaseManager db)
        {
            var commandes = new List<Commande>();
            var table = db.ExecuteQuery("SELECT * FROM commandes;");

            foreach (DataRow row in table.Rows)
            {
                commandes.Add(new Commande
                {
                    CommandeId = Convert.ToUInt64(row["commande_id"]),
                    HeureCommande = Convert.ToDateTime(row["heure_commande"]),
                    AdresseDepart = row["adresse_depart"].ToString(),
                    PrixTotal = Convert.ToDecimal(row["prix_total"]),
                    ClientId = row["client_id"] == DBNull.Value ? null : Convert.ToUInt64(row["client_id"]),
                    CuisinierId = row["cuisinier_id"] == DBNull.Value ? null : Convert.ToUInt64(row["cuisinier_id"])
                });
            }

            return commandes;
        }

        public static List<Commande> GetByCuisinier(DatabaseManager db, ulong cuisinierId)
        {
            var commandes = new List<Commande>();

            var table = db.ExecuteQuery($@"
                SELECT 
                    c.*,
                    cli.adresse AS adresse_client,
                    cuis.adresse AS adresse_cuisinier
                FROM commandes c
                LEFT JOIN users cli ON c.client_id = cli.user_id
                LEFT JOIN users cuis ON c.cuisinier_id = cuis.user_id
                WHERE c.cuisinier_id = {cuisinierId};
            ");

            foreach (DataRow row in table.Rows)
            {
                var commande = new Commande
                {
                    CommandeId = Convert.ToUInt64(row["commande_id"]),
                    HeureCommande = Convert.ToDateTime(row["heure_commande"]),
                    AdresseDepart = row["adresse_cuisinier"]?.ToString(),  // 👍 cuisinier
                    AdresseArrivee = row["adresse_client"]?.ToString(),     // 👍 client
                    PrixTotal = Convert.ToDecimal(row["prix_total"]),
                    ClientId = row["client_id"] == DBNull.Value ? null : Convert.ToUInt64(row["client_id"]),
                    CuisinierId = row["cuisinier_id"] == DBNull.Value ? null : Convert.ToUInt64(row["cuisinier_id"]),
                    Plats = Plat.GetByCommandeId(db, Convert.ToUInt64(row["commande_id"]))
                };

                commandes.Add(commande);
            }

            return commandes;
        }
    }
}
