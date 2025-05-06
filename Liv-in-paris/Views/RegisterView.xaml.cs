using System.Windows;
using System.Windows.Controls;

namespace Liv_in_paris;

/// <summary>
/// Vue permettant l'inscription d'un nouvel utilisateur dans l'application Liv'in Paris.
/// Gère les rôles, types d'utilisateurs, suggestions d'adresse et validation des champs.
/// </summary>
public partial class RegisterView : UserControl
{
    /// <summary>
    /// Initialise les composants de la vue et sélectionne par défaut le premier type d'utilisateur.
    /// </summary>
    public RegisterView()
    {
        InitializeComponent();
        UserType.SelectedIndex = 0;
    }

    /// <summary>
    /// Gère le clic sur le bouton d'enregistrement.
    /// Récupère les mots de passe, les rôles sélectionnés et déclenche la logique de création via le ViewModel.
    /// </summary>
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
        {
            vm.NewPassword = PasswordBox.Password;
            vm.ConfirmPassword = ConfirmBox.Password;
            var roles = RoleListBox.SelectedItems
                .Cast<ListBoxItem>()
                .Select(item => item.Content.ToString());
            vm.SelectedRole = string.Join(",", roles);
            vm.Register();
        }
    }

    /// <summary>
    /// Gère le changement de type d'utilisateur (Particulier, Entreprise, etc.).
    /// Met à jour dynamiquement l'affichage des champs spécifiques à l'entreprise et ajuste les rôles disponibles.
    /// </summary>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (UserType.SelectedItem is ComboBoxItem selectedItem)
        {
            string value = selectedItem.Content.ToString();

            // Affiche ou masque les champs entreprise
            TxtEntreprise.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;
            NomEntreprise.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;
            CooReferent.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;
            InfoEntreprise.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;

            // Forcer rôle Client uniquement si Entreprise
            if (DataContext is RegisterViewModel vm)
            {
                if (value == "Entreprise")
                {
                    vm.SelectedRole = "Client";
                    RoleListBox.SelectedItems.Clear();
                    foreach (ListBoxItem item in RoleListBox.Items)
                    {
                        if ((string)item.Content == "Client")
                        {
                            item.IsSelected = true;
                            break;
                        }
                    }
                    RoleListBox.IsEnabled = false;
                }
                else
                {
                    vm.SelectedRole = null;
                    RoleListBox.IsEnabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Déclenche la récupération de suggestions d'adresse à mesure que l'utilisateur saisit son adresse.
    /// </summary>
    private async void IDAdresse_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is RegisterViewModel vm)
        {
            string saisie = IDAdresse.Text;
            await vm.ChargerSuggestionsAsync(saisie);
        }
    }

    /// <summary>
    /// Gère la sélection d'une adresse dans la liste des suggestions.
    /// Met à jour le champ d'adresse du ViewModel et vide les suggestions.
    /// </summary>
    private void SuggestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is string adresse)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.NewAdresse = adresse;
                vm.Suggestions.Clear();
            }
        }
    }
}
