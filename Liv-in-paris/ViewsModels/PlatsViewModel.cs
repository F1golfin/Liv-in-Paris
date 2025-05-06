using Liv_in_paris;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

/// <summary>
/// ViewModel de la vue affichant les plats disponibles.
/// Gère les filtres, le tri, et l’ajout au panier.
/// </summary>
public class PlatsViewModel : ViewModelBase
{
    /// <summary>
    /// Liste des plats actuellement affichés (filtrés et triés).
    /// </summary>
    public ObservableCollection<Plat> Plats { get; set; } = new();

    /// <summary>
    /// Liste des cuisiniers disponibles (hors client connecté).
    /// </summary>
    public ObservableCollection<User> Cuisiniers { get; set; } = new();

    private readonly NClientViewModel _clientVM;

    private User _cuisinierSelectionne;

    /// <summary>
    /// Cuisinier sélectionné par le client (filtre principal).
    /// </summary>
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

    /// <summary>
    /// Type de plat sélectionné (Entrée, Plat, Dessert, Tous).
    /// </summary>
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

    /// <summary>
    /// Détermine si le tri est effectué du moins cher au plus cher.
    /// </summary>
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

    /// <summary>
    /// Types de plats disponibles pour filtrage.
    /// </summary>
    public ObservableCollection<string> TypesDisponibles { get; } = new() { "Tous", "Entrée", "Plat", "Dessert" };

    /// <summary>
    /// Régimes alimentaires disponibles.
    /// </summary>
    public ObservableCollection<string> RegimesDisponibles { get; } = new();

    /// <summary>
    /// Régimes actuellement sélectionnés pour le filtrage.
    /// </summary>
    public ObservableCollection<string> RegimesFiltres { get; set; } = new();

    /// <summary>
    /// Commande pour appliquer les filtres de type et de régime.
    /// </summary>
    public ICommand AppliquerFiltresCommand { get; }

    /// <summary>
    /// Commande pour ajouter un plat au panier.
    /// </summary>
    public ICommand AjouterAuPanierCommand { get; }

    /// <summary>
    /// Commande pour réinitialiser tous les filtres.
    /// </summary>
    public ICommand ResetFiltresCommand { get; }

    /// <summary>
    /// Initialise une nouvelle instance du <see cref="PlatsViewModel"/>.
    /// </summary>
    /// <param name="clientVM">Le ViewModel client qui utilise cette vue.</param>
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

    /// <summary>
    /// Recharge les plats depuis la base de données (en appliquant les filtres actifs).
    /// </summary>
    private void ChargerPlatsDepuisBDD()
    {
        FiltrerEtTrierPlats();
    }

    /// <summary>
    /// Applique les filtres sélectionnés (type, régimes) et trie les plats par prix.
    /// Exclut les plats déjà présents dans le panier.
    /// </summary>
    public void FiltrerEtTrierPlats()
    {
        Plats.Clear();

        var db = Database.Instance;
        var tousLesPlats = Plat.GetDisponibles(db, CuisinierSelectionne?.UserId);

        var platsFiltres = tousLesPlats
            .Where(p => _clientVM.Panier.All(panierPlat => panierPlat.PlatId != p.PlatId))
            .Where(p =>
                (TypeFiltre == "Tous" || p.Recette.Type == TypeFiltre) &&
                (RegimesFiltres.Count == 0 || RegimesFiltres.All(r => p.Recette.RegimesNoms.Contains(r)))
            );

        platsFiltres = TriPrixCroissant
            ? platsFiltres.OrderBy(p => p.PrixParPersonne)
            : platsFiltres.OrderByDescending(p => p.PrixParPersonne);

        foreach (var plat in platsFiltres)
            Plats.Add(plat);
    }

    /// <summary>
    /// Retire un plat de la liste affichée (ex: après ajout au panier).
    /// </summary>
    /// <param name="plat">Plat à retirer de la vue.</param>
    public void RetirerPlatDisponible(Plat plat)
    {
        Plats.Remove(plat);
    }
}
