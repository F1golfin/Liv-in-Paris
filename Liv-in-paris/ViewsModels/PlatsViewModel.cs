using Liv_in_paris;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

public class PlatsViewModel : ViewModelBase
{
    public ObservableCollection<Plat> Plats { get; set; } = new();
    public ObservableCollection<User> Cuisiniers { get; set; } = new();
    
    private readonly NClientViewModel _clientVM;
    private User _cuisinierSelectionne;
    public User CuisinierSelectionne
    {
        get => _cuisinierSelectionne;
        set
        {
            if (_cuisinierSelectionne != value)
            {
                _cuisinierSelectionne = value;
                OnPropertyChanged();
                ChargerPlatsDepuisBDD();
            }
        }
    }
    
    private string _typeFiltre = "Tous";
    public string TypeFiltre
    {
        get => _typeFiltre;
        set
        {
            _typeFiltre = value;
            OnPropertyChanged();
            FiltrerEtTrierPlats();
        }
    }

    private bool _triPrixCroissant = true;
    public bool TriPrixCroissant
    {
        get => _triPrixCroissant;
        set
        {
            _triPrixCroissant = value;
            OnPropertyChanged();
            FiltrerEtTrierPlats();
        }
    }

    public ObservableCollection<string> TypesDisponibles { get; } = new() { "Tous", "Entrée", "Plat", "Dessert" };
    public ObservableCollection<string> RegimesDisponibles { get; } = new();
    public ObservableCollection<string> RegimesFiltres { get; set; } = new();

    public ICommand AppliquerFiltresCommand { get; }
    public ICommand AjouterAuPanierCommand { get; }
    public ICommand ResetFiltresCommand { get; }

    public PlatsViewModel(NClientViewModel clientVM)
    {
        _clientVM = clientVM;
        
        AjouterAuPanierCommand = new RelayCommand<Plat>(plat => _clientVM.AjouterAuPanier(plat));   
        AppliquerFiltresCommand = new RelayCommand(FiltrerEtTrierPlats);
        ResetFiltresCommand = new RelayCommand(() =>
        {
            TypeFiltre = "Tous";
            RegimesFiltres.Clear();
            TriPrixCroissant = true;
        });
        RegimesFiltres.CollectionChanged += (_, _) => FiltrerEtTrierPlats();
        
        var db = Database.Instance;
        
        foreach (User user in User.GetAllCuisinier(db))
        {
            if (user.UserId != _clientVM._utilisateur.UserId)
            {
                Cuisiniers.Add(user);
            }
        }
        
        if (Cuisiniers.Any())
        {
            CuisinierSelectionne = Cuisiniers[0];
        }

        foreach (RegimeAlimentaire regime in RegimeAlimentaire.GetAll(db))
        {
            RegimesDisponibles.Add(regime.Regime);
        }
    }

    private void ChargerPlatsDepuisBDD()
    {
        FiltrerEtTrierPlats();
    }
    
    public void FiltrerEtTrierPlats()
    {
        Plats.Clear();

        var db = Database.Instance;

        var tousLesPlats = Plat.GetDisponibles(db, CuisinierSelectionne?.UserId);

        // Exclure les plats déjà présents dans le panier
        var platsFiltres = tousLesPlats
            .Where(p => _clientVM.Panier.All(panierPlat => panierPlat.PlatId != p.PlatId))
            .Where(p =>
                (TypeFiltre == "Tous" || p.Recette.Type == TypeFiltre) &&
                (RegimesFiltres.Count == 0 || RegimesFiltres.All(r => p.Recette.RegimesNoms.Contains(r)))
            );

        platsFiltres = TriPrixCroissant ? platsFiltres.OrderBy(p => p.PrixParPersonne) : platsFiltres.OrderByDescending(p => p.PrixParPersonne);

        foreach (var plat in platsFiltres)
            Plats.Add(plat);
    }
    
    public void RetirerPlatDisponible(Plat plat)
    {
        Plats.Remove(plat);
    }
    
}