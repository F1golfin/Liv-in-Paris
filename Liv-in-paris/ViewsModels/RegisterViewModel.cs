using System.Windows.Controls;
using System.Windows.Input;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public class RegisterViewModel : ViewModelBase
{
    private readonly AppViewModel _appViewModel;

    public string NewNom { get; set; }
    public string NewPrenom { get; set; }
    public string NewEmail { get; set; }
    public string NewAdresse { get; set; }
    public string NewTelephone { get; set; }
    public string NewEntreprise { get; set; }
    public string SelectedType { get; set; }
    public string SelectedRole { get; set; }
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

    public void Register()
    {
        // TODO: Ajouter utilisateur à la BDD

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
        Console.WriteLine($"Compte créé pour {NewNom}");
        _appViewModel.NavigateToLogin();
        
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
            DatabaseManager db = new DatabaseManager("localhost", "livin_paris", "root", "root"); // Modifier les paramètres si nécessaire
            newUser.CreerUser(db);
        
            Console.WriteLine($"Compte créé pour {NewNom}");
            _appViewModel.NavigateToLogin();
        }
        catch (Exception ex)
        {
            MessageErreur = "Erreur lors de l'enregistrement : " + ex.Message;
            OnPropertyChanged(nameof(MessageErreur));
        }

    }
}