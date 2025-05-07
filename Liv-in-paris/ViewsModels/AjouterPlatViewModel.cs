using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Liv_in_paris
{
    class AjouterPlatViewModel : ViewModelBase
    {
        private readonly AppViewModel _app;
        private readonly User _utilisateurConnecte;

        public ObservableCollection<Plat> Plats { get; set; }
        public ObservableCollection<Recette> RecettesExistantes { get; set; }

        private ImageSource _image;
        /// <summary>
        /// Image à afficher dans la vue, et potentiellement à sauvegarder.
        /// </summary>
        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public string NewNomPlat { get; set; }
        public string NewPrixPlat { get; set; }
        public string NewTypePlat { get; set; }
        public Recette RecetteSelectionnee { get; set; }
        private string _newNbParts;
        
        /// <summary>
        /// Nombre de parts du plat à ajouter.
        /// </summary>
        public string NewNbParts
        {
            get => _newNbParts;
            set
            {
                _newNbParts = value;
                OnPropertyChanged(nameof(NewNbParts));
            }
        }

        public ObservableCollection<Evaluation> EvaluationsRecues { get; set; }

        /// <summary>
        /// Commande liée au bouton "Ajouter le plat".
        /// </summary>
        public ICommand AjouterPlatCommand { get; }
        public ICommand SupprimerPlatCommand { get; }
        /// <summary>
        /// Commande pour ajouter une nouvelle recette.
        /// </summary>
        public ICommand AjouterNouvelleRecetteCommand { get; }
        
        /// <summary>
        /// Constructeur du ViewModel. Initialise les commandes et charge les données.
        /// </summary>
        /// <param name="utilisateur">Utilisateur cuisinier connecté</param>
        /// <param name="parent">ViewModel principal</param>
        public AjouterPlatViewModel(User utilisateur, AppViewModel parent)
        {
            _app = parent;
            _utilisateurConnecte = utilisateur;
            AjouterPlatCommand = new RelayCommand(AjouterPlat);
            AjouterNouvelleRecetteCommand = new RelayCommand(AjouterNouvelleRecette);
            ChargerDonnees();
        }
        
        /// <summary>
        /// Charge les plats et recettes depuis la base de données.
        /// </summary>
        private void ChargerDonnees()
        {
            var db = Database.Instance;

            Plats = new ObservableCollection<Plat>(Plat.GetAllByCuisinier(db, _utilisateurConnecte.UserId));
            RecettesExistantes = new ObservableCollection<Recette>(Recette.GetAll(db));
            OnPropertyChanged(nameof(Plats));
            OnPropertyChanged(nameof(RecettesExistantes));


        }

        /// <summary>
        /// Ajoute un nouveau plat à la base de données après validation des champs.
        /// </summary>
        private void AjouterPlat()
        {
            if (string.IsNullOrWhiteSpace(NewNomPlat) || string.IsNullOrWhiteSpace(NewPrixPlat) || string.IsNullOrWhiteSpace(NewTypePlat))
            {
                MessageBox.Show("Veuillez renseigner le nom, le prix et le type du plat.");
                return;
            }

            if (!decimal.TryParse(NewPrixPlat, out decimal prix))
            {
                MessageBox.Show("Prix invalide.");
                return;
            }

            if (!int.TryParse(NewNbParts, out int nbParts) || nbParts <= 0)
            {
                MessageBox.Show("Nombre de parts invalide.");
                return;
            }

            var db = Database.Instance;
            byte[] imageBytes = null;
            if (Image is BitmapSource bitmapSource)
            {
                using (var stream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder(); 
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(stream);
                    imageBytes = stream.ToArray();
                }
            }
            var nouveauPlat = new Plat
            {
                NomPlat = NewNomPlat,
                PrixParPersonne = prix,
                NbParts = nbParts,
                DateFabrication = DateTime.Now,
                DatePeremption = DateTime.Now.AddDays(3),
                CuisinierId = _utilisateurConnecte.UserId,
                Photo = imageBytes,
                RecetteId = RecetteSelectionnee?.RecetteId ?? 0
            };

            nouveauPlat.AjouterPlat(db);
            MessageBox.Show("Plat ajouté !");
            ChargerDonnees();
        }
        
        /// <summary>
        /// Ouvre une nouvelle fenêtre pour créer une recette personnalisée.
        /// </summary>
        private void AjouterNouvelleRecette()
        {
            var fenetre = new NouvelleRecetteWindow();
            fenetre.ShowDialog();
            ChargerDonnees();
        }
        
        /// <summary>
        /// Charge une image à partir d’un chemin local (utilisé par l’UI).
        /// </summary>
        /// <param name="path">Chemin d'accès à l'image</param>
        public void SetImageFromPath(string path)
        {
            if (!File.Exists(path)) return;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();

            Image = bitmap;
        }
    }
}
