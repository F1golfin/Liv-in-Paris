using Liv_in_paris.Core.Models;
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
    public class NClientViewModel : ViewModelBase
    {
        private object _platsVue;
        public object PlatsVue
        {
            get => _platsVue;
            set { _platsVue = value; OnPropertyChanged(); }
        }
        private object _panierVue;
        public object PanierVue
        {
            get => _panierVue;
            set { _panierVue = value; OnPropertyChanged(); }
        }
        private object _commandesVue;
        public object CommandesVue
        {
            get => _commandesVue;
            set { _commandesVue = value; OnPropertyChanged(); }
        }

        private string _texteBoutonCompte = "Gérer votre compte";
        public string TexteBoutonCompte
        {
            get => _texteBoutonCompte;
            set { _texteBoutonCompte = value; OnPropertyChanged(); }
        }
        private bool _estDansCompte = false;

        private readonly AppViewModel _app;
        public ICommand DeconnexionCommand { get; }
        public ICommand ActualiserCommand { get; }
        public ICommand GererCompteCommand { get; }

        public readonly User _utilisateur;

        public ObservableCollection<Plat> Panier { get; } = new();

        public string UtilisateurLabel => $"Bonjour {_utilisateur.Prenom}";
        public NClientViewModel(AppViewModel app, User utilisateur)
        {
            _app = app;
            _utilisateur = utilisateur;
            ActualiserCommand = new RelayCommand(ChargerDonnees);
            DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());
            GererCompteCommand = new RelayCommand(ToggleCompteOuPlats);

            ChargerDonnees();

        }
        
        public void ChargerDonnees()
        {

            var platsView = new PlatsView();
            platsView.DataContext = new PlatsViewModel(this);
            PlatsVue=platsView;

            var panierView = new PanierView();
            panierView.DataContext = new PanierViewModel(Panier, _utilisateur, this);
            PanierVue=panierView;

            var commandesView = new CommandesView();
            commandesView.DataContext = new CommandesViewModel(_app, _utilisateur);
            CommandesVue=commandesView;
        }
        

        public void AjouterAuPanier(Plat plat)
        {
            if (Panier.Count > 0 && plat.CuisinierId != Panier[0].CuisinierId)
            {
                MessageBox.Show("❌ Vous ne pouvez commander que des plats du même cuisinier. Veuillez valider ou vider votre panier.");
                return;
            }

            if (!Panier.Any(p => p.PlatId == plat.PlatId))
            {
                Panier.Add(plat);
            }

            if (PlatsVue is PlatsView vue && vue.DataContext is PlatsViewModel platsVM)
            {
                platsVM.RetirerPlatDisponible(plat);
            }

            OnPropertyChanged(nameof(Panier));
        }
        
        private void AfficherCompte()
        {
            var compteView = new CompteView();
            compteView.DataContext = new CompteViewModel(_utilisateur,this);
            PlatsVue = compteView;
        }
        
        public void AfficherPlats()
        {
            var platsView = new PlatsView();
            platsView.DataContext = new PlatsViewModel(this);
            PlatsVue = platsView;
        }
        
        private void ToggleCompteOuPlats()
        {
            if (_estDansCompte)
            {
                AfficherPlats();
                TexteBoutonCompte = "Gérer votre compte";
            }
            else
            {
                var compteView = new CompteView();
                compteView.DataContext = new CompteViewModel(_utilisateur, this);
                PlatsVue = compteView;
                TexteBoutonCompte = "← Revenir aux plats";
            }

            _estDansCompte = !_estDansCompte;
        }
    }
}
