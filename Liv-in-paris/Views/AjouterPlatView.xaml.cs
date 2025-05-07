using Liv_in_paris.Core.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;


namespace Liv_in_paris.Views
{
    /// <summary>
    /// Logique d'interaction pour AjouterPlatView.xaml
    /// </summary>
    public partial class AjouterPlatView : UserControl
    {
        public AjouterPlatView(User utilisateur, AppViewModel parent)
        {
            InitializeComponent();
            DataContext = new AjouterPlatViewModel(utilisateur, parent);
        }

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
