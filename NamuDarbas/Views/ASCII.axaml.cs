using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using System.Text;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

namespace NamuDarbas.Views;

public partial class ASCII : Window
{
    private bool IsAsciiToBinary = false;
    
    public ASCII()
    {
        InitializeComponent();
    }
    
    private string BinaryToAscii(string binIvestis)
    {
        binIvestis = binIvestis.Replace(" ", ""); // Panaikinti tarpus     

        if (binIvestis.Length % 8 != 0)
        {
            throw new Exception("Dvejetainio kodo ilgis turi buti sudarytas iš 8 bitų");
        }

        var bytai = Enumerable.Range(0, binIvestis.Length / 8)
            .Select(i => Convert.ToByte(binIvestis.Substring(i * 8, 8), 2))
            .ToArray();

        return Encoding.ASCII.GetString(bytai);
    }

    private string AsciiToBinary(string asciiIvestis)
    {
        StringBuilder binaryBuilder = new StringBuilder();

        foreach (char c in asciiIvestis)
        {
            binaryBuilder.Append(Convert.ToString(c, 2).PadLeft(8, '0')).Append(" ");
        }

        return binaryBuilder.ToString().Trim(); // Pašalina galinį tarpą.
    }
    private void Konvertuoti(object sender, RoutedEventArgs e)
    {
        string? input = Ivedimas.Text;
        string rezultatas;

        try
        {
            if (IsAsciiToBinary)
            {
                rezultatas = AsciiToBinary(input);
            }
            else
            {
                rezultatas = BinaryToAscii(input);
            }

            Isvedimas.Text = rezultatas;
        }
        catch (Exception ex)
        {
            Isvedimas.Text = $"Error: {ex.Message}";
        }
        
    }

    private void PakeistiRezima(object sender, RoutedEventArgs e)
    {
        IsAsciiToBinary = Pasirinkimas.IsChecked ?? false;

        if (IsAsciiToBinary)
        {
            IvedimoPavadinimas.Text = "Iveskite ASCII eilutę";
            Ivedimas.Text = "";
            Isvedimas.Text = "";
        }
        else
        {
            IvedimoPavadinimas.Text = "Iveskite Dvejetainio kodo eilutę";
            Ivedimas.Text = "";
            Ivedimas.Text = "";
        }
    }
    
    
}