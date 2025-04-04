using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public class CommandesViewModel : ViewModelBase
{
    private readonly AppViewModel _app;
    private readonly User _utilisateur;

    public ObservableCollection<CommandeAvecPlats> CommandesClient { get; } = new();

    public ICommand NoterCuisinierCommand { get; }

    public CommandesViewModel(AppViewModel app, User utilisateur)
    {
        _app = app;
        _utilisateur = utilisateur;

        NoterCuisinierCommand = new RelayCommand<CommandeAvecPlats>(NoterCuisinier);

        ChargerCommandes();
    }

    public void ChargerCommandes()
    {
        CommandesClient.Clear();
        var db = new DatabaseManager("localhost", "livin_paris", "root", "root");

        // 🔁 Requête pour récupérer uniquement les commandes du client
        string commandesQuery = $@"
        SELECT * FROM commandes
        WHERE client_id = {_utilisateur.UserId}
        ORDER BY heure_commande DESC";

        var table = db.ExecuteQuery(commandesQuery);

        foreach (DataRow row in table.Rows)
        {
            var commande = new Commande
            {
                CommandeId = row["commande_id"] != DBNull.Value ? Convert.ToUInt64(row["commande_id"]) : 0,
                HeureCommande = row["heure_commande"] != DBNull.Value ? Convert.ToDateTime(row["heure_commande"]) : DateTime.MinValue,
                AdresseDepart = row["adresse_depart"] != DBNull.Value ? row["adresse_depart"].ToString() : "Adresse inconnue",
                PrixTotal = row["prix_total"] != DBNull.Value ? Convert.ToDecimal(row["prix_total"]) : 0,
                ClientId = row["client_id"] != DBNull.Value ? Convert.ToUInt64(row["client_id"]) : 0,
                CuisinierId = row["cuisinier_id"] != DBNull.Value ? Convert.ToUInt64(row["cuisinier_id"]) : 0
            };

            var wrapper = new CommandeAvecPlats
            {
                Commande = commande,
                // Pas encore de plats ni de statut
                CuisinierNom = "Inconnu"
            };

            CommandesClient.Add(wrapper);
        }

        Console.WriteLine($"✅ Commandes chargées : {CommandesClient.Count}");
    }


    private void NoterCuisinier(CommandeAvecPlats commande)
    {
        // Ouvrir une boîte de dialogue ou naviguer vers une vue de notation
        MessageBox.Show($"Tu veux noter {commande.CuisinierNom} pour la commande {commande.Commande.CommandeId}");
    }
}