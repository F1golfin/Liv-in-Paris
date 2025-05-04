using System.Collections.ObjectModel;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

/// <summary>
/// ViewModel pour la vue d'enregistrement (RegisterView).
/// Gère la création d'un nouvel utilisateur et les suggestions d'adresse.
/// </summary>
public class RegisterViewModel : ViewModelBase
{
    private readonly AppViewModel _appViewModel;

    /// <summary>
    /// Nom de l'utilisateur.
    /// </summary>
    public string NewNom { get; set; }

    /// <summary>
    /// Prénom de l'utilisateur.
    /// </summary>
    public string NewPrenom { get; set; }

    /// <summary>
    /// Adresse e-mail de l'utilisateur.
    /// </summary>
    public string NewEmail { get; set; }

    private string _newAdresse;

    /// <summary>
    /// Adresse postale de l'utilisateur.
    /// </summary>
    public string NewAdresse
    {
        get => _newAdresse;
        set
        {
            if (_newAdresse != value)
            {
                _newAdresse = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Numéro de téléphone de l'utilisateur.
    /// </summary>
    public string NewTelephone { get; set; }

    /// <summary>
    /// Nom de l'entreprise si le type est "Entreprise".
    /// </summary>
    public string NewEntreprise { get; set; }

    /// <summary>
    /// Type de compte : "Particulier" ou "Entreprise".
    /// </summary>
    public string SelectedType { get; set; }

    /// <summary>
    /// Rôle(s) sélectionné(s) pour l'utilisateur : "Client", "Cuisinier", etc.
    /// </summary>
    public string SelectedRole { get; set; }

    /// <summary>
    /// Mot de passe saisi par l'utilisateur.
    /// </summary>
    public string NewPassword { get; set; }

    /// <summary>
    /// Confirmation du mot de passe.
    /// </summary>
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// Message d'erreur affiché dans l'interface utilisateur.
    /// </summary>
    public string MessageErreur { get; set; }

    /// <summary>
    /// Commande pour valider l'enregistrement.
    /// </summary>
    public ICommand RegisterCommand { get; }

    /// <summary>
    /// Commande pour retourner à la page de connexion.
    /// </summary>
    public ICommand GoToLoginCommand { get; }

    /// <summary>
    /// Liste des suggestions d'adresse proposées dynamiquement.
    /// </summary>
    public ObservableCollection<string> Suggestions { get; set; } = new();

    private readonly AdresseService _adresseService = new();

    /// <summary>
    /// Propriété liée à la TextBox d’adresse pour déclencher les suggestions.
    /// </summary>
    public string AdresseSaisie
    {
        get => NewAdresse;
        set
        {
            NewAdresse = value;
            OnPropertyChanged();
            _ = ChargerSuggestionsAsync(value);
        }
    }

    /// <summary>
    /// Initialise une nouvelle instance du <see cref="RegisterViewModel"/>.
    /// </summary>
    /// <param name="appViewModel">ViewModel principal de l'application.</param>
    public RegisterViewModel(AppViewModel appViewModel)
    {
        SelectedType = "Particulier";
        _appViewModel = appViewModel;
        RegisterCommand = new AsyncRelayCommand(Register);
        GoToLoginCommand = new RelayCommand(() => _appViewModel.NavigateToLogin());
    }

    /// <summary>
    /// Tente d'enregistrer un nouvel utilisateur après validation des champs.
    /// Vérifie les contraintes de mot de passe, d'adresse, et de doublon.
    /// </summary>
    public async Task Register()
    {
        if (string.IsNullOrWhiteSpace(NewNom) || string.IsNullOrWhiteSpace(NewPrenom) ||
            string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword) ||
            string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(NewAdresse) ||
            string.IsNullOrWhiteSpace(NewTelephone) || string.IsNullOrWhiteSpace(SelectedType) ||
            string.IsNullOrWhiteSpace(SelectedRole))
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

        if (SelectedType == "Entreprise" && string.IsNullOrWhiteSpace(NewEntreprise))
        {
            MessageErreur = "Le nom de l'entreprise est requis pour les comptes professionnels.";
            OnPropertyChanged(nameof(MessageErreur));
            return;
        }

        var coords = await _adresseService.ObtenirCoordonneesAsync(NewAdresse);
        if (coords == null)
        {
            MessageErreur = "Veuillez saisir une adresse valide située à Paris.";
            OnPropertyChanged(nameof(MessageErreur));
            return;
        }

        var newUser = new User
        {
            Password = NewPassword,
            Role = SelectedRole,
            Type = SelectedType,
            Email = NewEmail,
            Nom = NewNom,
            Prenom = NewPrenom,
            Adresse = NewAdresse,
            Telephone = NewTelephone,
            Entreprise = (SelectedType == "Entreprise" ? NewEntreprise : null)
        };

        try
        {
            var db = Database.Instance;
            newUser.CreerUser(db);
            _appViewModel.NavigateToLogin();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Duplicate") && ex.Message.Contains("email"))
                MessageErreur = "Cette adresse email est déjà utilisée.";
            else if (ex.Message.Contains("Duplicate") && ex.Message.Contains("entreprise"))
                MessageErreur = "Ce nom d’entreprise est déjà utilisé.";
            else
                MessageErreur = "Erreur lors de l'enregistrement : " + ex.Message;

            OnPropertyChanged(nameof(MessageErreur));
        }
    }

    /// <summary>
    /// Appelle le service d'adresse pour charger des suggestions à partir de la saisie.
    /// </summary>
    /// <param name="saisie">Texte saisi par l'utilisateur.</param>
    public async Task ChargerSuggestionsAsync(string saisie)
    {
        var service = new AdresseService();
        var resultats = await service.ObtenirSuggestionsAsync(saisie);

        Suggestions.Clear();
        foreach (var suggestion in resultats)
            Suggestions.Add(suggestion);
    }
}
