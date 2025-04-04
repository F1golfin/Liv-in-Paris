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
    }

    private void AfficherPlats()
    {
        var vue = new PlatsView();
        vue.DataContext = new PlatsViewModel();
        VueActive = vue;
    }
    private void AfficherPanier() => VueActive = new TextBlock { Text = "Panier vide pour l’instant." };
    private void AfficherCommandes() => VueActive = new TextBlock { Text = "Historique non chargé." };
    
    
}