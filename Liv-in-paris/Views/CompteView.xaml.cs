using System.Windows.Controls;

namespace Liv_in_paris;

public partial class CompteView : UserControl
{
    public CompteView()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Gère la sélection d'une adresse dans la liste des suggestions.
    /// Met à jour le champ d'adresse avec la valeur choisie et vide la liste des suggestions.
    /// </summary>
    private void Suggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is string selectedAddress)
        {
            if (DataContext is CompteCuisinierViewModel vm)
            {
                vm.AdresseSaisie = selectedAddress;
                vm.Suggestions.Clear();
            }
        }
    }
}