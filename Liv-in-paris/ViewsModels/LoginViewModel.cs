using System.Windows.Input;
using Liv_in_paris;
using Liv_in_paris.Core.Models;

public class LoginViewModel : ViewModelBase
{
    private readonly AppViewModel _parent;

    public string UserPrenom { get; set; }
    public string Password { get; set; }
    public string MessageErreur { get; set; }

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
            var db = new DatabaseManager("localhost", "livin_paris", "root", "root"); // adapte selon ton user

            string query = $"SELECT * FROM users WHERE prenom = '{UserPrenom}' AND password = '{Password}'";
            var table = db.ExecuteQuery(query);

            if (table.Rows.Count == 1)
            {
                // Authentification réussie
                var row = table.Rows[0];
                string nom = row["nom"].ToString();
                Console.WriteLine($"✅ Bienvenue, {nom} !");
            
                // Redirection ou changement de vue
                _parent.NaviguerVersAccueil("Client"); // ou ce que tu veux
            }
            else
            {
                MessageErreur = "Email ou mot de passe incorrect.";
                OnPropertyChanged(nameof(MessageErreur));
            }
        }
        catch (Exception ex)
        {
            MessageErreur = "Erreur lors de la connexion : " + ex.Message;
            OnPropertyChanged(nameof(MessageErreur));
        }
        /*if (UserPrenom == "chef" && Password == "1234")
            _parent.NaviguerVersAccueil("Cuisinier");
        else if (UserPrenom == "client" && Password == "abcd")
            _parent.NaviguerVersAccueil("Client");
        else
        {
            MessageErreur = "Nom d'utilisateur ou mot de passe incorrect ";
            OnPropertyChanged(nameof(MessageErreur));
        }*/
    }
}

