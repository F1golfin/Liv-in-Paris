using Liv_in_paris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
