using System.Windows;
using System.Windows.Input;
using Liv_in_paris;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;

namespace Liv_in_paris
{
    public class CompteViewModel : ViewModelBase
    {
        private readonly User _user;
        private readonly DatabaseManager _db = Database.Instance;

        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }
        public string? Entreprise { get; set; }

        public bool EstEntreprise => _user.Type == "professionnel" && _user.Role.Contains("client");
        public bool EstCuisinier => _user.Role.Contains("cuisinier");

        public ICommand EnregistrerCommand { get; }
        public ICommand RetourCommand { get; }
        
        private readonly NClientViewModel _client;

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

