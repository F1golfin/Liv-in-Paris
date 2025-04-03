using System;
using System.Windows;
using System.Windows.Controls;

namespace Liv_in_paris;

public partial class ClientView : UserControl
{
    public ClientView()
    {
        InitializeComponent();
        Loaded += ClientView_Loaded;
    }

    private void ClientView_Loaded(object sender, RoutedEventArgs e)
    {
        Console.WriteLine($"[DEBUG] DataContext: {DataContext?.GetType().Name ?? "null"}");

        if (DataContext is ClientViewModel vm)
        {
            Console.WriteLine("[DEBUG] ClientViewModel bien lié !");
        }
        else
        {
            Console.WriteLine("[ERREUR] Le DataContext n'est pas un ClientViewModel !");
        }
    }
}