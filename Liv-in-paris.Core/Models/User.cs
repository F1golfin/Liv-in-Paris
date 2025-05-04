using System.Data;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente un utilisateur du système : client, cuisinier ou administrateur.
/// </summary>
public class User
{
    /// <summary>Identifiant unique de l'utilisateur.</summary>
    public ulong UserId { get; set; }

    /// <summary>Mot de passe de l'utilisateur (en clair, à chiffrer dans un vrai projet).</summary>
    public string Password { get; set; }

    /// <summary>Rôle(s) de l'utilisateur (ex : Client, Cuisinier, ou les deux).</summary>
    public string Role { get; set; }

    /// <summary>Type d'utilisateur (Particulier ou Entreprise).</summary>
    public string Type { get; set; }

    /// <summary>Adresse email de l'utilisateur (identifiant principal pour les particuliers).</summary>
    public string Email { get; set; }

    /// <summary>Nom de l'utilisateur (ou du contact si entreprise).</summary>
    public string Nom { get; set; }

    /// <summary>Prénom de l'utilisateur (ou du contact si entreprise).</summary>
    public string Prenom { get; set; }

    /// <summary>Adresse physique de l'utilisateur.</summary>
    public string Adresse { get; set; }

    /// <summary>Numéro de téléphone (unique).</summary>
    public string Telephone { get; set; }

    /// <summary>Nom de l'entreprise (null pour les particuliers).</summary>
    public string? Entreprise { get; set; }

    /// <summary>Insère un nouvel utilisateur dans la base, après vérification d'unicité email/téléphone.</summary>
    public void CreerUser(DatabaseManager db)
    {
        var checkQuery = $@"
        SELECT 1 FROM users 
        WHERE email = '{Email.Replace("'", "''")}' 
        OR telephone = '{Telephone.Replace("'", "''")}'";

        var result = db.ExecuteQuery(checkQuery);
        if (result.Rows.Count > 0)
        {
            Console.WriteLine($"⚠️ Utilisateur déjà existant : {Email}");
            return;
        }

        string query = $@"
        INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise)
        VALUES (
            '{Password}', '{Role}', '{Type}', '{Email}', '{Nom}', '{Prenom}', '{Adresse}', '{Telephone}',
            {(Entreprise != null ? $"'{Entreprise}'" : "NULL")}
        );";

        try
        {
            Console.WriteLine("Requête SQL : " + query);
            db.ExecuteNonQuery(query);
            Console.WriteLine("Utilisateur inséré !");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur d'insertion utilisateur : " + ex.Message);
        }
    }

    /// <summary>Met à jour les informations de l'utilisateur dans la base.</summary>
    public void ModifierUser(DatabaseManager db)
    {
        string query = $@"
            UPDATE users SET
                password = '{Password}',
                role = '{Role}',
                type = '{Type}',
                email = '{Email}',
                nom = '{Nom}',
                prenom = '{Prenom}',
                adresse = '{Adresse}',
                telephone = '{Telephone}',
                entreprise = {(Entreprise != null ? $"'{Entreprise}'" : "NULL")}
            WHERE user_id = {UserId};";

        db.ExecuteNonQuery(query);
    }

    /// <summary>Supprime l'utilisateur de la base de données (suppression logique ou physique).</summary>
    public void SupprimerUser(DatabaseManager db)
    {
        db.ExecuteNonQuery($"DELETE FROM users WHERE user_id = {UserId};");
    }

    /// <summary>Authentifie un particulier via email et mot de passe.</summary>
    public static User? AuthenticateParEmail(DatabaseManager db, string email, string password)
    {
        string query = $@"
        SELECT * FROM users 
        WHERE email = '{email.Replace("'", "''")}' 
        AND password = '{password.Replace("'", "''")}'
        LIMIT 1;";

        var table = db.ExecuteQuery(query);
        return table.Rows.Count == 0 ? null : HydraterUser(table.Rows[0]);
    }

    /// <summary>Authentifie une entreprise via son nom et mot de passe.</summary>
    public static User? AuthenticateParEntreprise(DatabaseManager db, string entreprise, string password)
    {
        string query = $@"
        SELECT * FROM users 
        WHERE entreprise = '{entreprise.Replace("'", "''")}' 
        AND password = '{password.Replace("'", "''")}'
        LIMIT 1;";

        var table = db.ExecuteQuery(query);
        return table.Rows.Count == 0 ? null : HydraterUser(table.Rows[0]);
    }

    /// <summary>Récupère tous les utilisateurs enregistrés dans la base.</summary>
    public static List<User> GetAllUsers(DatabaseManager db)
    {
        var table = db.ExecuteQuery("SELECT * FROM users;");
        return table.AsEnumerable().Select(HydraterUser).ToList();
    }

    /// <summary>Récupère uniquement les utilisateurs ayant le rôle de cuisinier.</summary>
    public static List<User> GetAllCuisinier(DatabaseManager db)
    {
        var table = db.ExecuteQuery("SELECT * FROM users WHERE role LIKE '%Cuisinier%';");
        return table.AsEnumerable().Select(HydraterUser).ToList();
    }

    /// <summary>Récupère un utilisateur spécifique à partir de son identifiant.</summary>
    public static User? GetById(DatabaseManager db, ulong userId)
    {
        var table = db.ExecuteQuery($"SELECT * FROM users WHERE user_id = {userId} LIMIT 1;");
        return table.Rows.Count == 0 ? null : HydraterUser(table.Rows[0]);
    }

    /// <summary>Hydrate un objet User à partir d'une ligne de résultat SQL.</summary>
    private static User HydraterUser(DataRow row)
    {
        return new User
        {
            UserId = Convert.ToUInt64(row["user_id"]),
            Password = row["password"].ToString(),
            Role = row["role"].ToString(),
            Type = row["type"].ToString(),
            Email = row["email"].ToString(),
            Nom = row["nom"].ToString(),
            Prenom = row["prenom"].ToString(),
            Adresse = row["adresse"].ToString(),
            Telephone = row["telephone"].ToString(),
            Entreprise = row["entreprise"] == DBNull.Value ? null : row["entreprise"].ToString()
        };
    }
}
