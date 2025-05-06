using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Views;

namespace Liv_in_paris;

/// <summary>
/// ViewModel de la boîte de dialogue d'évaluation d'un cuisinier par un client.
/// Gère la saisie de la note, du commentaire et l'enregistrement de l'évaluation.
/// </summary>
public class EvaluationDialogViewModel : ViewModelBase
{
    /// <summary>
    /// Identifiant du client évaluateur.
    /// </summary>
    public ulong ClientId { get; set; }

    /// <summary>
    /// Identifiant du cuisinier évalué.
    /// </summary>
    public ulong CuisinierId { get; set; }

    private int _note = 5;

    /// <summary>
    /// Note attribuée par le client (entre 1 et 5).
    /// </summary>
    public int Note
    {
        get => _note;
        set { _note = value; OnPropertyChanged(); }
    }

    private string? _commentaire;

    /// <summary>
    /// Commentaire rédigé par le client (facultatif).
    /// </summary>
    public string? Commentaire
    {
        get => _commentaire;
        set { _commentaire = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Commande pour envoyer l'évaluation.
    /// </summary>
    public ICommand EnvoyerEvaluationCommand { get; }

    /// <summary>
    /// Événement permettant de fermer la boîte de dialogue après l'envoi.
    /// </summary>
    public event Action? CloseRequested;

    /// <summary>
    /// Initialise une nouvelle instance du <see cref="EvaluationDialogViewModel"/>.
    /// </summary>
    /// <param name="clientId">Identifiant du client évaluateur.</param>
    /// <param name="cuisinierId">Identifiant du cuisinier évalué.</param>
    public EvaluationDialogViewModel(ulong clientId, ulong cuisinierId)
    {
        ClientId = clientId;
        CuisinierId = cuisinierId;
        EnvoyerEvaluationCommand = new RelayCommand(Envoyer);
    }

    /// <summary>
    /// Enregistre l'évaluation dans la base de données et affiche un message de confirmation.
    /// Ferme la boîte de dialogue si tout s'est bien passé.
    /// </summary>
    private void Envoyer()
    {
        try
        {
            var db = Database.Instance;
            var evaluation = new Evaluation
            {
                ClientId = ClientId,
                CuisinierId = CuisinierId,
                Note = Note,
                Commentaire = Commentaire,
                DateEvaluation = DateTime.Now
            };

            evaluation.Enregistrer(db);
            MessageBox.Show("✅ Merci pour votre évaluation !");
            CloseRequested?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Erreur lors de l'enregistrement : " + ex.Message);
        }
    }

    /// <summary>
    /// Ouvre une boîte de dialogue d'évaluation préremplie avec les identifiants client et cuisinier.
    /// </summary>
    /// <param name="clientId">Identifiant du client évaluateur.</param>
    /// <param name="cuisinierId">Identifiant du cuisinier évalué.</param>
    public static void OuvrirDialog(ulong clientId, ulong cuisinierId)
    {
        var vm = new EvaluationDialogViewModel(clientId, cuisinierId);
        var dialog = new EvaluationDialog { DataContext = vm };
        vm.CloseRequested += () => dialog.Close();
        dialog.ShowDialog();
    }

    /// <summary>
    /// Événement de notification pour la mise à jour d'une propriété (pour le binding).
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifie que la propriété a changé de valeur.
    /// </summary>
    /// <param name="name">Nom de la propriété (automatique si non précisé).</param>
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
