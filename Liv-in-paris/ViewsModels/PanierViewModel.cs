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

        _panier = new ObservableCollection<PlatCommandeViewModel>();

        foreach (var plat in panier)
        {
            _panier.Add(new PlatCommandeViewModel(plat, utilisateur.Adresse));
        }

        // écoute les ajouts futurs dans le panier du client
        panier.CollectionChanged += (sender, e) =>
        {
            if (e.NewItems != null)
            {
                foreach (Plat plat in e.NewItems)
                {
                    _panier.Add(new PlatCommandeViewModel(plat, utilisateur.Adresse));
                }
            }

            if (e.OldItems != null)
            {
                foreach (Plat plat in e.OldItems)
                {
                    var toRemove = _panier.FirstOrDefault(vm => vm.Plat.PlatId == plat.PlatId);
                    if (toRemove != null)
                        _panier.Remove(toRemove);
                }
            }

            OnPropertyChanged(nameof(PrixTotal));
        };

        RetirerDuPanierCommand = new RelayCommand<PlatCommandeViewModel>(RetirerDuPanier);
        PasserCommandeCommand = new RelayCommand(async () => await PasserCommandeAsync());

        _panier.CollectionChanged += (_, _) => OnPropertyChanged(nameof(PrixTotal));
    }

    private void RetirerDuPanier(PlatCommandeViewModel platVM)
    {
        Panier.Remove(platVM);

        var db = Database.Instance;
        LigneCommande.SupprimerParPlatId(db, platVM.Plat.PlatId);
        _client.Panier.Remove(platVM.Plat);
        
        // Réaffiche le plat dans la liste disponible
        if (_client.PlatsVue is PlatsView vue && vue.DataContext is PlatsViewModel platsVM)
        {
            platsVM.FiltrerEtTrierPlats();
        }

        OnPropertyChanged(nameof(PrixTotal));
    }

    private async Task PasserCommandeAsync()
    {
        if (!Panier.Any())
        {
            MessageBox.Show("Votre panier est vide.");
            return;
        }

        var service = new AdresseService();

        foreach (var platVM in Panier)
        {
            bool estValide = await service.EstAdresseValideAsync(platVM.AdresseLivraison);
            if (!estValide)
            {
                MessageBox.Show($"L'adresse suivante n'est pas valide :\n{platVM.AdresseLivraison}");
                return;
            }
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
                AdresseDepart = Commande.GetAdresseUser(db, Panier.First().Plat.CuisinierId)
            };
            commande.AjouterCommande(db);

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

            if (_client.CommandesVue is CommandesView commandesView && commandesView.DataContext is CommandesViewModel commandesVM)
            {
                commandesVM.RechargerCommandes();
            }
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
