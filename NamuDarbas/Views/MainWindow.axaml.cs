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
        var newWindow = new OmoDesnis();
        newWindow.Show();
    }

    private void Binary(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var newWindow = new Binary();
        newWindow.Show();
    }
}