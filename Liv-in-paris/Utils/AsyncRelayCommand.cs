using System.Windows.Input;

namespace Liv_in_paris
{
    /// <summary>
    /// Commande asynchrone permettant de lier une tâche à une commande WPF tout en gérant l'état d'exécution.
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Se produit lorsque l'état de la commande change (par exemple, pour indiquer si elle peut s'exécuter ou non).
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AsyncRelayCommand"/>.
        /// </summary>
        /// <param name="execute">La fonction asynchrone à exécuter lorsque la commande est appelée.</param>
        /// <param name="canExecute">Fonction déterminant si la commande peut être exécutée.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="execute"/> est null.</exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Détermine si la commande peut s'exécuter.
        /// </summary>
        /// <param name="parameter">Paramètre de la commande (non utilisé).</param>
        /// <returns><c>true</c> si la commande peut être exécutée, sinon <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        /// <summary>
        /// Exécute la commande de manière asynchrone.
        /// </summary>
        /// <param name="parameter">Paramètre de la commande (non utilisé).</param>
        public async void Execute(object parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            try
            {
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Déclenche l'événement <see cref="CanExecuteChanged"/> pour notifier l'interface que l'état de la commande a changé.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
