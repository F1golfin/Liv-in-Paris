using System.Windows.Input;
using Liv_in_paris;

public class LoginViewModel : ViewModelBase
{
    private readonly AppViewModel _parent;

    public string Username { get; set; }
    public string Password { get; set; }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    public LoginViewModel(AppViewModel parent)
    {
        _parent = parent;
        LoginCommand = new RelayCommand(Login);
        GoToRegisterCommand = new RelayCommand(() => _parent.NavigateToRegister());
    }

    private void Login()
    {
        if (Username == "chef" && Password == "1234")
            _parent.NaviguerVersAccueil("Cuisinier");
        else if (Username == "client" && Password == "abcd")
            _parent.NaviguerVersAccueil("Client");
        else
            Console.WriteLine("Login échoué !" + Username + "," + Password);
    }
}

