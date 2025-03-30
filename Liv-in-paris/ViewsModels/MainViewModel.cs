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

    private readonly AccueilViewModel _accueilVM = new();
    private readonly MetroGraphViewModel _metroVM = new();

    public MainViewModel()
    {
        ShowAccueilCommand = new RelayCommand(() => CurrentView = _accueilVM);
        ShowMetroCommand = new RelayCommand(() => CurrentView = _metroVM);

        // Vue affichée par défaut
        CurrentView = _accueilVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
