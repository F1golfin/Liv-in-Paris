using System.ComponentModel;
using System.Runtime.CompilerServices;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Views;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris
{
    /// <summary>
    /// ViewModel principal de l'application Liv'in Paris.
    /// Gère la navigation entre les différentes vues (Login, Register, Client, Cuisinier, Admin),
    /// et initialise les données nécessaires au démarrage.
    /// </summary>
    public class AppViewModel : ViewModelBase
    {
        private object _currentSubView;

        /// <summary>
        /// Vue actuellement affichée à l'écran (ex. LoginView, RegisterView, NClientView, etc.).
        /// </summary>
        public object CurrentSubView
        {
            get => _currentSubView;
            set
            {
                _currentSubView = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Constructeur principal du ViewModel de l'application.
        /// Supprime les lignes de panier orphelines et affiche la vue de connexion.
        /// </summary>
        public AppViewModel()
        {
            NettoyerLignesPanierOrphelines();
            NavigateToLogin();
        }

        /// <summary>
        /// Affiche la vue de connexion et initialise son ViewModel.
        /// </summary>
        public void NavigateToLogin()
        {
            var vue = new LoginView();
            vue.DataContext = new LoginViewModel(this);
            CurrentSubView = vue;
        }

        /// <summary>
        /// Affiche la vue d'enregistrement et initialise son ViewModel.
        /// </summary>
        public void NavigateToRegister()
        {
            var vue = new RegisterView();
            vue.DataContext = new RegisterViewModel(this);
            CurrentSubView = vue;
        }

        /// <summary>
        /// Navigue vers la vue d'accueil correspondant au rôle de l'utilisateur connecté.
        /// </summary>
        /// <param name="user">Utilisateur connecté.</param>
        /// <param name="role">Rôle de l'utilisateur : "Client", "Cuisinier" ou "Admin".</param>
        public void NaviguerVersAccueil(User user, string role)
        {
            if (role == "Client")
            {
                var vue = new NClientView();
                vue.DataContext = new NClientViewModel(this, user);
                CurrentSubView = vue;
            }
            else if (role == "Cuisinier")
            {
                var vue = new CuisinierView(user, this);
                CurrentSubView = vue;
            }
            else if (role == "Admin")
            {
                var vue = new AdminView(this);
                CurrentSubView = vue;
            }
        }

        /// <summary>
        /// Déconnecte l'utilisateur et revient à la vue de connexion.
        /// </summary>
        public void Deconnexion()
        {
            NettoyerLignesPanierOrphelines();
            NavigateToLogin();
        }

        /// <summary>
        /// Supprime de la base les lignes de commande en statut "Panier" non rattachées à une commande.
        /// </summary>
        public void NettoyerLignesPanierOrphelines()
        {
            var db = Database.Instance;
            string query = "DELETE FROM lignes_commandes WHERE statut = 'Panier' AND commande_id IS NULL;";
            db.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Événement déclenché lorsqu'une propriété change.
        /// Utilisé par le binding WPF.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
