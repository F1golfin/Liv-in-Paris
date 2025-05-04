using System.Windows.Controls;
using System.Windows.Input;

namespace Liv_in_paris;

public partial class PanierView : UserControl
{
    public PanierView()
    {
        InitializeComponent();
    }
    
    private void SuggestionsListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is string selectedAddress)
        {
            if (listBox.DataContext is PlatCommandeViewModel vm)
            {
                vm.AdresseLivraison = selectedAddress;
                vm.Suggestions.Clear();
            }
        }
    }
}