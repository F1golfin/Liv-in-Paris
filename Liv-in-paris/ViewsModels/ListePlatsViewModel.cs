using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Liv_in_paris
{
    /// <summary>
    /// ViewModel de la vue ListePlatsView.
    /// Permet au cuisinier connecté de visualiser, modifier et supprimer ses plats, et de gérer les commandes.
    /// </summary>
    public class ListePlatsViewModel : ViewModelBase
    {
        private readonly AppViewModel _app;
        private readonly User _utilisateurConnecte;

        public ObservableCollection<Plat> Plats { get; set; }
        public ObservableCollection<Recette> RecettesExistantes { get; set; }

        public string NewNomPlat { get; set; }
        public string NewPrixPlat { get; set; }
        public string NewTypePlat { get; set; }
        public ICommand MettreAJourStatutCommand { get; }
        public ICommand SupprimerPlatCommand { get; }
        public Recette RecetteSelectionnee { get; set; }
        private string _newNbParts;
        public string NewNbParts
        {
            get => _newNbParts;
            set
            {
                _newNbParts = value;
                OnPropertyChanged(nameof(NewNbParts));
            }
            
        }
        
        public ObservableCollection<Evaluation> EvaluationsRecues { get; set; }
        /// <summary>
        /// Constructeur du ViewModel.
        /// Initialise les commandes et charge les données nécessaires à l'affichage.
        /// </summary>
        /// <param name="parent">ViewModel principal</param>
        /// <param name="utilisateur">Utilisateur connecté (cuisinier)</param>
        public ListePlatsViewModel(AppViewModel parent, User utilisateur)
        {
            _app = parent;
            _utilisateurConnecte = utilisateur;
            ChargerDonnees();
            MettreAJourStatutCommand = new RelayCommand<LigneCommande>(ligne =>
            {
                var db = Database.Instance;
                ligne.MettreAJourStatut(db, ligne.Statut); // statut déjà sélectionné dans ComboBox
                ChargerCommandes(); // refresh
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
        /// Liste des commandes associées au cuisinier.
        /// </summary>
        public ObservableCollection<Commande> Commandes { get; set; }
        
        /// <summary>
        /// Charge les plats du cuisinier depuis la base de données.
        /// </summary>
        private void ChargerCommandes()
        {
            var db = Database.Instance;
            Commandes = new ObservableCollection<Commande>(
                Commande.GetByCuisinier(db, _utilisateurConnecte.UserId)
            );
            OnPropertyChanged(nameof(Commandes));
        }

        /// <summary>
        /// Charge les plats du cuisinier depuis la base de données.
        /// </summary>
        private void ChargerDonnees()
        {
            var db = Database.Instance;

            Plats = new ObservableCollection<Plat>(Plat.GetAllByCuisinier(db, _utilisateurConnecte.UserId));
           
            OnPropertyChanged(nameof(Plats));



        }
    }
}
