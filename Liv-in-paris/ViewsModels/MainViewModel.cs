using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZstdSharp.Unsafe;

namespace Liv_in_paris;

public class MainViewModel : INotifyPropertyChanged
{
    public ICommand ShowAccueilCommand { get; }
    public ICommand ShowMetroCommand { get; }
    public ICommand ShowAppCommand { get; }
    public ICommand ShowLoginCommand { get; }
    public ICommand ShowAdminCommand =>
        new RelayCommand(() =>
        {
            var vue = new UtilisateursView();
            vue.DataContext = new UtilisateursViewModel();
            CurrentView = vue;
        });

    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    private readonly AccueilView _accueilView = new();
    private readonly LoginView _loginView = new();
    private readonly MetroGraphView _metroView = new();
    private readonly AppView _appView = new();
    

    public MainViewModel()
    {
        ShowLoginCommand = new RelayCommand(() => CurrentView = _loginView);
        ShowAccueilCommand = new RelayCommand(() => CurrentView = _accueilView);
        ShowMetroCommand = new RelayCommand(() => CurrentView = _metroView);
        ShowAppCommand = new RelayCommand(() => CurrentView = _appView);

        // Vue affichée par défaut
        CurrentView = _appView;
    }

    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
