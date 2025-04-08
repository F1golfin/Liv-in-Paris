using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public class PanierViewModel : INotifyPropertyChanged
{
    private readonly ObservableCollection<Plat> _panier;
    private readonly User _utilisateur;
    private readonly AppViewModel _app;

    public ObservableCollection<Plat> Panier => _panier;

    public decimal PrixTotal => Panier.Sum(p => p.PrixParPersonne);

    public ICommand RetirerDuPanierCommand { get; }
    public ICommand PasserCommandeCommand { get; }

    public PanierViewModel(ObservableCollection<Plat> panier, User utilisateur, AppViewModel app)
    {
        _panier = panier;
        _utilisateur = utilisateur;
        _app = app;

        RetirerDuPanierCommand = new RelayCommand<Plat>(RetirerDuPanier);
        PasserCommandeCommand = new RelayCommand(PasserCommande);

        _panier.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrixTotal));
    }

    private void RetirerDuPanier(Plat plat)
    {
        Panier.Remove(plat);
    }

    private void PasserCommande()
    {
        if (!Panier.Any())
        {
            MessageBox.Show("Votre panier est vide.");
            return;
        }

        try
        {
            var cuisinierId = Panier.First().CuisinierId;
            var db = new DatabaseManager("localhost", "livin_paris", "root", "root");

            string insertCommande = $@"
                INSERT INTO commandes (heure_commande, adresse_depart, prix_total, client_id, cuisinier_id)
                VALUES (NOW(), 'Adresse factice', {PrixTotal.ToString().Replace(',', '.')}, {_utilisateur.UserId}, {cuisinierId});";

            db.ExecuteNonQuery(insertCommande);

            var result = db.ExecuteQuery("SELECT LAST_INSERT_ID() AS id;");
            int commandeId = Convert.ToInt32(result.Rows[0]["id"]);

            foreach (var plat in Panier)
            {
                string update = $"UPDATE plats SET commande_id = {commandeId} WHERE plat_id = {plat.PlatId};";
                db.ExecuteNonQuery(update);
            }

            MessageBox.Show("✅ Commande enregistrée !");
            Panier.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Erreur lors de la commande : " + ex.Message);
        }

        OnPropertyChanged(nameof(PrixTotal));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
