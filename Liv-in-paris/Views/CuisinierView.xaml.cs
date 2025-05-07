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
    
    /// <summary>
    /// Vue principale pour un utilisateur cuisinier.
    /// Permet de naviguer entre la liste des plats, l’ajout de plat, et la visualisation des trajets de livraison.
    /// </summary>
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

    /// <summary>
    /// Constructeur de la vue Cuisinier.
    /// Initialise les sous-vues et met la liste des plats comme vue par défaut.
    /// </summary>
    /// <param name="utilisateur">Utilisateur cuisinier connecté</param>
    /// <param name="parent">ViewModel principal</param>
    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        ContentBox.Content = new ListePlatsView(_utilisateur, _model);
        MesPlats.Background = Brushes.LightGray;
        AjouterPlat.Background = Brushes.Gray;
    }

    /// <summary>
    /// Bouton pour afficher la vue "Mes Plats".
    /// </summary>
    private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
    {
        ContentBox.Content = new AjouterPlatView(_utilisateur, _model);
        AjouterPlat.Background = Brushes.LightGray;
        MesPlats.Background = Brushes.Gray;
    }

    /// <summary>
    /// Bouton pour afficher la vue "Ajouter un plat".
    /// </summary>
    private async void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
    {
        var viewModel = DataContext as CuisinierViewModel;
        if (viewModel == null)
            return;
        
        string adresseDepart = viewModel._utilisateurConnecte.Adresse;
        
        var adressesLivraison = viewModel.Commandes
            .SelectMany(c => c.LignesCommandes)
            .Select(l => l.AdresseArrivee)
            .Distinct()
            .ToList();
        
        if (_vueMetro is MetroGraphView metroView && metroView.DataContext is MetroGraphViewModel metroVM)
        {
            metroVM.InitialiserAdresses(adresseDepart, adressesLivraison);
        }
        
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