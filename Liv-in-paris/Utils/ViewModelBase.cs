using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Liv_in_paris
{
    /// <summary>
    /// Classe de base pour tous les ViewModels de l'application Liv'in Paris.
    /// Fournit l'implémentation de <see cref="INotifyPropertyChanged"/> pour la gestion des notifications de changement de propriété.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Événement déclenché lorsqu'une propriété du ViewModel change.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifie l'interface utilisateur qu'une propriété a changé.
        /// </summary>
        /// <param name="nom">Nom de la propriété (rempli automatiquement grâce à <see cref="CallerMemberNameAttribute"/>).</param>
        protected void OnPropertyChanged([CallerMemberName] string nom = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));
        }
    }
}