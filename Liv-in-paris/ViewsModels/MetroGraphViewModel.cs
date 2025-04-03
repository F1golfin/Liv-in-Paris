using System.Collections.ObjectModel;
using System.Windows.Input;
using Liv_in_paris.Core.Entities;
using Liv_in_paris.Core.Graph;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel responsable de l'affichage et du traitement du graphe du métro de Paris.
/// Il gère la sélection des stations, le choix de l'algorithme, et l'appel au calcul de trajet.
/// </summary>
public class MetroGraphViewModel : ViewModelBase
{
    /// <summary>
    /// Liste des stations disponibles pour sélection (chargée depuis le graphe).
    /// </summary>
    public ObservableCollection<Station> Stations { get; } = new();

    /// <summary>
    /// Liste des algorithmes disponibles pour le calcul de trajet.
    /// </summary>
    public ObservableCollection<string> Algorithmes { get; }

    private Station _stationDepart;

    /// <summary>
    /// Station de départ sélectionnée par l'utilisateur.
    /// </summary>
    public Station StationDepart
    {
        get => _stationDepart;
        set { _stationDepart = value; OnPropertyChanged(); }
    }

    private Station _stationArrivee;

    /// <summary>
    /// Station d'arrivée sélectionnée par l'utilisateur.
    /// </summary>
    public Station StationArrivee
    {
        get => _stationArrivee;
        set { _stationArrivee = value; OnPropertyChanged(); }
    }

    private string _algoSelectionne;
    
    private string _resumeTrajet;

    public string ResumeTrajet
    {
        get => _resumeTrajet;
        set { _resumeTrajet = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Algorithme de plus court chemin sélectionné.
    /// </summary>
    public string AlgoSelectionne
    {
        get => _algoSelectionne;
        set { _algoSelectionne = value; OnPropertyChanged(); }
    }
    
    /// <summary>
    /// Action appelée une fois le chemin calculé, pour l'affichage (liaison avec la vue).
    /// </summary>
    public Action<List<int>>? OnCheminCalcule { get; set; }

    /// <summary>
    /// Commande déclenchée lorsqu'on clique sur "Calculer" (détermine le plus court chemin).
    /// </summary>
    public ICommand CalculerCheminCommand { get; }

    /// <summary>
    /// Représente le graphe du métro (chargé à partir des fichiers CSV).
    /// </summary>
    private readonly Graphe<Station> _graphe;
    public Graphe<Station> Graphe => _graphe;

    /// <summary>
    /// Initialise le ViewModel : charge le graphe, les stations, et configure la commande.
    /// </summary>
    public MetroGraphViewModel()
    {
        // Liste des algos disponibles
        Algorithmes = new ObservableCollection<string>
        {
            "Bellman-Ford", "Dijkstra", "Floyd-Warshall"
        };

        // Commande bouton "Calculer"
        CalculerCheminCommand = new RelayCommand(CalculerChemin);

        // Chargement du graphe depuis les CSV
        string chemin1 = "../../../../Files/MetroParis_onglet1.csv";
        string chemin2 = "../../../../Files/MetroParis_onglet2.csv";
        _graphe = GrapheMetroBuilder.ConstruireDepuisCSV(chemin1, chemin2);

        // Remplissage de la liste de stations à afficher
        var stationsTriees = _graphe.Noeuds.Values
            .Select(n => n.Data)
            .DistinctBy(s => s.Nom)
            .OrderBy(s => s.Nom);

        foreach (var station in stationsTriees)
            Stations.Add(station);
    }

    /// <summary>
    /// Méthode appelée lors du clic sur le bouton "Calculer".
    /// Permettra d'exécuter l'algorithme sélectionné sur les stations choisies.
    /// </summary>
    private void CalculerChemin()
    {
        if (string.IsNullOrWhiteSpace(AlgoSelectionne))
            return;

        // Trouve les IDs associés aux stations sélectionnées
        var idsDepart = _graphe.Noeuds.Values.Where(n => n.Data.Nom == StationDepart.Nom).Select(n => n.Id).ToList();
        var idsArrivee = _graphe.Noeuds.Values.Where(n => n.Data.Nom == StationArrivee.Nom).Select(n => n.Id).ToList();
        
        List<int> meilleurChemin = new();
        int meilleurPoids = int.MaxValue;

        List<int> chemin = new List<int>();

        foreach (var dep in idsDepart)
        {
            foreach (var arr in idsArrivee)
            {

                switch (_algoSelectionne)
                {
                    case "Dijkstra":
                        chemin = _graphe.Dijkstra(dep, arr);
                        break;
                    case "Bellman-Ford":
                        chemin = _graphe.BellmanFord(dep, arr);
                        break;
                    case "Floyd-Warshall":
                        chemin = _graphe.CheminLePlusCourt(dep, arr);
                        break;
                }
                
                int poids = _graphe.CalculerPoids(chemin);

                if (chemin.Count > 0 && poids < meilleurPoids)
                {
                    meilleurPoids = poids;
                    meilleurChemin = chemin;
                }
            }
        }
        
        // Envoie le chemin à la vue
        OnCheminCalcule?.Invoke(chemin);
        
        if (meilleurChemin.Count > 0)
        {
            var stations = meilleurChemin
                .Select(id => _graphe.Noeuds[id].Data.ToString())
                .ToList();

            int poidsTotal = _graphe.CalculerPoids(meilleurChemin);

            ResumeTrajet = $"Trajet : " + string.Join(" → ", stations) + $"\n\nTemps total estimé : {poidsTotal} minutes";
        }
        else
        {
            ResumeTrajet = "Aucun chemin trouvé.";
        }
        
    }
}
