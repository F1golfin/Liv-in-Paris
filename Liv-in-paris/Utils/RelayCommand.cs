namespace Liv_in_paris;

using System;
using System.Windows.Input;


/// <summary>
/// Représente une commande réutilisable pour relier des actions dans un ViewModel à l'interface utilisateur.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Initialise une nouvelle instance de la commande.
    /// </summary>
    /// <param name="execute">Action à exécuter lors de l'exécution de la commande.</param>
    /// <param name="canExecute">Fonction facultative qui détermine si la commande peut s'exécuter.</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Détermine si la commande peut être exécutée dans son état actuel.
    /// </summary>
    /// <param name="parameter">Paramètre facultatif (non utilisé ici).</param>
    /// <returns>True si la commande peut s'exécuter, sinon False.</returns>
    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    /// <summary>
    /// Exécute la commande.
    /// </summary>
    /// <param name="parameter">Paramètre facultatif (non utilisé ici).</param>
    public void Execute(object? parameter) => _execute();

    /// <summary>
    /// Événement déclenché lorsqu’un changement d’état d’exécutabilité est détecté.
    /// </summary>
    public event EventHandler? CanExecuteChanged;
}