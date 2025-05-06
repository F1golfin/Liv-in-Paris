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
    /// Liste des stations disponibles pour la sélection.
    /// </summary>
    public ObservableCollection<Station> Stations { get; } = new();

    /// <summary>
    /// Liste des algorithmes de calcul de plus court chemin proposés.
    /// </summary>
    public ObservableCollection<string> Algorithmes { get; }

    private Station _stationDepart;
    private Station _stationArrivee;
    private string _algoSelectionne;
    private string _resumeTrajet;

    /// <summary>
    /// Station de départ sélectionnée par l'utilisateur.
    /// </summary>
    public Station StationDepart
    {
        get => _stationDepart;
        set { _stationDepart = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Station d'arrivée sélectionnée par l'utilisateur.
    /// </summary>
    public Station StationArrivee
    {
        get => _stationArrivee;
        set { _stationArrivee = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Algorithme sélectionné pour le calcul du plus court chemin.
    /// </summary>
    public string AlgoSelectionne
    {
        get => _algoSelectionne;
        set { _algoSelectionne = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Résumé du trajet calculé (liste des stations + durée).
    /// </summary>
    public string ResumeTrajet
    {
        get => _resumeTrajet;
        set { _resumeTrajet = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Action déléguée à la vue pour mettre en surbrillance le chemin calculé.
    /// </summary>
    public Action<List<int>>? OnCheminCalcule { get; set; }

    /// <summary>
    /// Commande exécutée lors du clic sur le bouton "Calculer".
    /// </summary>
    public ICommand CalculerCheminCommand { get; }

    /// <summary>
    /// Graphe du métro de Paris utilisé pour les calculs de trajets.
    /// </summary>
    private readonly Graphe<Station> _graphe;

    /// <summary>
    /// Expose le graphe au reste de l'application si nécessaire.
    /// </summary>
    public Graphe<Station> Graphe => _graphe;

    /// <summary>
    /// Initialise le ViewModel : charge les stations depuis les fichiers CSV,
    /// configure la liste d'algorithmes et la commande de calcul.
    /// </summary>
    public MetroGraphViewModel()
    {
        Algorithmes = new ObservableCollection<string>
        {
            "Bellman-Ford", "Dijkstra", "Floyd-Warshall"
        };

        CalculerCheminCommand = new RelayCommand(CalculerChemin);

        string chemin1 = "../../../../Files/MetroParis_onglet1.csv";
        string chemin2 = "../../../../Files/MetroParis_onglet2.csv";
        _graphe = GrapheMetroBuilder.ConstruireDepuisCSV(chemin1, chemin2);

        var stationsTriees = _graphe.Noeuds.Values
            .Select(n => n.Data)
            .DistinctBy(s => s.Nom)
            .OrderBy(s => s.Nom);

        foreach (var station in stationsTriees)
            Stations.Add(station);
    }

    /// <summary>
    /// Méthode exécutée lors du clic sur "Calculer".
    /// Elle applique l'algorithme choisi sur les ID des stations sélectionnées,
    /// puis envoie le chemin trouvé à la vue et génère un résumé lisible.
    /// </summary>
    private void CalculerChemin()
    {
        if (string.IsNullOrWhiteSpace(AlgoSelectionne))
            return;

        var idsDepart = _graphe.Noeuds.Values.Where(n => n.Data.Nom == StationDepart.Nom).Select(n => n.Id).ToList();
        var idsArrivee = _graphe.Noeuds.Values.Where(n => n.Data.Nom == StationArrivee.Nom).Select(n => n.Id).ToList();

        List<int> meilleurChemin = new();
        int meilleurPoids = int.MaxValue;
        List<int> chemin = new();

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

        OnCheminCalcule?.Invoke(chemin);

        if (meilleurChemin.Count > 0)
        {
            var stations = meilleurChemin
                .Select(id => _graphe.Noeuds[id].Data.ToString())
                .ToList();

            int poidsTotal = _graphe.CalculerPoids(meilleurChemin);

            ResumeTrajet = $"Trajet : {string.Join(" → ", stations)}\n\nTemps total estimé : {poidsTotal} minutes";
        }
        else
        {
            ResumeTrajet = "Aucun chemin trouvé.";
        }
    }
}
