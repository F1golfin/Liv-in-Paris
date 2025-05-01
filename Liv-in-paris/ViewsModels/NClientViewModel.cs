using Liv_in_paris.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        private readonly AppViewModel _app;
        public ICommand DeconnexionCommand { get; }
        public ICommand ActualiserCommand { get; }

        private User _utilisateur;

        public ObservableCollection<Plat> Panier { get; set; } = new();

        public string UtilisateurLabel => $"Bonjour {_utilisateur.Prenom}";
        public NClientViewModel(AppViewModel app, User utilisateur)
        {
            _app = app;
            _utilisateur = utilisateur;
            ActualiserCommand = new RelayCommand(ChargerDonnees);
            DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());

            ChargerDonnees();

        }



        public void ChargerDonnees()
        {

            var platsView = new PlatsView();
            platsView.DataContext = new PlatsViewModel(this);
            PlatsVue=platsView;

            var panierView = new PanierView();
            panierView.DataContext = new PanierViewModel(Panier, _utilisateur, _app);
            PanierVue=panierView;

            var commandesView = new CommandesView();
            commandesView.DataContext = new CommandesViewModel(_app, _utilisateur);
            CommandesVue=commandesView;
        }


        public void AjouterAuPanier(Plat plat)
        {
            if (Panier.Count > 0)
            {
                var premierCuisinierId = Panier[0].CuisinierId;

                if (plat.CuisinierId != premierCuisinierId)
                {
                    //MessageBox.Show("❌ Vous ne pouvez commander que des plats du même cuisinier. Veuillez valider ou vider votre panier.");
                    return;
                }
            }

            if (!Panier.Contains(plat))
                Panier.Add(plat);

            // Retirer le plat de la liste visible
            if (PlatsVue is PlatsView vue && vue.DataContext is PlatsViewModel platsVM)
            {
                platsVM.Plats.Remove(plat);
            }
            ChargerDonnees();
            OnPropertyChanged(nameof(Panier));
        }
    }
}
