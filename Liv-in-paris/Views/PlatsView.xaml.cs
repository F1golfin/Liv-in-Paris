using System.Windows.Controls;

namespace Liv_in_paris;

public partial class PlatsView : UserControl
{
    public PlatsView()
    {
        InitializeComponent();
    }

    private void RegimeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is PlatsViewModel vm)
        {
            vm.RegimesFiltres.Clear();
            foreach (string selected in RegimeListBox.SelectedItems)
            {
                vm.RegimesFiltres.Add(selected);
            }
        }
    }
}