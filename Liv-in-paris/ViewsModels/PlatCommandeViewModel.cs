using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

public class PlatCommandeViewModel : INotifyPropertyChanged
{
    public Plat Plat { get; set; }

    private string _adresseLivraison;
    public string AdresseLivraison
    {
        get => _adresseLivraison;
        set
        {
            if (_adresseLivraison != value)
            {
                _adresseLivraison = value;
                OnPropertyChanged();
                _ = ChargerSuggestionsAsync(value); // lance la recherche sans attendre
            }
        }
    }

    private DateTime _heureLivraison;
    public DateTime HeureLivraison
    {
        get => _heureLivraison;
        set { _heureLivraison = value; OnPropertyChanged(); }
    }

    public ObservableCollection<string> Suggestions { get; set; } = new();
    private readonly AdresseService _adresseService = new();

    public PlatCommandeViewModel(Plat plat, string adresseClient)
    {
        Plat = plat;
        AdresseLivraison = adresseClient;
        HeureLivraison = DateTime.Now.AddHours(2);
    }

    public async Task ChargerSuggestionsAsync(string saisie)
    {
        if (string.IsNullOrWhiteSpace(saisie) || saisie.Length < 3) return;

        var results = await _adresseService.ObtenirSuggestionsAsync(saisie);

        Suggestions.Clear();
        foreach (var r in results)
            Suggestions.Add(r);
    }

    public async Task<bool> AdresseEstValideAsync()
    {
        var coords = await _adresseService.ObtenirCoordonneesAsync(AdresseLivraison);
        return coords != null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}