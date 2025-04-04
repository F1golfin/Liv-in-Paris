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
            Console.WriteLine("🧪 Je vais me connecter avec le mot de passe : " + "root");
            var db = new DatabaseManager("localhost", "livin_paris", "root", "root"); // adapte selon ton user

            string query = $"SELECT * FROM users WHERE prenom = '{UserPrenom}' AND password = '{Password}'";
            var table = db.ExecuteQuery(query);

            if (table.Rows.Count == 1)
            {
                string rolesFromDb = table.Rows[0]["role"].ToString(); // ex: "Client,Cuisinier"
                var allRoles = rolesFromDb.Split(','); // => ["Client", "Cuisinier"]

                // Authentification réussie
                var row = table.Rows[0];
                var utilisateur = new User
                {
                    UserId = Convert.ToUInt64(row["user_id"]),
                    Password = row["password"].ToString(),
                    Role = row["role"].ToString(),
                    Type = row["type"].ToString(),
                    Email = row["email"].ToString(),
                    Nom = row["nom"].ToString(),
                    Prenom = row["prenom"].ToString(),
                    Adresse = row["adresse"].ToString(),
                    Telephone = row["telephone"].ToString(),
                    Entreprise = row["entreprise"]?.ToString()
                };
                string role = row["role"].ToString();
                
                Console.WriteLine($"✅ Connexion réussie en tant que {role}");
                if (allRoles.Length > 1)
                {
                    // Fenêtre ou menu de sélection
                    var selectedRole = ShowRoleSelectionPopup(allRoles); 
                    RedirectUser(utilisateur,selectedRole);
                }
                else
                {
                    RedirectUser(utilisateur,allRoles[0]);
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

