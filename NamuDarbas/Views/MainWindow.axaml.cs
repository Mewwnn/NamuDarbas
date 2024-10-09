using Avalonia.Controls;

namespace NamuDarbas.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OmoDesnis(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var omoDesnioLangas = new OmoDesnis();
        omoDesnioLangas.Show();
    }

    private void Binary(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var binaryLangas = new Binary();
        binaryLangas.Show();
    }

    private void ASCII(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var aSCIILangas = new ASCII();
        aSCIILangas.Show();
    }
    
    public void Resistor(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var resistorWindow = new Window
        {
            Title = "Resistor Calculator",
            Content = new Resistor(),
            Width = 800,
            Height = 700
        };
        resistorWindow.Show();
    }
}
