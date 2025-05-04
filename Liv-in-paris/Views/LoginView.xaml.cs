using System.Windows;
using System.Windows.Controls;

namespace Liv_in_paris;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        UserType.SelectedIndex = 0;
    }

    private void SeConnecter_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = MdPInput.Password;
            vm.LoginCommand.Execute(null);
        }
    }

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