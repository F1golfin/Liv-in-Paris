using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Liv_in_paris.Core.Entities;
using Liv_in_paris.Core.Graph;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Liv_in_paris;

public partial class MetroGraphView : UserControl
{
    private MetroGraphViewModel _viewModel;
    
    // Pour le dessin
    private float _scale = 0.9f;
    private SKPoint _offset = new(0, 0);
    private SKPoint _lastTouch;
    private bool _isDragging = false;
    
    private int _trajetStep = 0;
    private List<Noeud<Station>> _trajet = new();
    private DispatcherTimer _animationTimer;
    
    
    public MetroGraphView()
    {
        InitializeComponent();
        _viewModel = new MetroGraphViewModel();
        DataContext = _viewModel;
        
        _viewModel.OnCheminCalcule = cheminIds =>
        {
            var chemin = ConvertirEnCheminNoeuds(cheminIds);
            if (chemin.Count >= 2)
                LancerAnimationTrajet(chemin);
        };
    }
    
    private List<Station> GetStationsUniques()
    {
        return _viewModel.Graphe.Noeuds
            .Values
            .Select(n => n.Data)
            .GroupBy(s => (s.Nom, s.Latitude, s.Longitude))
            .Select(g => g.First())
            .ToList();
    }
    
    private List<Noeud<Station>> ConvertirEnCheminNoeuds(List<int> ids)
    {
        return ids
            .Where(id => _viewModel.Graphe.Noeuds.ContainsKey(id))
            .Select(id => _viewModel.Graphe.Noeuds[id])
            .ToList();
    }
    
    private Dictionary<(string nom, double lat, double lon), HashSet<string>> GetLignesParStation()
    {
        return _viewModel.Graphe.Noeuds
            .Values
            .GroupBy(n => (n.Data.Nom, n.Data.Latitude, n.Data.Longitude))
            .ToDictionary(
                g => g.Key,
                g => g.Select(n => n.Data.Ligne).ToHashSet()
            );
    }
    
    private readonly SKPaint textPaint = new SKPaint
    {
        Color = SKColors.Black,
        TextSize = 12,
        IsAntialias = true,
        Typeface = SKTypeface.Default
    };
    
    private readonly SKPaint stationPaint = new SKPaint
    {
        Color = SKColors.Black,
        IsAntialias = true
    };
    
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);
        
        canvas.Scale(_scale);
        canvas.Translate(_offset.X / _scale, _offset.Y / _scale);
        
        Dictionary<string, SKColor> ligneCouleurs = new()
        {
            ["1"] = SKColors.Gold,
            ["2"] = SKColors.DeepSkyBlue,
            ["3"] = SKColors.Olive,
            ["3bis"] = SKColors.MediumSeaGreen,
            ["4"] = SKColors.Magenta,
            ["5"] = SKColors.Orange,
            ["6"] = SKColors.LightGreen,
            ["7"] = SKColors.Salmon,
            ["7bis"] = SKColors.SkyBlue,
            ["8"] = SKColors.Violet,
            ["9"] = SKColors.YellowGreen,
            ["10"] = SKColors.Tan,
            ["11"] = SKColors.Sienna,
            ["12"] = SKColors.ForestGreen,
            ["13"] = SKColors.DarkGreen,
            ["14"] = SKColors.DarkViolet
        };

        var largeur = e.Info.Width;
        var hauteur = e.Info.Height;

        var graphe = _viewModel.Graphe;
        var stations = GetStationsUniques();
        
        // Trouve les limites géographiques
        double minLat = stations.Min(s => s.Latitude);
        double maxLat = stations.Max(s => s.Latitude);
        double minLon = stations.Min(s => s.Longitude);
        double maxLon = stations.Max(s => s.Longitude);

        // Fonction de transformation GPS => Canvas
        SKPoint ConvertirCoord(Station s)
        {
            float x = (float)((s.Longitude - minLon) / (maxLon - minLon) * (largeur - 40) + 20);
            float y = (float)((1 - (s.Latitude - minLat) / (maxLat - minLat)) * (hauteur - 40) + 20);
            return new SKPoint(x, y);
        }

        // Dessine les arêtes (liens entre les noeuds)
        // Regroupe les liens par tronçon (même stations, ordre indifférent)
        var tronconsParStations = _viewModel.Graphe.Liens
            .GroupBy(lien =>
            {
                var id1 = lien.Origine.Id;
                var id2 = lien.Destination.Id;
                return id1 < id2 ? (id1, id2) : (id2, id1);
            });

        float decalage = 3;

        foreach (var troncon in tronconsParStations)
        {
            var liens = troncon.ToList();
            if (liens.Count == 0) continue;

            // Position d'affichage (à partir du 1er lien)
            var p1 = ConvertirCoord(liens[0].Origine.Data);
            var p2 = ConvertirCoord(liens[0].Destination.Data);

            // Calcul d'un vecteur perpendiculaire pour décaler les traits
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            var longueur = Math.Sqrt(dx * dx + dy * dy);
            var nx = -(dy / longueur);
            var ny = dx / longueur;

            for (int i = 0; i < liens.Count; i++)
            {
                var ligne = liens[i].Origine.Data.Ligne;
                var couleur = ligneCouleurs.ContainsKey(ligne) ? ligneCouleurs[ligne] : SKColors.Gray;

                using var paint = new SKPaint
                {
                    Color = couleur,
                    StrokeWidth = 2,
                    IsAntialias = true
                };

                float offset = (i - (liens.Count - 1) / 2.0f) * decalage;

                var p1Decale = new SKPoint((float)(p1.X + nx * offset), (float)(p1.Y + ny * offset));
                var p2Decale = new SKPoint((float)(p2.X + nx * offset), (float)(p2.Y + ny * offset));

                canvas.DrawLine(p1Decale, p2Decale, paint);
            }
        }

        var lignesParStation = GetLignesParStation();
        foreach (var station in stations)
        {
            var point = ConvertirCoord(station);
            canvas.DrawCircle(point, 6, stationPaint);
            
            canvas.DrawText(station.Nom, point.X + 6, point.Y - 6, textPaint);
            
            // Affiche les lignes associées (L1, L4, L7...)
            var key = (station.Nom, station.Latitude, station.Longitude);
            if (lignesParStation.ContainsKey(key))
            {
                var lignes = string.Join(", ", lignesParStation[key]);
                canvas.DrawText($"({lignes})", point.X + 8, point.Y + 10, textPaint);
            }
        }
        
        if (_trajet != null && _trajet.Count > 1)
        {
            using var trajetPaint = new SKPaint
            {
                Color = SKColors.Red,
                StrokeWidth = 5,
                IsAntialias = true
            };

            for (int i = 0; i < Math.Min(_trajetStep, _trajet.Count - 1); i++)
            {
                var p1 = ConvertirCoord(_trajet[i].Data);
                var p2 = ConvertirCoord(_trajet[i + 1].Data);

                canvas.DrawLine(p1, p2, trajetPaint);
            }
        }
    }
    
    // Gérer la molette de la souris
    private void SKElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var skElement = (SKElement)sender;
        var position = e.GetPosition(skElement).ToSKPoint();

        float oldScale = _scale;
        float zoomFactor = (e.Delta > 0) ? 1.1f : 0.9f;

        _scale *= zoomFactor;
        _scale = Math.Clamp(_scale, 0.9f, 3f);

        // Ajustement de l'offset pour centrer le zoom autour de la souris
        _offset = new SKPoint(
            (position.X - _offset.X) - (position.X - _offset.X) * (_scale / oldScale) + _offset.X,
            (position.Y - _offset.Y) - (position.Y - _offset.Y) * (_scale / oldScale) + _offset.Y
        );
        LimiterOffset((int)skElement.ActualWidth, (int)skElement.ActualHeight);
        skElement.InvalidateVisual();
    }
    
    private void SKElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _lastTouch = e.GetPosition((IInputElement)sender).ToSKPoint();
    }

    private void SKElement_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging)
        {
            var current = e.GetPosition((IInputElement)sender).ToSKPoint();
            _offset += current - _lastTouch;
            _lastTouch = current;

            LimiterOffset((int)skElement.ActualWidth, (int)skElement.ActualHeight);
            skElement.InvalidateVisual();
        }
    }

    private void SKElement_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
    }
    
    private void ReinitialiserVue_Click(object sender, RoutedEventArgs e)
    {
        _scale = 0.9f; 
        _offset = new SKPoint(0, 0); 
        skElement.InvalidateVisual();
    }
    
    private void LimiterOffset(int canvasWidth, int canvasHeight)
    {
        float marge = 1200f;

        float minX = -canvasWidth * (_scale - 1) - marge;
        float maxX = canvasWidth - marge;

        float minY = -canvasHeight * (_scale - 1) - marge;
        float maxY = canvasHeight - marge;

        _offset = new SKPoint(
            Math.Clamp(_offset.X, minX, maxX),
            Math.Clamp(_offset.Y, minY, maxY)
        );
    }
    
    
    private void LancerAnimationTrajet(List<Noeud<Station>> chemin)
    {
        _trajet = chemin;
        _trajetStep = 1;

        _animationTimer = new DispatcherTimer();
        _animationTimer.Interval = TimeSpan.FromMilliseconds(500); // vitesse animation
        _animationTimer.Tick += (s, e) =>
        {
            _trajetStep++;

            if (_trajetStep >= _trajet.Count)
                _animationTimer.Stop();

            skElement.InvalidateVisual();
        };

        _animationTimer.Start();
    }
    
    private void ReinitialiserTrajet_Click(object sender, RoutedEventArgs e)
    {
        _trajet.Clear();
        _trajetStep = 0;
        _animationTimer?.Stop();
        skElement.InvalidateVisual();
    }
}