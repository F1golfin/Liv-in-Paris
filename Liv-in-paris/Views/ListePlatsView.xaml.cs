using Liv_in_paris.Core.Models;
using System.Windows.Controls;

namespace Liv_in_paris.Views
{
    /// <summary>
    /// Logique d'interaction pour ListePlats.xaml
    /// </summary>
    public partial class ListePlatsView : UserControl
    {
        public ListePlatsView(User utilisateur, AppViewModel parent)
        {
            InitializeComponent();
            DataContext = new ListePlatsViewModel(parent, utilisateur);
        }
    }
}
