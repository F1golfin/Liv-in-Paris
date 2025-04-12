using System.Data;
namespace Liv_in_paris.Core.Models;

/// <summary>
/// Représente un utilisateur de la plateforme Liv'in Paris, pouvant être client, cuisinier, ou les deux.
/// </summary>
public class User
{
    /// <summary>
    /// Identifiant unique de l'utilisateur.
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Mot de passe de l'utilisateur.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Rôle de l'utilisateur (par exemple : client, cuisinier).
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Type de l'utilisateur (par exemple : particulier, professionnel).
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Adresse email de l'utilisateur.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Nom de l'utilisateur.
    /// </summary>
    public string Nom { get; set; }

    /// <summary>
    /// Prénom de l'utilisateur.
    /// </summary>
    public string Prenom { get; set; }

    /// <summary>
    /// Adresse postale de l'utilisateur.
    /// </summary>
    public string Adresse { get; set; }

    /// <summary>
    /// Numéro de téléphone de l'utilisateur.
    /// </summary>
    public string Telephone { get; set; }

    /// <summary>
    /// Nom de l'entreprise si l'utilisateur est un professionnel. Peut être null.
    /// </summary>
    public string? Entreprise { get; set; }

    /// <summary>
    /// Insère un nouvel utilisateur dans la base de données.
    /// </summary>
    /// <param name="db">Instance de <see cref="DatabaseManager"/> utilisée pour exécuter la requête.</param>
    public void CreerUser(DatabaseManager db)
    {
        string query = $@"
        INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise)
        VALUES (
            '{Password}',
            '{Role}',
            '{Type}',
            '{Email}',
            '{Nom}',
            '{Prenom}',
            '{Adresse}',
            '{Telephone}',
            {(Entreprise != null ? $"'{Entreprise}'" : "NULL")}
        );";

        try
        {
            Console.WriteLine("Requête SQL : " + query);
            db.ExecuteNonQuery(query);
            Console.WriteLine("✅ Utilisateur inséré !");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Erreur d'insertion utilisateur : " + ex.Message);
        }
    }
    
    public static User? Authenticate(DatabaseManager db, string prenom, string password)
    {
        string query = $@"
        SELECT * FROM users 
        WHERE prenom = '{prenom.Replace("'", "''")}' 
        AND password = '{password.Replace("'", "''")}' 
        LIMIT 1;
    ";

        var table = db.ExecuteQuery(query);
        if (table.Rows.Count == 0) return null;
        
        DataRow row = table.Rows[0];
        User user = new User
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
            Entreprise = row["entreprise"]?.ToString()
        };

        return user;
    }

    /// <summary>
    /// Met à jour les informations de l'utilisateur dans la base de données.
    /// </summary>
    /// <param name="database">Instance de <see cref="DatabaseManager"/> utilisée pour exécuter la requête.</param>
    public void ModifierUser(DatabaseManager database)
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
            WHERE user_id = {UserId};
        ";
        database.ExecuteNonQuery(query);
    }

    /// <summary>
    /// Supprime l'utilisateur de la base de données.
    /// </summary>
    /// <param name="database">Instance de <see cref="DatabaseManager"/> utilisée pour exécuter la requête.</param>
    public void SupprimerUser(DatabaseManager database)
    {
        string query = $"DELETE FROM users WHERE user_id = {UserId};";
        database.ExecuteNonQuery(query);
    }
    
    /// <summary>
    /// Récupère tous les utilisateurs depuis la base de données.
    /// </summary>
    /// <param name="database">Instance de <see cref="DatabaseManager"/> utilisée pour exécuter la requête.</param>
    /// <returns>Une liste d'objets <see cref="User"/> représentant tous les utilisateurs.</returns>
    public static List<User> GetAllUsers(DatabaseManager database)
    {
        var users = new List<User>();
        var table = database.ExecuteQuery("SELECT * FROM users;");

        foreach (DataRow row in table.Rows)
        {
            users.Add(new User
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
                Entreprise = row["entreprise"]?.ToString()
            });
        }

        return users;
    }
    
    /// <summary>
    /// Récupère tous les utilisateurs qui ont le role cuisinier depuis la base de données.
    /// </summary>
    /// <param name="database">Instance de <see cref="DatabaseManager"/> utilisée pour exécuter la requête.</param>
    /// <returns>Une liste d'objets <see cref="User"/> représentant tous les utilisateurs.</returns>
    public static List<User> GetAllCuisinier(DatabaseManager database)
    {
        var users = new List<User>();
        var table = database.ExecuteQuery("SELECT * FROM users WHERE role LIKE '%Cuisinier%'");

        foreach (DataRow row in table.Rows)
        {
            users.Add(new User
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
                Entreprise = row["entreprise"]?.ToString()
            });
        }

        return users;
    }

    /// <summary>
    /// Récupère un utilisateur en fonction de son identifiant.
    /// </summary>
    /// <param name="db">Instance de <see cref="DatabaseManager"/> utilisée pour exécuter la requête.</param>
    /// <param name="userId">Identifiant de l'utilisateur à rechercher.</param>
    /// <returns>Un objet <see cref="User"/> si trouvé, sinon null.</returns>
    public static User? GetById(DatabaseManager db, ulong userId)
    {
        var table = db.ExecuteQuery($"SELECT * FROM users WHERE user_id = {userId} LIMIT 1;");

        if (table.Rows.Count == 0)
            return null; 

        var row = table.Rows[0];

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
            Entreprise = row["entreprise"]?.ToString()
        };
    }
}
