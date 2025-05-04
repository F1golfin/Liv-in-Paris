using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel responsable de l'affichage, de la gestion et des actions sur les commandes d'un client.
/// </summary>
public class CommandesViewModel : ViewModelBase
{
    private readonly NClientViewModel _clientVM;
    private readonly User _utilisateur;

    /// <summary>
    /// Liste observable des commandes du client.
    /// </summary>
    public ObservableCollection<Commande> CommandesClient { get; set; } = new();

    /// <summary>
    /// Commande pour supprimer une commande existante.
    /// </summary>
    public ICommand SupprimerCommandeCommand { get; }

    /// <summary>
    /// Commande pour noter un cuisinier.
    /// </summary>
    public ICommand NoterCuisinierCommand { get; }

    /// <summary>
    /// Initialise une nouvelle instance du <see cref="CommandesViewModel"/>.
    /// </summary>
    /// <param name="clientVM">ViewModel parent représentant le client connecté.</param>
    /// <param name="utilisateur">Utilisateur client associé.</param>
    public CommandesViewModel(NClientViewModel clientVM, User utilisateur)
    {
        _clientVM = clientVM;
        _utilisateur = utilisateur;

        NoterCuisinierCommand = new RelayCommand<Commande>(NoterCuisinier);
        SupprimerCommandeCommand = new RelayCommand<Commande>(SupprimerCommande);

        ChargerCommandes();
    }

    /// <summary>
    /// Charge toutes les commandes associées au client depuis la base de données.
    /// </summary>
    public void ChargerCommandes()
    {
        CommandesClient.Clear();

        var db = Database.Instance;
        foreach (Commande c in Commande.GetByClient(db, _utilisateur.UserId))
        {
            CommandesClient.Add(c);
        }
        Console.WriteLine($"Commandes chargées : {CommandesClient.Count}");
    }

    /// <summary>
    /// Ouvre une boîte de dialogue permettant de noter le cuisinier d'une commande.
    /// </summary>
    /// <param name="commande">Commande à évaluer.</param>
    private void NoterCuisinier(Commande commande)
    {
        EvaluationDialogViewModel.OuvrirDialog(_utilisateur.UserId, commande.CuisinierId ?? 0);
    }

    /// <summary>
    /// Supprime une commande après confirmation de l'utilisateur, puis recharge la liste des commandes.
    /// </summary>
    /// <param name="commande">Commande à supprimer.</param>
    private void SupprimerCommande(Commande commande)
    {
        if (commande == null) return;

        var result = MessageBox.Show("Voulez-vous vraiment supprimer cette commande ?", "Confirmation", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            var db = Database.Instance;
            commande.SupprimerCommande(db);

            MessageBox.Show("✅ Commande supprimée !");
            _clientVM.AfficherPlats();
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Erreur lors de la suppression : " + ex.Message);
        }

        ChargerCommandes();
    }

    /// <summary>
    /// Recharge la liste des commandes du client à partir de la base de données.
    /// </summary>
    public void RechargerCommandes()
    {
        CommandesClient.Clear();
        foreach (Commande c in Commande.GetByClient(Database.Instance, _utilisateur.UserId))
        {
            CommandesClient.Add(c);
        }
    }
}
