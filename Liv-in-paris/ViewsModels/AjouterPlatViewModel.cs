using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Core.Utils;
using Liv_in_paris.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Liv_in_paris
{
    class AjouterPlatViewModel : ViewModelBase
    {
        private readonly AppViewModel _app;
        private readonly User _utilisateurConnecte;

        public ObservableCollection<Plat> Plats { get; set; }
        public ObservableCollection<Recette> RecettesExistantes { get; set; }

        public string NewNomPlat { get; set; }
        public string NewPrixPlat { get; set; }
        public string NewTypePlat { get; set; }
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

        public ICommand AjouterPlatCommand { get; }
        public ICommand SupprimerPlatCommand { get; }
        public ICommand AjouterNouvelleRecetteCommand { get; }
        public AjouterPlatViewModel(User utilisateur, AppViewModel parent)
        {
            _app = parent;
            _utilisateurConnecte = utilisateur;
            AjouterPlatCommand = new RelayCommand(AjouterPlat);
            AjouterNouvelleRecetteCommand = new RelayCommand(AjouterNouvelleRecette);
            ChargerDonnees();
        }
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

        }

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
        private void AjouterNouvelleRecette()
        {
            var fenetre = new NouvelleRecetteWindow();
            fenetre.ShowDialog();
            ChargerDonnees();
        }
    }
}
