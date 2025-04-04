using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public class ClientViewModel : ViewModelBase
{
    private readonly AppViewModel _app;
    public ICommand DeconnexionCommand { get; }
    public ICommand VoirPlatsCommand { get; }
    public ICommand VoirPanierCommand { get; }
    public ICommand VoirCommandesCommand { get; }
    public ICommand PasserCommandeCommand => new RelayCommand(PasserCommande);

    private object _vueActive;
    private User _utilisateur;
    
    public object VueActive
    {
        get => _vueActive;
        set { _vueActive = value; OnPropertyChanged(); }
    }
    
    public decimal PrixTotal
    {
        get => Panier.Sum(p => p.PrixParPersonne);
    }
    
    public ObservableCollection<Plat> Panier { get; set; } = new();
    public ObservableCollection<CommandeAvecPlats> CommandesClient { get; set; } = new();
    
    public string UtilisateurLabel => $"Bonjour {_utilisateur.Prenom}";
    
    public ClientViewModel(AppViewModel app, User utilisateur)
    {
        _app = app;
        _utilisateur = utilisateur;

        VoirPlatsCommand = new RelayCommand(AfficherPlats);
        VoirPanierCommand = new RelayCommand(AfficherPanier);
        VoirCommandesCommand = new RelayCommand(AfficherCommandes);
        DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());
        
        AfficherPlats(); // vue par défaut
        Panier.CollectionChanged += Panier_CollectionChanged;
    }

    private void AfficherPlats()
    {
        var vue = new PlatsView();
        try
        {
            vue.DataContext = new PlatsViewModel(this);
            VueActive = vue;
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Erreur dans PlatsViewModel : " + ex.Message);
        }
    }
    
    private void AfficherPanier()
    {
        var vue = new PanierView();
        vue.DataContext = this;
        VueActive = vue;
    }
    
    private void AfficherCommandes()
    {
        var vue = new CommandesView();
        vue.DataContext = new CommandesViewModel(_app, _utilisateur);
        VueActive = vue;
    }
    
    public void AjouterAuPanier(Plat plat)
    {
        if (Panier.Count > 0)
        {
            var premierCuisinierId = Panier[0].CuisinierId;

            if (plat.CuisinierId != premierCuisinierId)
            {
                MessageBox.Show("❌ Vous ne pouvez commander que des plats du même cuisinier. Veuillez valider ou vider votre panier.");
                return;
            }
        }

        if (!Panier.Contains(plat))
            Panier.Add(plat);

        // Facultatif : retirer le plat de la liste visible
        if (VueActive is PlatsView vue && vue.DataContext is PlatsViewModel platsVM)
        {
            platsVM.Plats.Remove(plat);
        }

        OnPropertyChanged(nameof(Panier));
    }
    
    public ICommand RetirerDuPanierCommand => new RelayCommand<Plat>(plat =>
    {
        Panier.Remove(plat);
        OnPropertyChanged(nameof(Panier));
    });
    
    private void Panier_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Panier));
        OnPropertyChanged(nameof(PrixTotal));
    }
    
    private void PasserCommande()
    {
        if (Panier.Count == 0)
        {
            MessageBox.Show("Votre panier est vide.");
            return;
        }

        try
        {
            var db = new DatabaseManager("localhost", "livin_paris", "root", "root");

            // 1. Création de la commande
            string insertCommande = $@"
            INSERT INTO commandes (heure_commande, adresse_depart, prix_total, client_id, cuisinier_id)
            VALUES (NOW(), 'Adresse factice', {PrixTotal.ToString().Replace(',', '.')}, {_utilisateur.UserId}, {Panier[0].CuisinierId});
        ";

            db.ExecuteNonQuery(insertCommande);

            // 2. Récupération de l'ID de la commande insérée
            var result = db.ExecuteQuery("SELECT LAST_INSERT_ID() AS id;");
            int commandeId = Convert.ToInt32(result.Rows[0]["id"]);

            // 3. Mise à jour des plats avec le nouvel ID de commande
            foreach (var plat in Panier)
            {
                string update = $"UPDATE plats SET commande_id = {commandeId} WHERE plat_id = {plat.PlatId};";
                db.ExecuteNonQuery(update);
            }

            MessageBox.Show("✅ Commande enregistrée !");
            Panier.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Erreur lors de la commande : " + ex.Message);
        }
    }
    
    
}