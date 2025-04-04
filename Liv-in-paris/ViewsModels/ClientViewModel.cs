using System.Collections.ObjectModel;
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
    
    public object VueActive
    {
        get => _vueActive;
        set { _vueActive = value; OnPropertyChanged(); }
    }
    
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

        AfficherPlats(); // vue par défaut
        Panier.CollectionChanged += Panier_CollectionChanged;
    }

    private void AfficherPlats()
    {
        var vue = new PlatsView();
        vue.DataContext = new PlatsViewModel(this);
        VueActive = vue;
    }
    
    private void AfficherPanier()
    {
        var vue = new PanierView();
        vue.DataContext = this;
        VueActive = vue;
    }
    
    private void AfficherCommandes() => VueActive = new TextBlock { Text = "Historique non chargé." };
    
    public void AjouterAuPanier(Plat plat)
    {
        if (!Panier.Contains(plat))
            Panier.Add(plat);
        
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
    }
    
}