using Avalonia;
using Avalonia.Controls;
using NamuDarbas.ViewModels;

namespace NamuDarbas.Views;

public partial class Binary : Window
{
    public Binary()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        DataContext = new BinaryWindowView();
    }
}