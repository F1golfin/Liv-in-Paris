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

    private object _vueActive;
    private User _utilisateur;
    
    public ObservableCollection<Plat> Panier { get; set; } = new();
    
    public string UtilisateurLabel => $"Bonjour {_utilisateur.Prenom}";
    
    public ClientViewModel(AppViewModel app, User utilisateur)
    {
        _app = app;
        _utilisateur = utilisateur;

        VoirPlatsCommand = new RelayCommand(AfficherPlats);
        VoirPanierCommand = new RelayCommand(AfficherPanier);
        VoirCommandesCommand = new RelayCommand(AfficherCommandes);
        DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());
        
        ChargerDonnees();
    }
    
    public object VueActive
    {
        get => _vueActive;
        set { _vueActive = value; OnPropertyChanged(); }
    }
    
    public void ChargerDonnees()
    {
        switch (VueActive)
        {
            case PlatsView:
                var platsView = new PlatsView();
                platsView.DataContext = new PlatsViewModel(this);
                VueActive = platsView;
                break;

            case PanierView:
                var panierView = new PanierView();
                panierView.DataContext = new PanierViewModel(Panier, _utilisateur, _app);
                VueActive = panierView;
                break;

            case CommandesView:
                var commandesView = new CommandesView();
                commandesView.DataContext = new CommandesViewModel(_app, _utilisateur);
                VueActive = commandesView;
                break;

            default:
                var vue = new PlatsView();
                vue.DataContext = new PlatsViewModel(this);
                VueActive = vue;
                break;
        }
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
        vue.DataContext = new PanierViewModel(Panier, _utilisateur, _app);
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

        // Retirer le plat de la liste visible
        if (VueActive is PlatsView vue && vue.DataContext is PlatsViewModel platsVM)
        {
            platsVM.Plats.Remove(plat);
        }

        OnPropertyChanged(nameof(Panier));
    }
    
}