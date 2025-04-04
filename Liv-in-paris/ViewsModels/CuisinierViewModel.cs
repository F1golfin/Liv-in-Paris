using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Views;

namespace Liv_in_paris
{
    public class CuisinierViewModel : ViewModelBase
    {
        private readonly AppViewModel _app;
        private readonly DatabaseManager _db;
        private readonly User _utilisateurConnecte;

        public ObservableCollection<Plat> Plats { get; set; }
        public ObservableCollection<Recette> RecettesExistantes { get; set; }

        public string NewNomPlat { get; set; }
        public string NewPrixPlat { get; set; }
        public string NewTypePlat { get; set; }
        public Recette RecetteSelectionnee { get; set; }

        public ICommand AjouterPlatCommand { get; }
        public ICommand AjouterNouvelleRecetteCommand { get; }
        public ICommand DeconnexionCommand { get; }

        public CuisinierViewModel(AppViewModel parent, User utilisateur)
        {
            _app = parent;
            _utilisateurConnecte = utilisateur;
            _db = new DatabaseManager("localhost", "livin_paris", "root", "root");

            if (!_utilisateurConnecte.Role.ToLower().Contains("cuisinier"))
            {
                MessageBox.Show("Accès réservé aux cuisiniers.");
                return;
            }

            AjouterPlatCommand = new RelayCommand(AjouterPlat);
            AjouterNouvelleRecetteCommand = new RelayCommand(AjouterNouvelleRecette);
            DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());

            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            Plats = new ObservableCollection<Plat>(Plat.GetAllByCuisinier(_db, _utilisateurConnecte.UserId));
            RecettesExistantes = new ObservableCollection<Recette>(Recette.GetAll(_db));
            OnPropertyChanged(nameof(Plats));
            OnPropertyChanged(nameof(RecettesExistantes));
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

            nouveauPlat.AjouterPlat(_db);
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
