using System.Windows.Input;
using Liv_in_paris;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Views;

public class LoginViewModel : ViewModelBase
{
    private readonly AppViewModel _parent;

    public string UserPrenom { get; set; }
    public string Password { get; set; }
    public string MessageErreur { get; set; }
    public string SelectedUserType { get; set; } = "Particulier";

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
        if (string.IsNullOrWhiteSpace(UserPrenom) || string.IsNullOrWhiteSpace(Password))
        {
            MessageErreur = "Veuillez remplir tous les champs.";
            OnPropertyChanged(nameof(MessageErreur));
            return;
        }

        try
        {
            User? utilisateur = null;
            var db = Database.Instance;

            if (SelectedUserType == "Particulier")
            {
                utilisateur = User.AuthenticateParEmail(db, UserPrenom, Password);
            }
            else if (SelectedUserType == "Entreprise")
            {
                utilisateur = User.AuthenticateParEntreprise(db, UserPrenom, Password);
            }

            if (utilisateur != null)
            {
                string[] allRoles = utilisateur.Role.Split(',');

                if (allRoles.Length > 1)
                {
                    string selectedRole = ShowRoleSelectionPopup(allRoles);
                    RedirectUser(utilisateur, selectedRole);
                }
                else
                {
                    RedirectUser(utilisateur, allRoles[0]);
                }
            }
            else
            {
                MessageErreur = "Identifiants incorrects.";
                OnPropertyChanged(nameof(MessageErreur));
            }
        }
        catch (Exception ex)
        {
            MessageErreur = "Erreur lors de la connexion : " + ex.Message;
            OnPropertyChanged(nameof(MessageErreur));
        }
    }

    
    private string ShowRoleSelectionPopup(string[] allRoles)
    {
        var popup = new RoleSelectionPopup(allRoles);
        bool? result = popup.ShowDialog();

        if (result == true)
            return popup.SelectedRole;
        else
            return null;
    }

    void RedirectUser(User utilisateur, string role)
    {
        _parent.NaviguerVersAccueil(utilisateur, role);
    }
}

