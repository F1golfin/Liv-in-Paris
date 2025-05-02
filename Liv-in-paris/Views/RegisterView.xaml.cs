using System.Windows;
using System.Windows.Controls;

namespace Liv_in_paris;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
        UserType.SelectedIndex = 0;
    }
    
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

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (UserType.SelectedItem is ComboBoxItem selectedItem)
        {
            string value = selectedItem.Content.ToString();
            TxtEntreprise.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;
            NomEntreprise.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;
            CooReferent.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;
            InfoEntreprise.Visibility = (value == "Particulier") ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}