using System.Windows.Controls;
using System.Windows.Media;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Views;

namespace Liv_in_paris;

public partial class CuisinierView : UserControl
{
    public User _utilisateur;
    public AppViewModel _model;
    private UserControl _vuePlats;
    private UserControl _vueMetro;
    private bool _estVueSecondaireAffichee = false;
    public CuisinierView(User utilisateur, AppViewModel parent)
    {
        _utilisateur = utilisateur;
        _model = parent;
        
        InitializeComponent();
        ContentBox.Content = new ListePlatsView(_utilisateur, _model);
        _vuePlats = new ListePlatsView(utilisateur, parent);
        _vueMetro = new MetroGraphView();
        MesPlats.Background = Brushes.LightGray;
        AjouterPlat.Background = Brushes.Gray;
        DataContext = new CuisinierViewModel(parent, utilisateur);
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        ContentBox.Content = new ListePlatsView(_utilisateur, _model);
        MesPlats.Background = Brushes.LightGray;
        AjouterPlat.Background = Brushes.Gray;
    }

    private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
    {
        ContentBox.Content = new AjouterPlatView(_utilisateur, _model);
        AjouterPlat.Background = Brushes.LightGray;
        MesPlats.Background = Brushes.Gray;
    }

    private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
    {
        
        if (_estVueSecondaireAffichee)
        {
            ContentBox.Content = _vuePlats;
            _estVueSecondaireAffichee = false;
            AjouterPlat.Visibility = System.Windows.Visibility.Hidden;
            MesPlats.Visibility = System.Windows.Visibility.Hidden;
        }
        else
        {
            ContentBox.Content = _vueMetro;
            _estVueSecondaireAffichee = true;
            AjouterPlat.Visibility = System.Windows.Visibility.Visible;
            MesPlats.Visibility = System.Windows.Visibility.Visible;
        }
    }
}