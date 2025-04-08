using System;
using System.Windows;
using System.Windows.Controls;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris.Views;

public partial class NouvelleRecetteWindow : Window
{
    private readonly DatabaseManager _db;

    // Constructeur pour nouvelle recette
    public NouvelleRecetteWindow()
    {
        InitializeComponent();
        _db = new DatabaseManager("localhost", "livin_paris", "root", "root");
    }
        
        

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

        if (!int.TryParse(StyleTextBox.Text, out int style))
        {
            MessageBox.Show("Le style doit être un entier.");
            return;
        }

        var recette = new Recette
        {
            NomRecette = NomTextBox.Text,
            Type = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString(),
            Ingredients = IngredientsTextBox.Text,
            StyleCuisine = style,
            RegimeAlimentaire = string.IsNullOrWhiteSpace(RegimeTextBox.Text) ? null : RegimeTextBox.Text
        };


        recette.AjouterRecette(_db);
        MessageBox.Show("Recette enregistrée !");
        this.Close();
    }
}