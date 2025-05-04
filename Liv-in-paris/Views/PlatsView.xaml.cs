using System.Windows.Controls;

namespace Liv_in_paris;

/// <summary>
/// Vue permettant d'afficher et de filtrer les plats proposés dans l'application Liv'in Paris.
/// </summary>
public partial class PlatsView : UserControl
{
    /// <summary>
    /// Initialise les composants de la vue Plats.
    /// </summary>
    public PlatsView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gère le changement de sélection dans la liste des régimes alimentaires.
    /// Met à jour la liste des régimes filtrés dans le <see cref="PlatsViewModel"/>.
    /// </summary>
    /// <param name="sender">Objet <see cref="ListBox"/> source de l'événement.</param>
    /// <param name="e">Données de l'événement de changement de sélection.</param>
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