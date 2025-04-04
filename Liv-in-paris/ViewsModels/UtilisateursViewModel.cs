using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public class UtilisateursViewModel : ViewModelBase
{
    public ObservableCollection<User> Utilisateurs { get; } = new();
    public ICommand SupprimerUtilisateurCommand => new RelayCommand<User>(SupprimerUtilisateur);

    public UtilisateursViewModel()
    {
        ChargerUtilisateurs();
    }

    private void ChargerUtilisateurs()
    {
        Utilisateurs.Clear();
        var db = new DatabaseManager("localhost", "livin_paris", "root", "root");

        var table = db.ExecuteQuery("SELECT * FROM users");

        foreach (DataRow row in table.Rows)
        {
            Utilisateurs.Add(new User
            {
                UserId = Convert.ToUInt64(row["user_id"]),
                Nom = row["nom"].ToString(),
                Prenom = row["prenom"].ToString(),
                Email = row["email"].ToString()
            });
        }
    }

    private void SupprimerUtilisateur(User user)
    {
        if (user == null) return;

        var result = MessageBox.Show($"Supprimer l'utilisateur {user.Prenom} {user.Nom} ?", "Confirmation", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            var db = new DatabaseManager("localhost", "livin_paris", "root", "root");
            db.ExecuteNonQuery($"DELETE FROM plats WHERE cuisinier_id = {user.UserId};");
            db.ExecuteNonQuery($"DELETE FROM users WHERE user_id = {user.UserId}");
            Utilisateurs.Remove(user);
            MessageBox.Show("✅ Utilisateur supprimé !");
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Erreur : " + ex.Message);
        }
    }
}