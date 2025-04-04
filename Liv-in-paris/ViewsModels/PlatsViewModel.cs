using Liv_in_paris;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

public class PlatsViewModel : ViewModelBase
{
    public ObservableCollection<Plat> Plats { get; set; } = new();
    
    private readonly ClientViewModel _clientVM;
    

    public ObservableCollection<User> Cuisiniers { get; set; } = new();
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
    
    public ICommand AjouterAuPanierCommand => new RelayCommand<Plat>(plat =>
    {
        _clientVM.AjouterAuPanier(plat);
    });

    public PlatsViewModel(ClientViewModel clientVM)
    {
        _clientVM = clientVM;
        
        // Chargement des cuisiniers
        var db = new DatabaseManager("localhost", "livin_paris", "root", "root");
        var table = db.ExecuteQuery("SELECT * FROM users WHERE role LIKE '%Cuisinier%'");

        foreach (DataRow row in table.Rows)
        {
            Cuisiniers.Add(new User
            {
                UserId = Convert.ToUInt64(row["user_id"]),
                Prenom = row["prenom"].ToString(),
                Nom = row["nom"].ToString()
            });
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
        
        var db = new DatabaseManager("localhost", "livin_paris", "root", "root");
        string query = $"SELECT * FROM plats WHERE commande_id IS NULL AND cuisinier_id = {CuisinierSelectionne.UserId}";
        var table = db.ExecuteQuery(query);

        foreach (DataRow row in table.Rows)
        {
            Plats.Add(new Plat
            {
                PlatId = Convert.ToUInt64(row["plat_id"]),
                NomPlat = row["nom_plat"].ToString(),
                PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]),
                NbParts = Convert.ToInt32(row["nb_parts"]),
                CuisinierId = Convert.ToUInt64(row["cuisinier_id"])
            });
        }
    }
}