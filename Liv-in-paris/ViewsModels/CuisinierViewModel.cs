using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Views;

namespace Liv_in_paris
{
    /// <summary>
    /// ViewModel destiné aux cuisiniers, permettant la gestion des plats, des recettes, des commandes, et des évaluations.
    /// </summary>
    public class CuisinierViewModel : ViewModelBase
    {
        private readonly AppViewModel _app;
        private readonly User _utilisateurConnecte;

        /// <summary>
        /// Liste observable des plats du cuisinier.
        /// </summary>
        public ObservableCollection<Plat> Plats { get; set; }

        /// <summary>
        /// Liste observable des recettes existantes dans la base.
        /// </summary>
        public ObservableCollection<Recette> RecettesExistantes { get; set; }

        /// <summary>
        /// Nom du nouveau plat à ajouter.
        /// </summary>
        public string NewNomPlat { get; set; }

        /// <summary>
        /// Prix par personne du nouveau plat.
        /// </summary>
        public string NewPrixPlat { get; set; }

        /// <summary>
        /// Type du plat (entrée, plat principal, dessert...).
        /// </summary>
        public string NewTypePlat { get; set; }

        /// <summary>
        /// Recette sélectionnée pour le nouveau plat.
        /// </summary>
        public Recette RecetteSelectionnee { get; set; }

        private string _newNbParts;

        /// <summary>
        /// Nombre de parts du plat à ajouter.
        /// </summary>
        public string NewNbParts
        {
            get => _newNbParts;
            set
            {
                _newNbParts = value;
                OnPropertyChanged(nameof(NewNbParts));
            }
        }

        /// <summary>
        /// Liste des évaluations reçues par le cuisinier.
        /// </summary>
        public ObservableCollection<Evaluation> EvaluationsRecues { get; set; }

        /// <summary>
        /// Liste des commandes à livrer ou déjà livrées.
        /// </summary>
        public ObservableCollection<Commande> Commandes { get; set; }

        /// <summary>
        /// Commande pour ajouter un plat.
        /// </summary>
        public ICommand AjouterPlatCommand { get; }

        /// <summary>
        /// Commande pour ouvrir la fenêtre de création de recette.
        /// </summary>
        public ICommand AjouterNouvelleRecetteCommand { get; }

        /// <summary>
        /// Commande pour se déconnecter.
        /// </summary>
        public ICommand DeconnexionCommand { get; }

        /// <summary>
        /// Commande pour mettre à jour le statut d'une ligne de commande.
        /// </summary>
        public ICommand MettreAJourStatutCommand { get; }

        /// <summary>
        /// Commande pour supprimer un plat existant.
        /// </summary>
        public ICommand SupprimerPlatCommand { get; }

        /// <summary>
        /// Initialise une nouvelle instance du <see cref="CuisinierViewModel"/>.
        /// </summary>
        /// <param name="parent">ViewModel principal de l'application.</param>
        /// <param name="utilisateur">Utilisateur connecté (doit être cuisinier).</param>
        public CuisinierViewModel(AppViewModel parent, User utilisateur)
        {
            _app = parent;
            _utilisateurConnecte = utilisateur;

            if (!_utilisateurConnecte.Role.ToLower().Contains("cuisinier"))
            {
                MessageBox.Show("Accès réservé aux cuisiniers.");
                return;
            }

            AjouterPlatCommand = new RelayCommand(AjouterPlat);
            AjouterNouvelleRecetteCommand = new RelayCommand(AjouterNouvelleRecette);
            DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());

            ChargerDonnees();

            MettreAJourStatutCommand = new RelayCommand<LigneCommande>(ligne =>
            {
                var db = Database.Instance;
                ligne.MettreAJourStatut(db, ligne.Statut);
                ChargerCommandes();
            });

            SupprimerPlatCommand = new RelayCommand<Plat>(plat =>
            {
                var result = MessageBox.Show($"Supprimer le plat « {plat.NomPlat} » ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var db = Database.Instance;
                    plat.SupprimerPlat(db);
                    ChargerDonnees();
                }
            });
        }

        /// <summary>
        /// Charge tous les plats, recettes et évaluations du cuisinier depuis la base de données.
        /// </summary>
        private void ChargerDonnees()
        {
            var db = Database.Instance;

            Plats = new ObservableCollection<Plat>(Plat.GetAllByCuisinier(db, _utilisateurConnecte.UserId));
            RecettesExistantes = new ObservableCollection<Recette>(Recette.GetAll(db));
            OnPropertyChanged(nameof(Plats));
            OnPropertyChanged(nameof(RecettesExistantes));

            EvaluationsRecues = new ObservableCollection<Evaluation>(
                Evaluation.GetByCuisinier(db, _utilisateurConnecte.UserId)
            );
            OnPropertyChanged(nameof(EvaluationsRecues));

            ChargerCommandes();
        }

        /// <summary>
        /// Tente d’ajouter un nouveau plat à la base de données après validation des champs saisis.
        /// </summary>
        private void AjouterPlat()
        {
            if (string.IsNullOrWhiteSpace(NewNomPlat) || string.IsNullOrWhiteSpace(NewPrixPlat) || string.IsNullOrWhiteSpace(NewTypePlat))
            {
                MessageBox.Show("Veuillez renseigner le nom, le prix et le type du plat.");
                return;
            }

            if (!decimal.TryParse(NewPrixPlat, out decimal prix))
            {
                MessageBox.Show("Prix invalide.");
                return;
            }

            if (!int.TryParse(NewNbParts, out int nbParts) || nbParts <= 0)
            {
                MessageBox.Show("Nombre de parts invalide.");
                return;
            }

            var db = Database.Instance;
            var nouveauPlat = new Plat
            {
                NomPlat = NewNomPlat,
                PrixParPersonne = prix,
                NbParts = nbParts,
                DateFabrication = DateTime.Now,
                DatePeremption = DateTime.Now.AddDays(3),
                CuisinierId = _utilisateurConnecte.UserId,
                RecetteId = RecetteSelectionnee?.RecetteId ?? 0
            };

            nouveauPlat.AjouterPlat(db);
            MessageBox.Show("Plat ajouté !");
            ChargerDonnees();
        }

        /// <summary>
        /// Ouvre la fenêtre de création d’une nouvelle recette.
        /// </summary>
        private void AjouterNouvelleRecette()
        {
            var fenetre = new NouvelleRecetteWindow();
            fenetre.ShowDialog();
            ChargerDonnees();
        }

        /// <summary>
        /// Recharge les commandes associées au cuisinier.
        /// </summary>
        private void ChargerCommandes()
        {
            var db = Database.Instance;
            Commandes = new ObservableCollection<Commande>(
                Commande.GetByCuisinier(db, _utilisateurConnecte.UserId)
            );
            OnPropertyChanged(nameof(Commandes));
        }
    }
}
