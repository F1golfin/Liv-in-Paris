using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris;

public class RegisterViewModel : ViewModelBase
{
    private readonly AppViewModel _appViewModel;

    public string NewNom { get; set; }
    public string NewPrenom { get; set; }
    public string NewEmail { get; set; }
    private string _newAdresse;
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
    public string NewTelephone { get; set; }
    public string NewEntreprise { get; set; }
    public string SelectedType { get; set; }
    public string SelectedRole { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public string MessageErreur { get; set; }


    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }
    
    public ObservableCollection<string> Suggestions { get; set;} = new();
    private readonly AdresseService _adresseService = new();
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

    public RegisterViewModel(AppViewModel appViewModel)
    {
        SelectedType = "Particulier";
        _appViewModel = appViewModel;
        RegisterCommand = new AsyncRelayCommand(Register);
        GoToLoginCommand = new RelayCommand(() => _appViewModel.NavigateToLogin());
    }

    public async Task Register()
    {
        if (string.IsNullOrWhiteSpace(NewNom) || string.IsNullOrWhiteSpace(NewPrenom) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword)
            || string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(NewAdresse) || string.IsNullOrWhiteSpace(NewTelephone) || string.IsNullOrWhiteSpace(SelectedType) || string.IsNullOrWhiteSpace(SelectedRole))
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
        
        Console.WriteLine($"Compte créé pour {NewNom}");
        Console.WriteLine($"Role = '{SelectedRole}', Type = '{SelectedType}'");

        // Création de l'utilisateur
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
        
            Console.WriteLine($"Compte créé pour {NewNom}");
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
    
    public async Task ChargerSuggestionsAsync(string saisie)
    {
        var service = new AdresseService();
        var resultats = await service.ObtenirSuggestionsAsync(saisie);
    
        Suggestions.Clear();
        foreach (var suggestion in resultats)
            Suggestions.Add(suggestion);
    }
}