using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using NamuDarbas.Utils.ResistorColours;


namespace NamuDarbas.Views;

public partial class Resistor : Window
{
    public Resistor()
    {
        InitializeComponent();

        PopulateColors();

        BandSelection.SelectionChanged += BandSelectionChanged;
        Band1.SelectionChanged += ResistorCalculate;
        Band2.SelectionChanged += ResistorCalculate;
        Band3.SelectionChanged += ResistorCalculate;
        Multiplier.SelectionChanged += ResistorCalculate;
        Tolerance.SelectionChanged += ResistorCalculate;
        TemperatureCoefficient.SelectionChanged += ResistorCalculate;
    }

    private void PopulateColors()
    {
        var colours = ResistorColours.ColorDigitMap.Keys.ToList();
        Band1.ItemsSource = new List<string>(colours);
        Band2.ItemsSource = new List<string>(colours);
        Band3.ItemsSource = new List<string>(colours);
        Multiplier.ItemsSource = new List<string>(ResistorColours.ColorMultiplier.Keys.ToList());
        Tolerance.ItemsSource = new List<string>(ResistorColours.ColorTolerance.Keys.ToList());
        TemperatureCoefficient.ItemsSource = new List<string>(ResistorColours.ColorTemperature.Keys.ToList());
    }

    private void BandSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        bool isFiveBands = BandSelection.SelectedIndex == 1;
        bool isSixBands = BandSelection.SelectedIndex == 2;

        Band3.IsVisible = isFiveBands || isSixBands;
        Band3Rect.IsVisible = isFiveBands || isSixBands;

        TemperatureCoefficient.IsVisible = isSixBands;

        ResistorCalculate(null, null!);
    }

    private void ResistorCalculate(object? sender, SelectionChangedEventArgs e)
    {
        if (Band1.SelectedItem == null || Band2.SelectedItem == null || Multiplier.SelectedItem == null || Tolerance.SelectedItem == null)
        {
          return;  
        }

        if (Band3.IsVisible && Band3.SelectedItem == null)
        {
            return;
        }

        if (TemperatureCoefficient.IsVisible && TemperatureCoefficient.SelectedItem == null)
        {
            return;
        }
        if (!ResistorColours.ColorDigitMap.TryGetValue(Band1.SelectedItem.ToString()!, out int band1) || 
            !ResistorColours.ColorDigitMap.TryGetValue(Band2.SelectedItem.ToString()!, out int band2))   
        {                                                                                                
            return;                     
        }
        
        int band3 = 0;
        
        if (Band3.IsVisible && Band3.SelectedItem != null && !ResistorColours.ColorDigitMap.TryGetValue(Band3.SelectedItem.ToString()!, out band3))
        {
            return;
        }
        
        if (!ResistorColours.ColorMultiplier.TryGetValue(Multiplier.SelectedItem.ToString()!, out double multiplier))
        {
            return;
        }

        if (!ResistorColours.ColorTolerance.TryGetValue(Tolerance.SelectedItem.ToString()!, out double tolerance))
        {
            return;
        }

        double resistence = 0;
        if (Band3.IsVisible)
        {
            resistence = (band1 * 100 + band2 * 10 + band3) * multiplier;
        }
        else
        {
            resistence = (band1 * 10 + band2) * multiplier;
        }

        double minValue = resistence * (1 - tolerance / 100);
        double maxValue = resistence * (1 + tolerance / 100);

        ResistanceValue.Text = $"{resistence} Ω";
        ToleranceValue.Text = $"{tolerance}%";
        MinValue.Text = $"{minValue} Ω";
        MaxValue.Text = $"{maxValue} Ω";
        if (TemperatureCoefficient.IsVisible && TemperatureCoefficient.SelectedItem != null)
        {
            if (ResistorColours.ColorTemperature.TryGetValue(TemperatureCoefficient.SelectedItem.ToString()!, out int tempCoefficient))
            {
                TemperatureCoefficientValue.Text = $"{tempCoefficient} ppm/°C";
            }
        }
        else
        {
            TemperatureCoefficientValue.Text = string.Empty;
        }

        Band1Rect.Fill = new SolidColorBrush(Color.Parse(Band1.SelectedItem.ToString()!));
        Band2Rect.Fill = new SolidColorBrush(Color.Parse(Band2.SelectedItem.ToString()!));
        Band3Rect.Fill = Band3.IsVisible ? new SolidColorBrush(Color.Parse(Band3.SelectedItem?.ToString()!)) : Brushes.Transparent;
        MultiplierRect.Fill = new SolidColorBrush(Color.Parse(Multiplier.SelectedItem.ToString()!));
        ToleranceRect.Fill = new SolidColorBrush(Color.Parse(Tolerance.SelectedItem.ToString()!));
        TemperatureRect.Fill = TemperatureCoefficient.IsVisible
            ? new SolidColorBrush(Color.Parse(TemperatureCoefficient.SelectedItem?.ToString()!))
            : Brushes.Transparent;
        
    }
}