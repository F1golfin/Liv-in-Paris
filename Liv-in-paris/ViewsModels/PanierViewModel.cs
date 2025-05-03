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
    private readonly NClientViewModel _client;

    private readonly ObservableCollection<PlatCommandeViewModel> _panier;
    public ObservableCollection<PlatCommandeViewModel> Panier => _panier;

    public decimal PrixTotal => Panier.Sum(p => p.Plat.PrixParPersonne);

    public ICommand RetirerDuPanierCommand { get; }
    public ICommand PasserCommandeCommand { get; }

    public PanierViewModel(ObservableCollection<Plat> panier, User utilisateur, NClientViewModel clientVM)
    {
        _utilisateur = utilisateur;
        _client = clientVM;

        _panier = new ObservableCollection<PlatCommandeViewModel>(
            panier.Select(p => new PlatCommandeViewModel(p, utilisateur.Adresse)));

        RetirerDuPanierCommand = new RelayCommand<PlatCommandeViewModel>(RetirerDuPanier);
        PasserCommandeCommand = new RelayCommand(PasserCommande);

        _panier.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrixTotal));
    }

    private void RetirerDuPanier(PlatCommandeViewModel platVM)
    {
        Panier.Remove(platVM);

        var db = Database.Instance;
        LigneCommande.SupprimerParPlatId(db, platVM.Plat.PlatId);
        _client.Panier.Remove(platVM.Plat);

        OnPropertyChanged(nameof(PrixTotal));
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

            //Créer la commande
            var commande = new Commande
            {
                HeureCommande = DateTime.Now,
                PrixTotal = PrixTotal,
                CuisinierId = Panier.First().Plat.CuisinierId,
                ClientId = _utilisateur.UserId,
                AdresseDepart = Commande.GetAdresseUser(db, Panier.First().Plat.CuisinierId)
            };
            commande.AjouterCommande(db); // récupère CommandeId

            //Mettre à jour les lignes existantes
            foreach (var platVM in Panier)
            {
                LigneCommande ligne = LigneCommande.GetByPlatId(db, platVM.Plat.PlatId);
                if (ligne != null)
                {
                    ligne.AdresseArrivee = platVM.AdresseLivraison;
                    ligne.HeureLivraison = platVM.HeureLivraison;
                    ligne.Statut = "Commandee";
                    ligne.CommandeId = commande.CommandeId;
                    ligne.ModifierCommande(db);
                }
            }

            MessageBox.Show("✅ Commande enregistrée !");
            Panier.Clear();
            _client.Panier.Clear();
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
