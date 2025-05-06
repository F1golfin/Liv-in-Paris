using System.Windows;
using System.Windows.Input;
using Liv_in_paris;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris
{
    /// <summary>
    /// ViewModel permettant la gestion et la modification des informations de compte utilisateur.
    /// </summary>
    public class CompteViewModel : ViewModelBase
    {
        private readonly User _user;
        private readonly DatabaseManager _db = Database.Instance;
        private readonly NClientViewModel _client;

        /// <summary>
        /// Prénom de l'utilisateur.
        /// </summary>
        public string Prenom { get; set; }

        /// <summary>
        /// Nom de famille de l'utilisateur.
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Adresse e-mail de l'utilisateur.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Adresse postale de l'utilisateur.
        /// </summary>
        public string Adresse { get; set; }

        /// <summary>
        /// Numéro de téléphone de l'utilisateur.
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Nom de l'entreprise si l'utilisateur est un professionnel.
        /// </summary>
        public string? Entreprise { get; set; }

        /// <summary>
        /// Indique si l'utilisateur est un client professionnel (entreprise).
        /// </summary>
        public bool EstEntreprise => _user.Type == "professionnel" && _user.Role.Contains("client");

        /// <summary>
        /// Indique si l'utilisateur est également cuisinier.
        /// </summary>
        public bool EstCuisinier => _user.Role.Contains("cuisinier");

        /// <summary>
        /// Commande pour enregistrer les modifications du profil utilisateur.
        /// </summary>
        public ICommand EnregistrerCommand { get; }

        /// <summary>
        /// Commande pour revenir à la liste des plats.
        /// </summary>
        public ICommand RetourCommand { get; }

        /// <summary>
        /// Initialise une nouvelle instance du <see cref="CompteViewModel"/>.
        /// </summary>
        /// <param name="user">Utilisateur connecté.</param>
        /// <param name="client">ViewModel client associé pour revenir à la vue principale.</param>
        public CompteViewModel(User user, NClientViewModel client)
        {
            _user = user;
            _client = client;

            Prenom = user.Prenom;
            Nom = user.Nom;
            Email = user.Email;
            Adresse = user.Adresse;
            Telephone = user.Telephone;
            Entreprise = user.Entreprise;

            EnregistrerCommand = new RelayCommand(Sauvegarder);
            RetourCommand = new RelayCommand(() => _client.AfficherPlats());
        }

        /// <summary>
        /// Met à jour les informations de l'utilisateur dans la base de données.
        /// Affiche une confirmation à l'utilisateur.
        /// </summary>
        private void Sauvegarder()
        {
            _user.Prenom = Prenom;
            _user.Nom = Nom;
            _user.Email = Email;
            _user.Adresse = Adresse;
            _user.Telephone = Telephone;
            _user.Entreprise = EstEntreprise ? Entreprise : null;

            _user.ModifierUser(_db);
            MessageBox.Show("✅ Informations mises à jour !");
        }
    }
}
