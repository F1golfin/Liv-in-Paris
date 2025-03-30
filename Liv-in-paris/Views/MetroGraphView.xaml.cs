using System.Windows;
using System.Windows.Controls;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Liv_in_paris;

public partial class MetroGraphView : UserControl
{
    public MetroGraphView()
    {
        InitializeComponent();
        DataContext = new MetroGraphViewModel();
    }
    
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        // Exemple test
        using var paint = new SKPaint
        {
            Color = SKColors.SteelBlue,
            IsAntialias = true
        };
        canvas.DrawCircle(100, 100, 30, paint);
        canvas.DrawText("Métro de Paris", 150, 105, new SKPaint { Color = SKColors.Black, TextSize = 24 });
        
        
        // Déssiner le graphe ICI
    }
}