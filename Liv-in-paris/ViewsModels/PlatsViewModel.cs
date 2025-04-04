using Liv_in_paris;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

public class PlatsViewModel : ViewModelBase
{
    public ObservableCollection<Plat> Plats { get; set; } = new();
    
    private readonly ClientViewModel _clientVM;
    
    public ICommand AjouterAuPanierCommand => new RelayCommand<Plat>(plat =>
    {
        _clientVM.AjouterAuPanier(plat);
    });

    public PlatsViewModel()
    {
        ChargerPlatsDepuisBDD();
    }

    private void ChargerPlatsDepuisBDD()
    {
        var db = new DatabaseManager("localhost", "livin_paris", "root", "root");
        var table = db.ExecuteQuery("SELECT * FROM plats");

        foreach (System.Data.DataRow row in table.Rows)
        {
            var plat = new Plat
            {
                PlatId = Convert.ToUInt64(row["plat_id"]),
                NomPlat = row["nom_plat"].ToString(),
                PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]),
                NbParts = Convert.ToInt32(row["nb_parts"]),
                //Photo = row["photo"].ToString() // lien d'image locale ou en ligne
            };
            Plats.Add(plat);
            Console.WriteLine(plat.NomPlat);
        }
    }
    
    public PlatsViewModel(ClientViewModel clientVM)
    {
        _clientVM = clientVM;
        ChargerPlatsDepuisBDD();
    }
}