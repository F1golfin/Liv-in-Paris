using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public class AdminViewModel : INotifyPropertyChanged
{
    private readonly DatabaseManager _db;
    private readonly AppViewModel _parent;
    public ICommand DeconnexionCommand { get; }

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

    public AdminViewModel(DatabaseManager db)
    {
        _db = db;
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



    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
