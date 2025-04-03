using System.Windows;

namespace Liv_in_paris.Views;

public partial class RoleSelectionPopup : Window
{
    public string SelectedRole { get; private set; }

    public RoleSelectionPopup(IEnumerable<string> roles)
    {
        InitializeComponent();
        RoleComboBox.ItemsSource = roles;
        RoleComboBox.SelectedIndex = 0;
    }

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