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

    public void CreerUser(DatabaseManager database)
    {
        string query = $@"
            INSERT INTO users (password, role, type, email, nom, prenom, adresse, telephone, entreprise)
            VALUES ('{Password}', '{Role}', '{Type}', '{Email}', '{Nom}', '{Prenom}', '{Adresse}', '{Telephone}', {(Entreprise != null ? $"'{Entreprise}'" : "NULL")});
        ";
        database.ExecuteNonQuery(query);
    }

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

    public void SupprimerUser(DatabaseManager database)
    {
        string query = $"DELETE FROM users WHERE user_id = {UserId};";
        database.ExecuteNonQuery(query);
    }
    
    public static List<User> GetAll(DatabaseManager database)
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
