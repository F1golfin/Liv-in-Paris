using System.Windows.Input;
using Liv_in_paris;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Views;

namespace Liv_in_paris;

/// <summary>
/// ViewModel de la vue de connexion dans l'application Liv'in Paris.
/// Gère l'authentification de l'utilisateur selon son type (Particulier ou Entreprise),
/// ainsi que la redirection en fonction de son rôle.
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private readonly AppViewModel _parent;

    /// <summary>
    /// Identifiant saisi par l'utilisateur (e-mail ou nom d'entreprise).
    /// </summary>
    public string UserPrenom { get; set; }

    /// <summary>
    /// Mot de passe saisi.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Message d'erreur affiché en cas d'échec de la connexion.
    /// </summary>
    public string MessageErreur { get; set; }

    /// <summary>
    /// Type d'utilisateur sélectionné ("Particulier", "Entreprise", "Admin").
    /// </summary>
    public string SelectedUserType { get; set; } = "Particulier";

    /// <summary>
    /// Commande de connexion.
    /// </summary>
    public ICommand LoginCommand { get; }

    /// <summary>
    /// Commande pour naviguer vers la page d'enregistrement.
    /// </summary>
    public ICommand GoToRegisterCommand { get; }

    /// <summary>
    /// Initialise une nouvelle instance du <see cref="LoginViewModel"/>.
    /// </summary>
    /// <param name="parent">ViewModel principal de l'application, utilisé pour la navigation.</param>
    public LoginViewModel(AppViewModel parent)
    {
        _parent = parent;
        LoginCommand = new RelayCommand(Login);
        GoToRegisterCommand = new RelayCommand(() => _parent.NavigateToRegister());
    }

    /// <summary>
    /// Tente d'authentifier l'utilisateur selon le type sélectionné.
    /// Gère la redirection en fonction des rôles obtenus.
    /// </summary>
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

    /// <summary>
    /// Affiche une fenêtre popup pour permettre à l'utilisateur de choisir son rôle si plusieurs sont disponibles.
    /// </summary>
    /// <param name="allRoles">Liste des rôles disponibles.</param>
    /// <returns>Rôle sélectionné par l'utilisateur, ou null si annulé.</returns>
    private string ShowRoleSelectionPopup(string[] allRoles)
    {
        var popup = new RoleSelectionPopup(allRoles);
        bool? result = popup.ShowDialog();

        if (result == true)
            return popup.SelectedRole;
        else
            return null;
    }

    /// <summary>
    /// Redirige l'utilisateur vers la vue appropriée selon son rôle.
    /// </summary>
    /// <param name="utilisateur">Utilisateur authentifié.</param>
    /// <param name="role">Rôle sélectionné ou détecté.</param>
    void RedirectUser(User utilisateur, string role)
    {
        _parent.NaviguerVersAccueil(utilisateur, role);
    }
}
