using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.ViewModels;

namespace Liv_in_paris;

public class CommandesViewModel : ViewModelBase
{
    private readonly AppViewModel _app;
    private readonly User _utilisateur;

    public ObservableCollection<Commande> CommandesClient { get; set; } = new();
    public ICommand SupprimerCommandeCommand { get; }
    public ICommand NoterCuisinierCommand { get; }

    public CommandesViewModel(AppViewModel app, User utilisateur)
    {
        _app = app;
        _utilisateur = utilisateur;

        NoterCuisinierCommand = new RelayCommand<Commande>(NoterCuisinier);
        SupprimerCommandeCommand = new RelayCommand<Commande>(SupprimerCommande);

        ChargerCommandes();
    }

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
    
    private void NoterCuisinier(Commande commande)
    {
        EvaluationDialogViewModel.OuvrirDialog(_utilisateur.UserId, commande.CuisinierId ?? 0);
    }
    
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
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Erreur lors de la suppression : " + ex.Message);
        }
        
        ChargerCommandes();
    }
}