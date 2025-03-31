using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Liv_in_paris;

public class MainViewModel : INotifyPropertyChanged
{
    public ICommand ShowAccueilCommand { get; }
    public ICommand ShowMetroCommand { get; }
    public ICommand ShowAppCommand { get; }

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
    private readonly MetroGraphView _metroView = new();
    private readonly AppView _appView = new();
    

    public MainViewModel()
    {
        ShowAccueilCommand = new RelayCommand(() => CurrentView = _accueilView);
        ShowMetroCommand = new RelayCommand(() => CurrentView = _metroView);
        ShowAppCommand = new RelayCommand(() => CurrentView = _appView);

        // Vue affichée par défaut
        CurrentView = _accueilView;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
