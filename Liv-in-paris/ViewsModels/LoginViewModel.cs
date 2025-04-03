using System.Windows.Input;
using Liv_in_paris;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Views;

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
                string rolesFromDb = table.Rows[0]["role"].ToString(); // ex: "Client,Cuisinier"
                var allRoles = rolesFromDb.Split(','); // => ["Client", "Cuisinier"]

                // Authentification réussie
                var row = table.Rows[0];
                string role = row["role"].ToString();
                Console.WriteLine($"✅ Connexion réussie en tant que {role}");
                if (allRoles.Length > 1)
                {
                    // Fenêtre ou menu de sélection
                    var selectedRole = ShowRoleSelectionPopup(allRoles); 
                    RedirectUser(selectedRole);
                }
                else
                {
                    RedirectUser(allRoles[0]);
                }
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
    private string ShowRoleSelectionPopup(string[] allRoles)
    {
        var popup = new RoleSelectionPopup(allRoles);
        bool? result = popup.ShowDialog();

        if (result == true)
            return popup.SelectedRole;
        else
            return null;
    }

    void RedirectUser(string role)
    {
        if (role == "Client")
            _parent.CurrentSubView = new ClientView();
        else if (role == "Cuisinier")
            _parent.CurrentSubView = new CuisinierView();
    }

}

