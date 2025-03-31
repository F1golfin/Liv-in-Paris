using System.Windows.Input;

namespace Liv_in_paris;

public class CuisinierViewModel
{
    private readonly AppViewModel _app;

    public ICommand DeconnexionCommand { get; }

    public CuisinierViewModel(AppViewModel app)
    {
        _app = app;
        DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());
    }
}