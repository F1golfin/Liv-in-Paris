using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Liv_in_paris.Core.Models;
using Liv_in_paris.Core.Services;
using Liv_in_paris.Views;

namespace Liv_in_paris.ViewModels;

public class EvaluationDialogViewModel : INotifyPropertyChanged
{
    public ulong ClientId { get; set; }
    public ulong CuisinierId { get; set; }

    private int _note = 5;
    public int Note
    {
        get => _note;
        set { _note = value; OnPropertyChanged(); }
    }

    private string? _commentaire;
    public string? Commentaire
    {
        get => _commentaire;
        set { _commentaire = value; OnPropertyChanged(); }
    }

    public ICommand EnvoyerEvaluationCommand { get; }

    public event Action? CloseRequested;

    public EvaluationDialogViewModel(ulong clientId, ulong cuisinierId)
    {
        ClientId = clientId;
        CuisinierId = cuisinierId;
        EnvoyerEvaluationCommand = new RelayCommand(Envoyer);
    }

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
    
    public static void OuvrirDialog(ulong clientId, ulong cuisinierId)
    {
        var vm = new EvaluationDialogViewModel(clientId, cuisinierId);
        var dialog = new EvaluationDialog { DataContext = vm };
        vm.CloseRequested += () => dialog.Close();
        dialog.ShowDialog();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}