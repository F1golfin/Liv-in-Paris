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
            var db = Database.Instance;

            var commande = new Commande
            {
                HeureCommande = DateTime.Now,
                PrixTotal = PrixTotal,
                CuisinierId = Panier.First().CuisinierId,
                ClientId = _utilisateur.UserId,
                AdresseDepart = Commande.GetAdresseUser(db, Panier.First().CuisinierId),
                Lignes = new List<LigneCommande>()
            };

            foreach (var plat in Panier)
            {
                commande.Lignes.Add(new LigneCommande
                {
                    PlatId = plat.PlatId,
                    AdresseArrivee = _utilisateur.Adresse, // Par défaut, adresse du client
                    HeureLivraison = DateTime.Now.AddHours(2), // Par défaut : 2h après la commande
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
