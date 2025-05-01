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
    
    public ICommand AjouterAuPanierCommand { get; }

    public PlatsViewModel(NClientViewModel clientVM)
    {
        _clientVM = clientVM;
        AjouterAuPanierCommand = new RelayCommand<Plat>(plat => _clientVM.AjouterAuPanier(plat));   
        var db = Database.Instance;
        
        foreach (User user in User.GetAllCuisinier(db))
        {
            Cuisiniers.Add(user);
        }
        
        if (Cuisiniers.Any())
        {
            CuisinierSelectionne = Cuisiniers[0];
        }
    }

    private void ChargerPlatsDepuisBDD()
    {
        Plats.Clear();

        if (CuisinierSelectionne == null)
        {
            Console.WriteLine("Aucun cuisinier sélectionné.");
            return;
        }

        var db = Database.Instance;
        foreach (Plat plat in Plat.GetDisponibles(db, CuisinierSelectionne.UserId))
        {
            Plats.Add(plat);
        }
    }
}