using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel de la vue du panier côté client.
/// Gère l'affichage des plats sélectionnés, leur suppression et la validation de la commande.
/// </summary>
public class PanierViewModel : ViewModelBase
{
    private readonly User _utilisateur;
    private readonly NClientViewModel _client;
    private readonly ObservableCollection<PlatCommandeViewModel> _panier;

    /// <summary>
    /// Plats affichés dans le panier, encapsulés dans des ViewModels pour gérer l’adresse et l’horaire.
    /// </summary>
    public ObservableCollection<PlatCommandeViewModel> Panier => _panier;

    /// <summary>
    /// Prix total calculé dynamiquement à partir du panier.
    /// </summary>
    public decimal PrixTotal => Panier.Sum(p => p.Plat.PrixParPersonne);

    /// <summary>
    /// Commande pour retirer un plat du panier.
    /// </summary>
    public ICommand RetirerDuPanierCommand { get; }

    /// <summary>
    /// Commande pour valider la commande (enregistrer les lignes, vérifier les adresses).
    /// </summary>
    public ICommand PasserCommandeCommand { get; }

    /// <summary>
    /// Initialise une nouvelle instance du <see cref="PanierViewModel"/>.
    /// </summary>
    /// <param name="panier">Collection initiale de plats sélectionnés.</param>
    /// <param name="utilisateur">Utilisateur client actuel.</param>
    /// <param name="clientVM">ViewModel principal du client.</param>
    public PanierViewModel(ObservableCollection<Plat> panier, User utilisateur, NClientViewModel clientVM)
    {
        _utilisateur = utilisateur;
        _client = clientVM;
        _panier = new ObservableCollection<PlatCommandeViewModel>();

        foreach (var plat in panier)
        {
            _panier.Add(new PlatCommandeViewModel(plat, utilisateur.Adresse));
        }

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

    /// <summary>
    /// Supprime un plat du panier et de la base, et le réaffiche dans la liste des plats disponibles.
    /// </summary>
    /// <param name="platVM">Plat à retirer.</param>
    private void RetirerDuPanier(PlatCommandeViewModel platVM)
    {
        Panier.Remove(platVM);

        var db = Database.Instance;
        LigneCommande.SupprimerParPlatId(db, platVM.Plat.PlatId);
        _client.Panier.Remove(platVM.Plat);

        if (_client.PlatsVue is PlatsView vue && vue.DataContext is PlatsViewModel platsVM)
        {
            platsVM.FiltrerEtTrierPlats();
        }

        OnPropertyChanged(nameof(PrixTotal));
    }

    /// <summary>
    /// Valide la commande : vérifie chaque adresse, enregistre la commande et les lignes dans la base,
    /// puis met à jour les vues.
    /// </summary>
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
}
