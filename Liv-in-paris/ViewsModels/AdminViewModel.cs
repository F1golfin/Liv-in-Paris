using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Graph;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel de l'interface admin. Permet de gérer les utilisateurs, exporter/importer, afficher des statistiques
/// et analyser un graphe de relations entre clients et cuisiniers.
/// </summary>
public class AdminViewModel : ViewModelBase
{
    private readonly DatabaseManager _db;
    private readonly AppViewModel _parent;
    private string _statistiquesResultat;
    
    /// <summary>
    /// Commande de déconnexion.
    /// </summary>
    public ICommand DeconnexionCommand { get; }
    
    
    /// <summary>
    /// Graphe construit lors de la dernière analyse de coloration.
    /// </summary>
    public Graphe<int> GrapheColoration { get; private set; }
    
    
    /// <summary>
    /// Résultat de la dernière coloration de graphe (clé = id noeud, valeur = couleur).
    /// </summary>
    public Dictionary<int, int> DerniereColoration { get; private set; }
    
    /// <summary>
    /// Liste observable des utilisateurs (clients, cuisiniers, admins).
    /// </summary>
    public ObservableCollection<User> Users { get; set; } = new();

    private User? _selectedUser;
    
    /// <summary>
    /// Utilisateur actuellement sélectionné dans l’interface.
    /// </summary>
    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            _selectedUser = value;
            OnPropertyChanged(nameof(SelectedUser));
        }
    }

    /// <summary>
    /// Constructeur principal du ViewModel admin.
    /// </summary>
    /// <param name="db">Gestionnaire d'accès à la base de données.</param>
    public AdminViewModel(AppViewModel app)
    {
        DeconnexionCommand = new RelayCommand(() => app.Deconnexion());
        _db = Database.Instance;
        LoadUsers();
        
    }
    
    /// <summary>
    /// Recharge tous les utilisateurs depuis la base de données.
    /// </summary>
    public void LoadUsers()
    {
        Users.Clear();
        var userList = User.GetAllUsers(_db);
        foreach (var user in userList)
            Users.Add(user);
    }

    /// <summary>
    /// Supprime l’utilisateur sélectionné.
    /// </summary>
    public void SupprimerUtilisateur()
    {
        if (SelectedUser == null) return;
        SelectedUser.SupprimerUser(_db);
        LoadUsers();
    }
    
    
    /// <summary>
    /// Exporte les utilisateurs au format JSON.
    /// </summary>
    /// <param name="filePath">Chemin du fichier de destination.</param>
    public void ExportUsersToJson(string filePath)
    {
        var users = User.GetAllUsers(_db);
        var json = System.Text.Json.JsonSerializer.Serialize(users, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
    
    /// <summary>
    /// Exporte les utilisateurs au format XML.
    /// </summary>
    /// <param name="filePath">Chemin du fichier de destination.</param>
    public void ExportUsersToXml(string filePath)
    {
        var users = User.GetAllUsers(_db);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<User>));
        using var stream = new FileStream(filePath, FileMode.Create);
        serializer.Serialize(stream, users);
    }
    
    /// <summary>
    /// Importe des utilisateurs depuis un fichier JSON.
    /// Vérifie les doublons d’email et de téléphone.
    /// </summary>
    public void ImportUsersFromJson(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(json);
        
        int nbSuccess = 0, nbErreurs = 0;
        if (users != null)
        {
            foreach (var user in users)
            {
                var existing = _db.ExecuteQuery($@"
                SELECT 1 FROM users 
                WHERE email = '{user.Email.Replace("'", "''")}' 
                OR telephone = '{user.Telephone.Replace("'", "''")}'");
                if (existing.Rows.Count == 0)
                {
                    user.CreerUser(_db);
                    nbSuccess++;
                }
                else
                {
                    Console.WriteLine($"⛔ Doublon : {user.Email}");
                    nbErreurs++;
                }
            }
        }
        MessageBox.Show($"Import terminé :\n✅ {nbSuccess} ajoutés\n❌ {nbErreurs} ignorés (doublons)", "Import utilisateurs");
        LoadUsers();
    }
    
    /// <summary>
    /// Importe des utilisateurs depuis un fichier XML.
    /// </summary>
    /// <param name="filePath">Chemin du fichier XML à importer.</param>
    public void ImportUsersFromXml(string filePath)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<User>));
        using var stream = new FileStream(filePath, FileMode.Open);
        var users = (List<User>)serializer.Deserialize(stream);
        foreach (var user in users)
        {
            user.CreerUser(_db);
        }
        LoadUsers();
    }
    
    /// <summary>
    /// Résultat formaté des statistiques calculées.
    /// </summary>
    public string StatistiquesResultat
    {
        get => _statistiquesResultat;
        set
        {
            _statistiquesResultat = value;
            OnPropertyChanged(nameof(StatistiquesResultat));
        }
    }
    
    
    /// <summary>
    /// Affiche le nombre de livraisons effectuées par chaque cuisinier.
    /// </summary>
    public void AfficherLivraisonsParCuisinier()
    {
        var table = _db.ExecuteQuery(@"
        SELECT u.prenom, COUNT(*) AS livraisons
        FROM commandes c
        JOIN users u ON c.cuisinier_id = u.user_id
        GROUP BY cuisinier_id;
    ");

        var sb = new StringBuilder("📦 Livraisons par cuisinier :\n");
        foreach (DataRow row in table.Rows)
            sb.AppendLine($"{row["prenom"]} : {row["livraisons"]} livraisons");

        StatistiquesResultat = sb.ToString();
    }
    
    
    /// <summary>
    /// Affiche les commandes passées au cours des 30 derniers jours.
    /// </summary>
    public void AfficherCommandesParPeriode()
    {
        var table = _db.ExecuteQuery(@"
        SELECT COUNT(*) AS total, MIN(heure_commande) AS debut, MAX(heure_commande) AS fin
        FROM commandes
        WHERE heure_commande >= DATE_SUB(NOW(), INTERVAL 30 DAY);
    ");

        var row = table.Rows[0];

        int total = Convert.ToInt32(row["total"]);
        string debut = row["debut"] != DBNull.Value ? Convert.ToDateTime(row["debut"]).ToShortDateString() : "n/a";
        string fin = row["fin"] != DBNull.Value ? Convert.ToDateTime(row["fin"]).ToShortDateString() : "n/a";

        StatistiquesResultat =
            $"📅 Commandes sur les 30 derniers jours :\n" +
            $"Total : {total}\n" +
            $"Période : du {debut} au {fin}";
    }

    /// <summary>
    /// Calcule et affiche le prix moyen des commandes.
    /// </summary>
    public void AfficherMoyennePrixCommandes()
    {
        var table = _db.ExecuteQuery(@"
        SELECT AVG(prix_total) AS moyenne
        FROM commandes;
    ");

        var moyenne = table.Rows[0]["moyenne"];
    
        if (moyenne == DBNull.Value)
        {
            StatistiquesResultat = "Aucune commande enregistrée.";
        }
        else
        {
            StatistiquesResultat = $"💰 Prix moyen des commandes : {Convert.ToDouble(moyenne):0.00} €";
        }
    }

    
    /// <summary>
    /// Calcule la moyenne des dépenses par client.
    /// </summary>
    public void AfficherMoyenneComptesClients()
    {
        var table = _db.ExecuteQuery(@"
        SELECT AVG(total) AS moyenne
        FROM (
            SELECT client_id, SUM(prix_total) AS total
            FROM commandes
            GROUP BY client_id
        ) AS sous_table;
    ");

        var moyenne = table.Rows[0]["moyenne"];

        if (moyenne == DBNull.Value)
        {
            StatistiquesResultat = "Aucun client n’a encore passé de commande.";
        }
        else
        {
            StatistiquesResultat = $"📊 Moyenne des comptes clients : {Convert.ToDouble(moyenne):0.00} €";
        }
    }

    
    /// <summary>
    /// Affiche les commandes de plats français passées récemment.
    /// </summary>
    public void AfficherCommandesClientFiltrées()
    {
        var table = _db.ExecuteQuery(@"
        SELECT c.commande_id, p.nom_plat, c.heure_commande, r.style_cuisine
        FROM commandes c
        JOIN lignes_commandes l ON l.commande_id = c.commande_id
        JOIN plats p ON p.plat_id = l.plat_id
        JOIN recettes r ON r.recette_id = p.recette_id
        WHERE r.style_cuisine = 'Française'
        AND c.heure_commande >= DATE_SUB(NOW(), INTERVAL 30 DAY)
        ORDER BY c.heure_commande DESC;
    ");

        var sb = new StringBuilder("🧾 Commandes avec plats français (30 derniers jours) :\n");
        foreach (DataRow row in table.Rows)
        {
            sb.AppendLine($"Commande #{row["commande_id"]} - {row["nom_plat"]} - {Convert.ToDateTime(row["heure_commande"]):g}");
        }

        StatistiquesResultat = sb.ToString();
    }
    
    
    private string _resultatColoration;
    
    
    /// <summary>
    /// Résultat de la dernière analyse de coloration du graphe.
    /// </summary>
    public string ResultatColoration
    {
        get => _resultatColoration;
        set
        {
            _resultatColoration = value;
            OnPropertyChanged(nameof(ResultatColoration));
        }
    }

    /// <summary>
    /// Analyse les relations client ↔ cuisinier, construit un graphe, applique la coloration
    /// et génère le résumé affiché dans l’interface admin.
    /// </summary>
    public void AnalyserColoration()
    {
        var graphe = new Graphe<int>();

        var table = _db.ExecuteQuery("SELECT client_id, cuisinier_id FROM commandes WHERE client_id IS NOT NULL AND cuisinier_id IS NOT NULL");
        foreach (DataRow row in table.Rows)
        {
            int clientId = Convert.ToInt32(row["client_id"]);
            int cuisinierId = Convert.ToInt32(row["cuisinier_id"]);

            var noeudClient = new Noeud<int>(clientId);
            var noeudCuisinier = new Noeud<int>(cuisinierId);

            graphe.ajouterNoeud(noeudClient);
            graphe.ajouterNoeud(noeudCuisinier);
            
            graphe.ajouterLien(new Lien<int>(noeudClient, 1, noeudCuisinier));
            graphe.ajouterLien(new Lien<int>(noeudCuisinier, 1, noeudClient));
        }
        
        var coloration = graphe.ColorierWelshPowell();
        DerniereColoration = coloration;
        GrapheColoration = graphe;
        
        int nbCouleurs = coloration.Values.Distinct().Count();
        bool biparti = nbCouleurs == 2;

        var parGroupe = coloration
            .GroupBy(kvp => kvp.Value)
            .OrderBy(g => g.Key)
            .Select(g => $"Couleur {g.Key} : {string.Join(", ", g.Select(x => x.Key))}");
        
        ResultatColoration = $"🎨 Nombre minimal de couleurs : {nbCouleurs}\n" +
                             (biparti ? "✅ Le graphe est biparti." : "❌ Le graphe n’est pas biparti.") + "\n\n" +
                             "📊 Groupes indépendants :\n" + string.Join("\n", parGroupe);
    }
}
