using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Liv_in_paris;

public class MainViewModel : INotifyPropertyChanged
{
    public ICommand ShowAccueilCommand { get; }
    public ICommand ShowMetroCommand { get; }

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

    public MainViewModel()
    {
        ShowAccueilCommand = new RelayCommand(() => CurrentView = _accueilView);
        ShowMetroCommand = new RelayCommand(() => CurrentView = _metroView);

        // Vue affichée par défaut
        CurrentView = _accueilView;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
