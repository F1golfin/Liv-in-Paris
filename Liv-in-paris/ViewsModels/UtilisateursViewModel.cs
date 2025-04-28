using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

public class UtilisateursViewModel : ViewModelBase
{
    public ObservableCollection<User> Utilisateurs { get; set; } = new();
    public ICommand SupprimerUtilisateurCommand { get; }

    public UtilisateursViewModel()
    {
        SupprimerUtilisateurCommand = new RelayCommand<User>(SupprimerUtilisateur);
        ChargerUtilisateurs();
    }

    private void ChargerUtilisateurs()
    {
        Utilisateurs.Clear();
        var db = Database.Instance;

        foreach (User user in User.GetAllUsers(db))
        {
            Utilisateurs.Add(user);
        }
    }

    private void SupprimerUtilisateur(User user)
    {
        if (user == null) return;

        var result = MessageBox.Show($"Supprimer l'utilisateur {user.Prenom} {user.Nom} ?", "Confirmation", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            var db = Database.Instance;
            
            // Supprimer les plats si cuisinier
            db.ExecuteNonQuery($"DELETE FROM plats WHERE cuisinier_id = {user.UserId};");
            
            user.SupprimerUser(db);
            Utilisateurs.Remove(user);
            MessageBox.Show("✅ Utilisateur supprimé !");
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Erreur : " + ex.Message);
        }
    }
}