using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Liv_in_paris;

/// <summary>
/// Classe de base pour tous les ViewModels. Implémente INotifyPropertyChanged.
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string nom = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));
    }
}