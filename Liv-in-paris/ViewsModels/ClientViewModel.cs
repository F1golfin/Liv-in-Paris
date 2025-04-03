using System.Windows.Input;

namespace Liv_in_paris;

public class ClientViewModel
{
    private readonly AppViewModel _app;

    public ICommand DeconnexionCommand { get; }

    public ClientViewModel(AppViewModel app)
    {
        _app = app;
        DeconnexionCommand = new RelayCommand(() => _app.Deconnexion());
    }
    
    
    
}