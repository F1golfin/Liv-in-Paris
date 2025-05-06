using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel associé à un plat dans le panier.
/// Contient l’adresse de livraison, l’horaire de livraison et les suggestions dynamiques.
/// </summary>
public class PlatCommandeViewModel : ViewModelBase
{
    /// <summary>
    /// Le plat associé à cette ligne de commande.
    /// </summary>
    public Plat Plat { get; set; }

    private string _adresseLivraison;

    /// <summary>
    /// Adresse de livraison saisie par le client.
    /// Déclenche la recherche de suggestions à chaque modification.
    /// </summary>
    public string AdresseLivraison
    {
        get => _adresseLivraison;
        set
        {
            if (_adresseLivraison != value)
            {
                _adresseLivraison = value;
                OnPropertyChanged();
                _ = ChargerSuggestionsAsync(value); // déclenche la recherche de suggestions sans attendre le résultat
            }
        }
    }

    private DateTime _heureLivraison;

    /// <summary>
    /// Heure de livraison souhaitée pour le plat.
    /// </summary>
    public DateTime HeureLivraison
    {
        get => _heureLivraison;
        set { _heureLivraison = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Liste des suggestions d’adresse obtenues dynamiquement.
    /// </summary>
    public ObservableCollection<string> Suggestions { get; set; } = new();

    private readonly AdresseService _adresseService = new();

    /// <summary>
    /// Initialise une nouvelle instance du <see cref="PlatCommandeViewModel"/> avec un plat et une adresse par défaut.
    /// </summary>
    /// <param name="plat">Le plat à livrer.</param>
    /// <param name="adresseClient">Adresse du client utilisée comme valeur initiale.</param>
    public PlatCommandeViewModel(Plat plat, string adresseClient)
    {
        Plat = plat;
        AdresseLivraison = adresseClient;
        HeureLivraison = DateTime.Now.AddHours(2);
    }

    /// <summary>
    /// Charge des suggestions d’adresse à partir d’une saisie utilisateur.
    /// Nécessite au moins 3 caractères.
    /// </summary>
    /// <param name="saisie">Texte saisi par l’utilisateur.</param>
    public async Task ChargerSuggestionsAsync(string saisie)
    {
        if (string.IsNullOrWhiteSpace(saisie) || saisie.Length < 3) return;

        var results = await _adresseService.ObtenirSuggestionsAsync(saisie);

        Suggestions.Clear();
        foreach (var r in results)
            Suggestions.Add(r);
    }

    /// <summary>
    /// Vérifie si l’adresse saisie est valide (i.e., a pu être géocodée).
    /// </summary>
    /// <returns><c>true</c> si l’adresse a pu être convertie en coordonnées GPS ; sinon <c>false</c>.</returns>
    public async Task<bool> AdresseEstValideAsync()
    {
        var coords = await _adresseService.ObtenirCoordonneesAsync(AdresseLivraison);
        return coords != null;
    }
}
