using System.Windows.Controls;
using System.Windows.Input;

namespace Liv_in_paris;

/// <summary>
/// Vue représentant le panier dans l'application Liv'in Paris.
/// Gère l'affichage et la sélection d'une adresse de livraison parmi les suggestions.
/// </summary>
public partial class PanierView : UserControl
{
    /// <summary>
    /// Initialise les composants de la vue Panier.
    /// </summary>
    public PanierView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gère le clic gauche sur un élément de la liste de suggestions.
    /// Met à jour l'adresse de livraison dans le <see cref="PlatCommandeViewModel"/> et vide les suggestions.
    /// </summary>
    /// <param name="sender">L'objet <see cref="ListBox"/> contenant les suggestions.</param>
    /// <param name="e">Données de l'événement de la souris.</param>
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