using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

public class PanierViewModel : INotifyPropertyChanged
{
    private readonly User _utilisateur;
    private readonly AppViewModel _app;

    private readonly ObservableCollection<PlatCommandeViewModel> _panier;
    public ObservableCollection<PlatCommandeViewModel> Panier => _panier;

    public decimal PrixTotal => Panier.Sum(p => p.Plat.PrixParPersonne);

    public ICommand RetirerDuPanierCommand { get; }
    public ICommand PasserCommandeCommand { get; }

    public PanierViewModel(ObservableCollection<Plat> panier, User utilisateur, AppViewModel app)
    {
        _utilisateur = utilisateur;
        _app = app;

        _panier = new ObservableCollection<PlatCommandeViewModel>(
            panier.Select(p => new PlatCommandeViewModel(p, utilisateur.Adresse)));

        RetirerDuPanierCommand = new RelayCommand<PlatCommandeViewModel>(RetirerDuPanier);
        PasserCommandeCommand = new RelayCommand(PasserCommande);

        _panier.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrixTotal));
    }

    private void RetirerDuPanier(PlatCommandeViewModel platVM)
    {
        Panier.Remove(platVM);
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
            var db = Database.Instance;

            var commande = new Commande
            {
                HeureCommande = DateTime.Now,
                PrixTotal = PrixTotal,
                CuisinierId = Panier.First().Plat.CuisinierId,
                ClientId = _utilisateur.UserId,
                AdresseDepart = Commande.GetAdresseUser(db, Panier.First().Plat.CuisinierId),
                Lignes = new List<LigneCommande>()
            };

            foreach (var plat in Panier)
            {
                commande.Lignes.Add(new LigneCommande
                {
                    PlatId = plat.Plat.PlatId,
                    AdresseArrivee = plat.AdresseLivraison,
                    HeureLivraison = plat.HeureLivraison,
                    Statut = "Commandee"
                });
            }

            commande.AjouterCommande(db);
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
