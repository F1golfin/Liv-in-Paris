using System.Windows.Controls;
using Liv_in_paris.Core.Models;

namespace Liv_in_paris;

public partial class CuisinierView : UserControl
{
    public CuisinierView(User utilisateur, AppViewModel parent)
    {
        InitializeComponent();
        DataContext = new CuisinierViewModel(parent, utilisateur);
    }
}