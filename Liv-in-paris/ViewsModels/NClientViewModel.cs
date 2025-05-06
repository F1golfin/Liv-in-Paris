using Liv_in_paris.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel principal pour les utilisateurs ayant le rôle "Client".
/// Gère la navigation entre les vues des plats, du panier, des commandes, et du compte.
/// </summary>
public class NClientViewModel : ViewModelBase
{
    private readonly AppViewModel _app;

    /// <summary>
    /// Utilisateur actuellement connecté.
    /// </summary>
    public readonly User _utilisateur;

    /// <summary>
    /// Vue actuellement affichée dans la section des plats.
    /// </summary>
    public object PlatsVue
    {
        get => _platsVue;
        set { _platsVue = value; OnPropertyChanged(); }
    }
    private object _platsVue;

    /// <summary>
    /// Vue du panier client.
    /// </summary>
    public object PanierVue
    {
        get => _panierVue;
        set { _panierVue = value; OnPropertyChanged(); }
    }
    private object _panierVue;

    /// <summary>
    /// Vue des commandes passées par le client.
    /// </summary>
    public object CommandesVue
    {
        get => _commandesVue;
        set { _commandesVue = value; OnPropertyChanged(); }
    }
    private object _commandesVue;

    /// <summary>
    /// Message affiché sur le bouton de gestion du compte (contextuel).
    /// </summary>
    public string TexteBoutonCompte
    {
        get => _texteBoutonCompte;
        set { _texteBoutonCompte = value; OnPropertyChanged(); }
    }
    private string _texteBoutonCompte = "Gérer votre compte";

    private bool _estDansCompte = false;

    /// <summary>
    /// Commande pour se déconnecter.
    /// </summary>
    public ICommand DeconnexionCommand { get; }

    /// <summary>
    /// Commande pour rafraîchir les vues (plats, panier, commandes).
    /// </summary>
    public ICommand ActualiserCommand { get; }

    /// <summary>
    /// Commande pour basculer entre la vue des plats et la gestion du compte.
    /// </summary>
    public ICommand GererCompteCommand { get; }

    /// <summary>
    /// Collection observable des plats actuellement dans le panier.
    /// </summary>
    public ObservableCollection<Plat> Panier { get; } = new();

    /// <summary>
    /// Message de bienvenue personnalisé pour l'utilisateur.
    /// </summary>
    public string UtilisateurLabel => $"Bonjour {_utilisateur.Prenom}";

    /// <summary>
    /// Initialise un nouveau ViewModel client avec les vues et les commandes.
    /// </summary>
    /// <param name="app">ViewModel principal de l'application.</param>
    /// <param name="utilisateur">Utilisateur client actuellement connecté.</param>
    public NClientViewModel(AppViewModel app, User utilisateur)
    {
        _app = app;
        _utilisateur = utilisateur;
        ActualiserCommand = new RelayCommand(ChargerDonnees);
        DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());
        GererCompteCommand = new RelayCommand(ToggleCompteOuPlats);

        ChargerDonnees();
    }

    /// <summary>
    /// Charge les vues Plats, Panier et Commandes avec leurs ViewModels respectifs.
    /// </summary>
    public void ChargerDonnees()
    {
        var platsView = new PlatsView();
        platsView.DataContext = new PlatsViewModel(this);
        PlatsVue = platsView;

        var panierView = new PanierView();
        panierView.DataContext = new PanierViewModel(Panier, _utilisateur, this);
        PanierVue = panierView;

        var commandesView = new CommandesView();
        commandesView.DataContext = new CommandesViewModel(this, _utilisateur);
        CommandesVue = commandesView;
    }

    /// <summary>
    /// Ajoute un plat au panier si toutes les conditions sont remplies,
    /// notamment que les plats soient du même cuisinier.
    /// </summary>
    /// <param name="plat">Plat à ajouter au panier.</param>
    public void AjouterAuPanier(Plat plat)
    {
        if (Panier.Count > 0 && plat.CuisinierId != Panier[0].CuisinierId)
        {
            MessageBox.Show("❌ Vous ne pouvez commander que des plats du même cuisinier. Veuillez valider ou vider votre panier.");
            return;
        }

        if (Panier.Any(p => p.PlatId == plat.PlatId))
            return; // Évite les doublons

        Panier.Add(plat);

        try
        {
            var db = Database.Instance;
            var ligne = new LigneCommande
            {
                PlatId = plat.PlatId,
                AdresseArrivee = null,
                HeureLivraison = null,
                Statut = "Panier",
                CommandeId = null
            };
            ligne.AjouterCommande_tps(db);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'ajout de la ligne de commande : {ex.Message}");
        }

        if (PlatsVue is PlatsView vue && vue.DataContext is PlatsViewModel platsVM)
        {
            platsVM.RetirerPlatDisponible(plat);
        }

        OnPropertyChanged(nameof(Panier));
    }

    /// <summary>
    /// Affiche la vue de gestion du compte.
    /// </summary>
    private void AfficherCompte()
    {
        var compteView = new CompteView();
        compteView.DataContext = new CompteViewModel(_utilisateur, this);
        PlatsVue = compteView;
    }

    /// <summary>
    /// Affiche la vue des plats disponibles.
    /// </summary>
    public void AfficherPlats()
    {
        var platsView = new PlatsView();
        platsView.DataContext = new PlatsViewModel(this);
        PlatsVue = platsView;
    }

    /// <summary>
    /// Bascule entre la vue de gestion du compte et la vue des plats.
    /// </summary>
    private void ToggleCompteOuPlats()
    {
        if (_estDansCompte)
        {
            AfficherPlats();
            TexteBoutonCompte = "Gérer votre compte";
        }
        else
        {
            var compteView = new CompteView();
            compteView.DataContext = new CompteViewModel(_utilisateur, this);
            PlatsVue = compteView;
            TexteBoutonCompte = "← Revenir aux plats";
        }

        _estDansCompte = !_estDansCompte;
    }
}
