using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Liv_in_paris
{
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
        public ObservableCollection<Commande> Commandes { get; set; }
        private void ChargerCommandes()
        {
            var db = Database.Instance;
            Commandes = new ObservableCollection<Commande>(
                Commande.GetByCuisinier(db, _utilisateurConnecte.UserId)
            );
            OnPropertyChanged(nameof(Commandes));
        }

        private void ChargerDonnees()
        {
            var db = Database.Instance;

            Plats = new ObservableCollection<Plat>(Plat.GetAllByCuisinier(db, _utilisateurConnecte.UserId));
           
            OnPropertyChanged(nameof(Plats));



        }
    }
}
