using System.Windows;

namespace Liv_in_paris.Views;

/// <summary>
/// Fenêtre popup permettant à l'utilisateur de sélectionner un rôle parmi une liste proposée.
/// </summary>
public partial class RoleSelectionPopup : Window
{
    /// <summary>
    /// Obtient le rôle sélectionné par l'utilisateur.
    /// </summary>
    public string SelectedRole { get; private set; }

    /// <summary>
    /// Initialise une nouvelle instance de la fenêtre <see cref="RoleSelectionPopup"/> avec une liste de rôles.
    /// </summary>
    /// <param name="roles">Liste des rôles disponibles à la sélection.</param>
    public RoleSelectionPopup(IEnumerable<string> roles)
    {
        InitializeComponent();
        RoleComboBox.ItemsSource = roles;
        RoleComboBox.SelectedIndex = 0;
    }

    /// <summary>
    /// Gère le clic sur le bouton de validation.
    /// Enregistre le rôle sélectionné, ferme la fenêtre et retourne <c>true</c> comme résultat de la boîte de dialogue.
    /// </summary>
    private void Validate_Click(object sender, RoutedEventArgs e)
    {
        if (RoleComboBox.SelectedItem != null)
        {
            SelectedRole = RoleComboBox.SelectedItem.ToString();
            DialogResult = true;
            Close();
        }
    }
}