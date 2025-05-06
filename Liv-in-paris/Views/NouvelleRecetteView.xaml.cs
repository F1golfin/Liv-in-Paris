using System;
using System.Windows;
using System.Windows.Controls;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris.Views;

/// <summary>
/// Fenêtre permettant de créer une nouvelle recette dans l'application Liv'in Paris.
/// </summary>
public partial class NouvelleRecetteWindow : Window
{
    private readonly DatabaseManager _db;

    /// <summary>
    /// Initialise une nouvelle instance de la fenêtre et charge les régimes alimentaires disponibles.
    /// </summary>
    public NouvelleRecetteWindow()
    {
        InitializeComponent();
        _db = Database.Instance;

        var regimes = RegimeAlimentaire.GetAll(_db);
        RegimesListBox.ItemsSource = regimes;
    }

    /// <summary>
    /// Gère le clic sur le bouton de création de recette.
    /// Vérifie les champs obligatoires, crée l'objet <see cref="Recette"/>, l'enregistre dans la base, puis ferme la fenêtre.
    /// </summary>
    private void CreerRecette_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomTextBox.Text) ||
            TypeComboBox.SelectedItem == null ||
            string.IsNullOrWhiteSpace(IngredientsTextBox.Text) ||
            string.IsNullOrWhiteSpace(StyleTextBox.Text))
        {
            MessageBox.Show("Veuillez remplir tous les champs obligatoires.");
            return;
        }

        var recette = new Recette
        {
            NomRecette = NomTextBox.Text,
            Type = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString(),
            Ingredients = IngredientsTextBox.Text,
            StyleCuisine = StyleTextBox.Text
        };
        
        foreach (RegimeAlimentaire regime in RegimesListBox.SelectedItems)
        {
            recette.RegimeIds.Add(regime.RegimeId);
        }

        recette.AjouterRecette(_db);
        MessageBox.Show("Recette enregistrée !");
        Close();
    }
}