using System.ComponentModel;
using System.Runtime.CompilerServices;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public class PlatCommandeViewModel : INotifyPropertyChanged
{
    public Plat Plat { get; set; }

    private string _adresseLivraison;
    public string AdresseLivraison
    {
        get => _adresseLivraison;
        set { _adresseLivraison = value; OnPropertyChanged(); }
    }

    private DateTime _heureLivraison;
    public DateTime HeureLivraison
    {
        get => _heureLivraison;
        set { _heureLivraison = value; OnPropertyChanged(); }
    }

    public PlatCommandeViewModel(Plat plat, string adresseClient)
    {
        Plat = plat;
        AdresseLivraison = adresseClient;
        HeureLivraison = DateTime.Now.AddHours(2);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}