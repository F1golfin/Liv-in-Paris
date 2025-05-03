using System.Windows;
using System.Windows.Controls;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris.Views;

public partial class AdminView : UserControl
{
    private AdminViewModel _viewModel;

    public AdminView() // plus besoin de passer db en paramètre
    {
        InitializeComponent();
        var db = Database.Instance;
        _viewModel = new AdminViewModel(db);
        DataContext = _viewModel;
        
    }


    private void Supprimer_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SupprimerUtilisateur();
    }
    
    private void ExporterJson_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Fichiers JSON (*.json)|*.json",
            FileName = "users_export.json"
        };
        if (dialog.ShowDialog() == true)
            _viewModel.ExportUsersToJson(dialog.FileName);
    }

    private void ExporterXml_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Fichiers XML (*.xml)|*.xml",
            FileName = "users_export.xml"
        };
        if (dialog.ShowDialog() == true)
            _viewModel.ExportUsersToXml(dialog.FileName);
    }

    private void ImporterJson_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Fichiers JSON (*.json)|*.json"
        };
        if (dialog.ShowDialog() == true)
            _viewModel.ImportUsersFromJson(dialog.FileName);
    }

    private void ImporterXml_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Fichiers XML (*.xml)|*.xml"
        };
        if (dialog.ShowDialog() == true)
            _viewModel.ImportUsersFromXml(dialog.FileName);
    }

}