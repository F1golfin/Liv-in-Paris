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

public class AdminViewModel : INotifyPropertyChanged
{
    private readonly DatabaseManager _db;
    private readonly AppViewModel _parent;
    public ICommand DeconnexionCommand { get; }
    public Graphe<int> GrapheColoration { get; private set; }
    public Dictionary<int, int> DerniereColoration { get; private set; }


    public ObservableCollection<User> Users { get; set; } = new();

    private User? _selectedUser;
    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            _selectedUser = value;
            OnPropertyChanged(nameof(SelectedUser));
        }
    }

    public AdminViewModel(AppViewModel app)
    {
        DeconnexionCommand = new RelayCommand(() => app.Deconnexion());
        _db = Database.Instance;
        LoadUsers();
        
    }
    
    
    public void LoadUsers()
    {
        Users.Clear();
        var userList = User.GetAllUsers(_db);
        foreach (var user in userList)
            Users.Add(user);
    }

    public void SupprimerUtilisateur()
    {
        if (SelectedUser == null) return;
        SelectedUser.SupprimerUser(_db);
        LoadUsers();
    }
    
    public void ExportUsersToJson(string filePath)
    {
        var users = User.GetAllUsers(_db);
        var json = System.Text.Json.JsonSerializer.Serialize(users, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
    
    public void ExportUsersToXml(string filePath)
    {
        var users = User.GetAllUsers(_db);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<User>));
        using var stream = new FileStream(filePath, FileMode.Create);
        serializer.Serialize(stream, users);
    }
    
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
    
    public void ImportUsersFromXml(string filePath)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<User>));
        using var stream = new FileStream(filePath, FileMode.Open);
        var users = (List<User>)serializer.Deserialize(stream);
        foreach (var user in users)
        {
            user.CreerUser(_db);
        }
        LoadUsers(); // Recharge après import
    }

    private string _statistiquesResultat;
    public string StatistiquesResultat
    {
        get => _statistiquesResultat;
        set
        {
            _statistiquesResultat = value;
            OnPropertyChanged(nameof(StatistiquesResultat));
        }
    }
    
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
    // 1. Création du graphe
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

        // On ajoute les deux sens pour simuler un graphe non orienté
        graphe.ajouterLien(new Lien<int>(noeudClient, 1, noeudCuisinier));
        graphe.ajouterLien(new Lien<int>(noeudCuisinier, 1, noeudClient));
    }

    // 2. Coloration du graphe
    var coloration = graphe.ColorierWelshPowell();
    DerniereColoration = coloration;
    GrapheColoration = graphe;

    // 3. Analyse du résultat
    int nbCouleurs = coloration.Values.Distinct().Count();
    bool biparti = nbCouleurs == 2;

    var parGroupe = coloration
        .GroupBy(kvp => kvp.Value)
        .OrderBy(g => g.Key)
        .Select(g => $"Couleur {g.Key} : {string.Join(", ", g.Select(x => x.Key))}");

    // 4. Résumé pour l’interface
    ResultatColoration = $"🎨 Nombre minimal de couleurs : {nbCouleurs}\n" +
                         (biparti ? "✅ Le graphe est biparti." : "❌ Le graphe n’est pas biparti.") + "\n\n" +
                         "📊 Groupes indépendants :\n" + string.Join("\n", parGroupe);
}





    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
