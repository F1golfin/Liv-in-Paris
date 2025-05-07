using Liv_in_paris.Core.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;


namespace Liv_in_paris.Views
{
    /// <summary>
    /// Interaction logic for AjouterPlatView.xaml
    /// Cette vue permet à un cuisinier d’ajouter un nouveau plat à l’application.
    /// Elle est associée au ViewModel AjouterPlatViewModel.
    /// </summary>
    public partial class AjouterPlatView : UserControl
    {
        /// <summary>
        /// Constructeur de la vue d’ajout de plat.
        /// Initialise le DataContext avec un AjouterPlatViewModel.
        /// </summary>
        /// <param name="utilisateur">Utilisateur courant (cuisinier connecté).</param>
        /// <param name="parent">ViewModel principal de l'application.</param>
        public AjouterPlatView(User utilisateur, AppViewModel parent)
        {
            InitializeComponent();
            DataContext = new AjouterPlatViewModel(utilisateur, parent);
        }

        /// <summary>
        /// Méthode appelée lorsqu’on clique sur le bouton pour choisir une image.
        /// Ouvre une boîte de dialogue pour sélectionner un fichier image, l’affiche dans l’UI,
        /// et transmet le chemin de l’image au ViewModel.
        /// </summary>
        /// <param name="sender">Le bouton cliqué.</param>
        /// <param name="e">Événement de clic.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Sélectionnez une image",
                Filter = "Fichiers image (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                ImagePreview.Source = bitmap;
                
                if (DataContext is AjouterPlatViewModel vm)
                {
                    vm.SetImageFromPath(imagePath);
                }
            }
        }
    }
}
