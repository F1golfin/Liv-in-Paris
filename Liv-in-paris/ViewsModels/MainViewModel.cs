using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Liv_in_paris;

/// <summary>
/// ViewModel principal de l'application, utilisé dans <c>MainWindow</c>.
/// Gère la navigation entre les vues principales : Accueil, Métro, Application, Login, et Admin.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Commande pour afficher la vue d'accueil.
    /// </summary>
    public ICommand ShowAccueilCommand { get; }

    /// <summary>
    /// Commande pour afficher la vue du graphe du métro.
    /// </summary>
    public ICommand ShowMetroCommand { get; }

    /// <summary>
    /// Commande pour afficher la vue principale de l'application (AppView).
    /// </summary>
    public ICommand ShowAppCommand { get; }

    /// <summary>
    /// Commande pour afficher la vue de connexion.
    /// </summary>
    public ICommand ShowLoginCommand { get; }

    /// <summary>
    /// Commande pour afficher la vue d'administration (gestion des utilisateurs).
    /// </summary>
    public ICommand ShowAdminCommand =>
        new RelayCommand(() =>
        {
            var vue = new UtilisateursView();
            vue.DataContext = new UtilisateursViewModel();
            CurrentView = vue;
        });

    private object _currentView;

    /// <summary>
    /// Vue actuellement affichée dans l'interface utilisateur.
    /// </summary>
    public object CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    // Instances préchargées des vues
    private readonly AccueilView _accueilView = new();
    private readonly LoginView _loginView = new();
    private readonly MetroGraphView _metroView = new();
    private readonly AppView _appView = new();

    /// <summary>
    /// Initialise le <see cref="MainViewModel"/> et configure les commandes de navigation.
    /// </summary>
    public MainViewModel()
    {
        ShowLoginCommand = new RelayCommand(() => CurrentView = _loginView);
        ShowAccueilCommand = new RelayCommand(() => CurrentView = _accueilView);
        ShowMetroCommand = new RelayCommand(() => CurrentView = _metroView);
        ShowAppCommand = new RelayCommand(() => CurrentView = _appView);

        // Vue affichée par défaut
        CurrentView = _appView;
    }

    /// <summary>
    /// Événement déclenché lorsqu'une propriété change.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifie l'interface qu'une propriété a changé de valeur.
    /// </summary>
    /// <param name="name">Nom de la propriété (déduit automatiquement si null).</param>
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
