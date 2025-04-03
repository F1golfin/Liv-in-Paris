using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Liv_in_paris;

public partial class CuisinierView : UserControl
{
    public CuisinierView()
    {
        InitializeComponent();
        
        
    }
    private void RemovePlaceholder(object sender, RoutedEventArgs e)
    {
        var tb = sender as TextBox;
        if (tb != null && tb.Text == (string)tb.Tag)
        {
            tb.Text = "";
            tb.Foreground = Brushes.Black;
        }
    }

    private void AddPlaceholder(object sender, RoutedEventArgs e)
    {
        var tb = sender as TextBox;
        if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
        {
            tb.Text = (string)tb.Tag;
            tb.Foreground = Brushes.Gray;
        }
    }

}