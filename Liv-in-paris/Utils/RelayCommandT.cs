using System.Windows.Input;

namespace Liv_in_paris 
{
    /// <summary>
    /// Représente une commande générique implémentant <see cref="ICommand"/>, acceptant un paramètre typé <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type du paramètre attendu par la commande.</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Action à exécuter lorsque la commande est invoquée.</param>
        /// <param name="canExecute">Fonction permettant de déterminer si la commande peut être exécutée.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="execute"/> est null.</exception>
        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Détermine si la commande peut être exécutée avec le paramètre donné.
        /// </summary>
        /// <param name="parameter">Paramètre passé à la commande.</param>
        /// <returns><c>true</c> si la commande peut être exécutée, sinon <c>false</c>.</returns>
        public bool CanExecute(object? parameter)
            => parameter is T t && (_canExecute?.Invoke(t) ?? true);

        /// <summary>
        /// Exécute l'action associée à la commande avec le paramètre donné.
        /// </summary>
        /// <param name="parameter">Paramètre passé à la commande.</param>
        public void Execute(object? parameter)
        {
            if (parameter is T t)
                _execute(t);
        }

        /// <summary>
        /// Événement déclenché pour signaler que l'état de la commande a changé.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value!;
            remove => CommandManager.RequerySuggested -= value!;
        }
    }
}
