using System.Windows.Input;

namespace Liv_in_paris;

public class RegisterViewModel : ViewModelBase
{
    private readonly AppViewModel _appViewModel;

    public string NewUsername { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public string MessageErreur { get; set; }


    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    public RegisterViewModel(AppViewModel appViewModel)
    {
        _appViewModel = appViewModel;
        RegisterCommand = new RelayCommand(Register);
        GoToLoginCommand = new RelayCommand(() => _appViewModel.NavigateToLogin());
    }

    private void Register()
    {
        // TODO: Ajouter utilisateur à la BDD

        if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            MessageErreur = "Veuillez remplir tous les champs.";
            OnPropertyChanged(nameof(MessageErreur));
            return;
        }
        if (NewPassword != ConfirmPassword)
        {
            MessageErreur = "Les mots de passe ne correspondent pas.";
            OnPropertyChanged(nameof(MessageErreur));
            return;
        }
        Console.WriteLine($"Compte créé pour {NewUsername}");
        _appViewModel.NavigateToLogin();
    }
}