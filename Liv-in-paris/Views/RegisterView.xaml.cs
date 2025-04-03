using System.Windows;
using System.Windows.Controls;

namespace Liv_in_paris;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
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
            vm.Register(); // Appelle la méthode dans ton ViewModel
        }
    }

}