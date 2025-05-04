using Liv_in_paris.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Sélectionnez une image";
            openFileDialog.Filter = "Fichiers image (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                ImagePreview.Source = bitmap;

                string imagePath = openFileDialog.FileName;
            }
        }
    }
}
