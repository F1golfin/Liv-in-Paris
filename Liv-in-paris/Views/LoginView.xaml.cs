using System.Windows;
using System.Windows.Controls;

namespace Liv_in_paris;

/// <summary>
/// Vue utilisateur pour la connexion à l'application Liv'in Paris.
/// Gère le formulaire d'identifiants, mot de passe, type d'utilisateur et les interactions de base.
/// </summary>
public partial class LoginView : UserControl
{
    /// <summary>
    /// Initialise les composants de la vue et sélectionne par défaut le premier type d'utilisateur.
    /// </summary>
    public LoginView()
    {
        InitializeComponent();
        UserType.SelectedIndex = 0;
    }

    /// <summary>
    /// Gère le clic sur le bouton de connexion.
    /// Transfère le mot de passe saisi au <see cref="LoginViewModel"/> et exécute la commande de connexion.
    /// </summary>
    private void SeConnecter_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = MdPInput.Password;
            vm.LoginCommand.Execute(null);
        }
    }

    /// <summary>
    /// Gère le changement de sélection dans la liste des types d'utilisateurs.
    /// Met à jour l'affichage du champ d'identifiant en fonction du type choisi.
    /// </summary>
    private void UserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (UserType.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedContent = selectedItem.Content.ToString();
            
            if (DataContext is LoginViewModel vm)
            {
                vm.SelectedUserType = selectedContent;
            }

            switch (selectedContent)
            {
                case "Particulier":
                    IDInfo.Text = "Adresse mail";
                    break;

                case "Entreprise":
                    IDInfo.Text = "Nom de l'entreprise";
                    break;

                case "Admin":
                    IDInfo.Text = "Adresse mail";
                    break;

                default:
                    IDInfo.Text = "Choisissez une option...";
                    break;
            }
        }
    }

    /// <summary>
    /// Affiche ou masque le label indicatif du champ d'identifiant selon s'il est vide ou non.
    /// </summary>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(IDInput.Text))
        {
            IDInfo.Visibility = Visibility.Visible;
        }
        else
        {
            IDInfo.Visibility = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Affiche ou masque le label indicatif du mot de passe selon s'il est vide ou non.
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(MdPInput.Password))
        {
            MdPInfo.Visibility = Visibility.Visible;
        }
        else
        {
            MdPInfo.Visibility = Visibility.Hidden;
        }
    }
}
