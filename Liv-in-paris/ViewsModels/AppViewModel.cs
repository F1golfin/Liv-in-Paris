using System.ComponentModel;
using System.Runtime.CompilerServices;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris
{
    /// <summary>
    /// ViewModel principal de l'application. Gère la navigation entre login, register, client et cuisinier.
    /// </summary>
    public class AppViewModel : INotifyPropertyChanged
    {
        private object _currentSubView;

        /// <summary>
        /// Vue actuelle affichée à l'écran (login, register, client, etc.)
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

        public AppViewModel()
        {
            NavigateToLogin();
        }

        public void NavigateToLogin()
        {
            var vue = new LoginView();
            vue.DataContext = new LoginViewModel(this);
            CurrentSubView = vue;
        }

        public void NavigateToRegister()
        {
            var vue = new RegisterView();
            vue.DataContext = new RegisterViewModel(this);
            CurrentSubView = vue;
        }

        public void NaviguerVersAccueil(User user, string role)
        {
            if (role == "Client")
            {
                var vue = new ClientView();
                vue.DataContext = new ClientViewModel(this,user);
                CurrentSubView = vue;
            }
            else if (role == "Cuisinier")
            {
                var vue = new CuisinierView(user, this);
                CurrentSubView = vue;
            }
        }


        public void Deconnexion()
        {
            NavigateToLogin();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
