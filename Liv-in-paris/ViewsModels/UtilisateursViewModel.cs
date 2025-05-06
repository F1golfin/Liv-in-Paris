using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel pour l’interface d’administration permettant la gestion des utilisateurs.
/// Gère l’affichage et la suppression des comptes utilisateurs.
/// </summary>
public class UtilisateursViewModel : ViewModelBase
{
    /// <summary>
    /// Liste observable des utilisateurs affichés dans la vue.
    /// </summary>
    public ObservableCollection<User> Utilisateurs { get; set; } = new();

    /// <summary>
    /// Commande pour supprimer un utilisateur sélectionné.
    /// </summary>
    public ICommand SupprimerUtilisateurCommand { get; }

    /// <summary>
    /// Initialise le ViewModel et charge les utilisateurs depuis la base de données.
    /// </summary>
    public UtilisateursViewModel()
    {
        SupprimerUtilisateurCommand = new RelayCommand<User>(SupprimerUtilisateur);
        ChargerUtilisateurs();
    }

    /// <summary>
    /// Charge tous les utilisateurs depuis la base et les ajoute à la collection observable.
    /// </summary>
    private void ChargerUtilisateurs()
    {
        Utilisateurs.Clear();
        var db = Database.Instance;

        foreach (User user in User.GetAllUsers(db))
        {
            Utilisateurs.Add(user);
        }
    }

    /// <summary>
    /// Supprime un utilisateur après confirmation, ainsi que ses plats s’il est cuisinier.
    /// </summary>
    /// <param name="user">Utilisateur à supprimer.</param>
    private void SupprimerUtilisateur(User user)
    {
        if (user == null) return;

        var result = MessageBox.Show(
            $"Supprimer l'utilisateur {user.Prenom} {user.Nom} ?",
            "Confirmation",
            MessageBoxButton.YesNo);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var db = Database.Instance;
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
