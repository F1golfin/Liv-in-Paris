using System.Windows;
using System.Windows.Controls;

namespace Liv_in_paris;

public partial class AppView : UserControl
{
    public AppView()
    {
        InitializeComponent();
        DataContext = new AppViewModel();
    }
}