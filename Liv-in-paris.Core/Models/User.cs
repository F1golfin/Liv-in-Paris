using System.Data;

namespace Liv_in_paris.Core.Models;

public class User
{
    public ulong UserId { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Type { get; set; }
    public string Email { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Adresse { get; set; }
    public string Telephone { get; set; }
    public string? Entreprise { get; set; }

    public void CreerUser(DatabaseManager db)
    {
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

    public void SupprimerUser(DatabaseManager db)
    {
        db.ExecuteNonQuery($"DELETE FROM users WHERE user_id = {UserId};");
    }

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

    public static List<User> GetAllUsers(DatabaseManager db)
    {
        var table = db.ExecuteQuery("SELECT * FROM users;");
        return table.AsEnumerable().Select(HydraterUser).ToList();
    }

    public static List<User> GetAllCuisinier(DatabaseManager db)
    {
        var table = db.ExecuteQuery("SELECT * FROM users WHERE role LIKE '%Cuisinier%';");
        return table.AsEnumerable().Select(HydraterUser).ToList();
    }

    public static User? GetById(DatabaseManager db, ulong userId)
    {
        var table = db.ExecuteQuery($"SELECT * FROM users WHERE user_id = {userId} LIMIT 1;");
        return table.Rows.Count == 0 ? null : HydraterUser(table.Rows[0]);
    }

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