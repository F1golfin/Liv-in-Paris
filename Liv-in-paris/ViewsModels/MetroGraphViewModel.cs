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
    private readonly AdresseService _adresseService = new();
    
    /// <summary>
    /// Liste des stations disponibles pour la sélection.
    /// </summary>
    public ObservableCollection<Station> Stations { get; } = new();

    /// <summary>
    /// Liste des algorithmes de calcul de plus court chemin proposés.
    /// </summary>
    public ObservableCollection<string> Algorithmes { get; }

    public string AdresseDepart { get; set; } = "";
    public List<string> AdressesLivraison { get; set; } = new();
    private string _algoSelectionne;
    private string _resumeTrajet;
    
    public Station? StationDepartCalculee { get; private set; }
    public List<Station> StationsLivraisonCalculees { get; private set; } = new();

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

    public void InitialiserAdresses(string depart, List<string> livraisons)
    {
        AdresseDepart = depart;
        AdressesLivraison = livraisons;
    }
    
    /// <summary>
    /// Méthode exécutée lors du clic sur "Calculer".
    /// Elle applique l'algorithme choisi sur les ID des stations sélectionnées,
    /// puis envoie le chemin trouvé à la vue et génère un résumé lisible.
    /// </summary>
    private async void CalculerChemin()
    {
        if (string.IsNullOrWhiteSpace(AlgoSelectionne))
            return;

        var stationDepart = await TrouverStationLaPlusProche(AdresseDepart);
        var stationsLivraison = new List<Station>();

        foreach (var adresse in AdressesLivraison)
        {
            var s = await TrouverStationLaPlusProche(adresse);
            if (s != null)
                stationsLivraison.Add(s);
        }

        if (stationDepart == null || stationsLivraison.Count == 0)
        {
            ResumeTrajet = "Impossible de géocoder les adresses.";
            return;
        }
        
        var cheminComplet = new List<int>();
        var idsNonVisites = new HashSet<int>(
            Graphe.Noeuds
                .Where(kvp => stationsLivraison.Contains(kvp.Value.Data))
                .Select(kvp => kvp.Key)
        );
        var currentId = Graphe.Noeuds.First(n => n.Value.Data.Nom == stationDepart.Nom).Key;

        while (idsNonVisites.Count > 0)
        {
            int prochainId = -1;
            List<int> meilleurChemin = new();
            int meilleurPoids = int.MaxValue;

            foreach (var cible in idsNonVisites)
            {
                List<int> chemin = AlgoSelectionne switch
                {
                    "Dijkstra" => Graphe.Dijkstra(currentId, cible),
                    "Bellman-Ford" => Graphe.BellmanFord(currentId, cible),
                    "Floyd-Warshall" => Graphe.CheminLePlusCourt(currentId, cible),
                    _ => new List<int>()
                };

                int poids = Graphe.CalculerPoids(chemin);
                if (chemin.Count > 0 && poids < meilleurPoids)
                {
                    meilleurPoids = poids;
                    meilleurChemin = chemin;
                    prochainId = cible;
                }
            }

            if (meilleurChemin.Count == 0)
                break;

            // Ne répète pas les doublons
            if (cheminComplet.Count > 0 && meilleurChemin[0] == cheminComplet.Last())
                meilleurChemin.RemoveAt(0);

            cheminComplet.AddRange(meilleurChemin);
            idsNonVisites.Remove(prochainId);
            currentId = prochainId;
        }

        if (cheminComplet.Count > 0)
        {
            OnCheminCalcule?.Invoke(cheminComplet);

            var noms = cheminComplet.Select(id => Graphe.Noeuds[id].Data.ToString()).ToList();
            int total = Graphe.CalculerPoids(cheminComplet);

            ResumeTrajet = $" Itinéraire :\n{string.Join(" → ", noms)}\n\nDurée estimée : {total} min";
        }
        else
        {
            ResumeTrajet = "Aucun chemin trouvé.";
        }
        
        StationDepartCalculee = stationDepart;
        StationsLivraisonCalculees = stationsLivraison;
    }
    
    /// <summary>
    /// Renvoie la station de métro la plus proche des coordonnées GPS données.
    /// </summary>
    public Station? TrouverStationLaPlusProche(double latitude, double longitude)
    {
        Station? stationProche = null;
        double distanceMin = double.MaxValue;

        foreach (var station in Stations)
        {
            double dist = DistanceHaversine(latitude, longitude, station.Latitude, station.Longitude);
            if (dist < distanceMin)
            {
                distanceMin = dist;
                stationProche = station;
            }
        }

        return stationProche;
    }

    /// <summary>
    /// Calcule la distance entre deux points GPS avec la formule de Haversine (en km).
    /// </summary>
    private double DistanceHaversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
    
    /// <summary>
    /// Géocode une adresse et retourne la station de métro la plus proche.
    /// </summary>
    public async Task<Station?> TrouverStationLaPlusProche(string adresse)
    {
        var coord = await _adresseService.ObtenirCoordonneesAsync(adresse);
        if (coord == null)
            return null;

        return TrouverStationLaPlusProche(coord.Value.lat, coord.Value.lon);
    }

    
}
