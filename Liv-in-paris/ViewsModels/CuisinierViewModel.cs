using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Core.Utils;
using Liv_in_paris.Views;

namespace Liv_in_paris
{
    public class CuisinierViewModel : ViewModelBase
    {
        private readonly AppViewModel _app;
        private readonly User _utilisateurConnecte;

        public ObservableCollection<Plat> Plats { get; set; }
        public ObservableCollection<Recette> RecettesExistantes { get; set; }

        public string NewNomPlat { get; set; }
        public string NewPrixPlat { get; set; }
        public string NewTypePlat { get; set; }
        public Recette RecetteSelectionnee { get; set; }
        
        public ObservableCollection<Evaluation> EvaluationsRecues { get; set; }

        public ICommand AjouterPlatCommand { get; }
        public ICommand AjouterNouvelleRecetteCommand { get; }
        public ICommand DeconnexionCommand { get; }
        public ICommand MettreAJourStatutCommand { get; }

        
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

            var db = Database.Instance;
            ChargerDonnees();
            MettreAJourStatutCommand = new RelayCommand<LigneCommande>(ligne =>
            {
                var db = Database.Instance;
                ligne.MettreAJourStatut(db, ligne.Statut); // statut déjà sélectionné dans ComboBox
                ChargerCommandes(); // refresh
            });

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

            ChargerCommandes();

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

            var db = Database.Instance;
            var nouveauPlat = new Plat
            {
                NomPlat = NewNomPlat,
                PrixParPersonne = prix,
                NbParts = 1,
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
        
        public ObservableCollection<Commande> Commandes { get; set; }

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
