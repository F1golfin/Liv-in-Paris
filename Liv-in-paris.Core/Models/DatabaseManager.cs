using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Liv_in_paris.Core.Models;

/// <summary>
/// Fournit un accès de haut niveau à la base de données MySQL de l’application.
/// Gère la connexion, les requêtes et la création des structures.
/// </summary>
public class DatabaseManager
{
    private readonly string connectionString;

    /// <summary>
    /// Initialise une instance de <see cref="DatabaseManager"/> avec les identifiants de connexion à la base.
    /// </summary>
    /// <param name="server">Adresse du serveur MySQL.</param>
    /// <param name="database">Nom de la base de données.</param>
    /// <param name="user">Nom d’utilisateur.</param>
    /// <param name="password">Mot de passe.</param>
    public DatabaseManager(string server, string database, string user, string password)
    {
        Console.WriteLine("🔐 Connexion string → " + $"Server={server};Database={database};Uid={user};Pwd={password};");
        connectionString = $"Server={server};Database={database};Uid={user};Pwd={password};";
    }

    /// <summary>
    /// Crée la base de données si elle n'existe pas encore (hors tables).
    /// </summary>
    public static void CreateDatabase(string server, string user, string password, string dbName)
    {
        var connStr = $"Server={server};Uid={user};Pwd={password};";
        using var connection = new MySqlConnection(connStr);
        connection.Open();

        using var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {dbName};", connection);
        cmd.ExecuteNonQuery();
        Console.WriteLine("📦 Base de données vérifiée/créée.");
    }

    /// <summary>
    /// Supprime et recrée toutes les tables nécessaires à l'application dans la base sélectionnée.
    /// Inclut les contraintes, relations et types ENUM/SET.
    /// </summary>
    public void CreateTablesIfNotExists()
    {
        string sql = @"DROP DATABASE IF EXISTS livin_paris;
CREATE DATABASE livin_paris;
USE livin_paris;

-- TABLE users
CREATE TABLE users
(
    user_id    BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    password   VARCHAR(255)                       NOT NULL,
    role       SET ('Client', 'Cuisinier')        NOT NULL,
    type       ENUM ('Particulier', 'Entreprise') NOT NULL,
    email      VARCHAR(100) UNIQUE                NOT NULL,
    nom        VARCHAR(50)                        NOT NULL,
    prenom     VARCHAR(50)                        NOT NULL,
    adresse    VARCHAR(255)                       NOT NULL,
    telephone  VARCHAR(15) UNIQUE                 NOT NULL,
    entreprise VARCHAR(50)
);

-- TABLE commandes
CREATE TABLE commandes
(
    commande_id    BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    heure_commande DATETIME,
    adresse_depart TEXT          NOT NULL,
    prix_total     DECIMAL(8, 2) NOT NULL,
    client_id      BIGINT UNSIGNED,
    cuisinier_id   BIGINT UNSIGNED,

    FOREIGN KEY (client_id) REFERENCES users (user_id) ON DELETE SET NULL,
    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id) ON DELETE SET NULL
);

-- TABLE recettes
CREATE TABLE recettes
(
    recette_id         BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom_recette        VARCHAR(100)                                 NOT NULL,
    type               ENUM ('Entrée', 'Plat Principal', 'Dessert') NOT NULL,
    ingredients        TEXT                                         NOT NULL,
    style_cuisine      INT                                          NOT NULL,
    regime_alimentaire VARCHAR(50),
    parent_recette_id  BIGINT UNSIGNED UNIQUE,

    FOREIGN KEY (parent_recette_id) REFERENCES recettes (recette_id) ON DELETE SET NULL
);

-- TABLE lignes_commandes
CREATE TABLE lignes_commandes
(
    ligne_commande_id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    heure_livraison   DATETIME,
    adresse_arrivee   TEXT                                                            NOT NULL,
    statut            ENUM ('Commandee', 'Preparee', 'En cours', 'Livree', 'Annulee') NOT NULL,
    commande_id       BIGINT UNSIGNED                                                 NOT NULL,

    FOREIGN KEY (commande_id) REFERENCES commandes (commande_id)
);

-- TABLE plats
CREATE TABLE plats
(
    plat_id           BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    nom_plat          VARCHAR(100)    NOT NULL,
    nb_parts          INT             NOT NULL,
    date_fabrication  DATE            NOT NULL,
    date_peremption   DATE            NOT NULL,
    prix_par_personne DECIMAL(6, 2)   NOT NULL,
    photo             LONGBLOB,
    cuisinier_id      BIGINT UNSIGNED NOT NULL,
    recette_id        BIGINT UNSIGNED NOT NULL,
    commande_id       BIGINT UNSIGNED,

    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id),
    FOREIGN KEY (recette_id) REFERENCES recettes (recette_id),
    FOREIGN KEY (commande_id) REFERENCES commandes (commande_id)
);

-- TABLE evaluation
CREATE TABLE evaluation
(
    client_id       BIGINT UNSIGNED,
    cuisinier_id    BIGINT UNSIGNED,
    note            INT CHECK (note BETWEEN 1 AND 5),
    commentaire     TEXT,
    date_evaluation DATETIME,

    PRIMARY KEY (client_id, cuisinier_id),
    FOREIGN KEY (client_id) REFERENCES users (user_id),
    FOREIGN KEY (cuisinier_id) REFERENCES users (user_id)
);";

        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        using var cmd = new MySqlCommand(sql, connection);
        cmd.ExecuteNonQuery();
        Console.WriteLine("🧱 Tables vérifiées/créées.");
    }

    /// <summary>
    /// Tente une ouverture de connexion pour vérifier la validité des identifiants.
    /// </summary>
    public void TesterConnexion()
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();
        Console.WriteLine("✅ Connexion réussie !");
    }

    /// <summary>
    /// Exécute une requête SQL en lecture et retourne un DataTable avec les résultats.
    /// </summary>
    /// <param name="query">Requête SELECT SQL.</param>
    /// <returns>Résultats sous forme de <see cref="DataTable"/>.</returns>
    public DataTable ExecuteQuery(string query)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        using var cmd = new MySqlCommand(query, connection);
        using var adapter = new MySqlDataAdapter(cmd);

        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    /// <summary>
    /// Exécute une commande SQL d’écriture (INSERT, UPDATE, DELETE).
    /// </summary>
    /// <param name="query">Requête SQL sans résultat attendu.</param>
    /// <returns>Nombre de lignes affectées.</returns>
    public int ExecuteNonQuery(string query)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        using var cmd = new MySqlCommand(query, connection);
        return cmd.ExecuteNonQuery();
    }
}
