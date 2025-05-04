using System.Windows;
using System.Windows.Controls;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Liv_in_paris.Views;

public partial class AdminView : UserControl
{
    private AdminViewModel _viewModel;

    public AdminView(AppViewModel parent) 
    {
        InitializeComponent();
        _viewModel = new AdminViewModel(parent);
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
    private void GraphCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        if (_viewModel.GrapheColoration == null)
            return;

        // Récupère les nœuds et leurs couleurs
        var graphe = _viewModel.GrapheColoration;
        var couleurs = _viewModel.DerniereColoration; // Dictionnaire<int, int>

        var radius = 20;
        var centerX = e.Info.Width / 2;
        var centerY = e.Info.Height / 2;
        var angleStep = 360.0 / graphe.Noeuds.Count;
        double angle = 0;

        var positions = new Dictionary<int, SKPoint>();
        var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };

        // Positionne les nœuds en cercle
        foreach (var noeud in graphe.Noeuds.Values)
        {
            var x = (float)(centerX + 150 * Math.Cos(angle * Math.PI / 180));
            var y = (float)(centerY + 150 * Math.Sin(angle * Math.PI / 180));
            positions[noeud.Id] = new SKPoint(x, y);
            angle += angleStep;
        }

        // Dessine les liens
        paint.Color = SKColors.Gray;
        paint.StrokeWidth = 2;
        foreach (var lien in graphe.Liens)
        {
            canvas.DrawLine(positions[lien.Origine.Id], positions[lien.Destination.Id], paint);
        }

        // Dessine les nœuds colorés
        foreach (var noeud in graphe.Noeuds.Values)
        {
            int colorIndex = couleurs[noeud.Id];
            paint.Color = GetColorForIndex(colorIndex);
            canvas.DrawCircle(positions[noeud.Id], radius, paint);

            // Ajoute l’ID
            paint.Color = SKColors.Black;
            paint.TextSize = 14;
            canvas.DrawText(noeud.Id.ToString(), positions[noeud.Id].X - 5, positions[noeud.Id].Y + 5, paint);
        }
    }

    private SKColor GetColorForIndex(int i)
    {
        var palette = new[] {
            SKColors.Red, SKColors.Blue, SKColors.Green,
            SKColors.Orange, SKColors.Purple, SKColors.Brown,
            SKColors.Teal, SKColors.Pink, SKColors.YellowGreen
        };
        return i < palette.Length ? palette[i] : SKColors.Black;
    }



    private void AnalyserColoration_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.AnalyserColoration();
        GraphCanvas.InvalidateVisual(); // <- très important pour forcer le redraw SkiaSharp
    }


    private void AfficherLivraisonsParCuisinier(object sender, RoutedEventArgs e) =>
        _viewModel.AfficherLivraisonsParCuisinier();

    private void AfficherCommandesParPeriode(object sender, RoutedEventArgs e) =>
        _viewModel.AfficherCommandesParPeriode();

    private void AfficherMoyennePrixCommandes(object sender, RoutedEventArgs e) =>
        _viewModel.AfficherMoyennePrixCommandes();

    private void AfficherMoyenneComptesClients(object sender, RoutedEventArgs e) =>
        _viewModel.AfficherMoyenneComptesClients();

    private void AfficherCommandesClientFiltrées(object sender, RoutedEventArgs e) =>
        _viewModel.AfficherCommandesClientFiltrées();



}