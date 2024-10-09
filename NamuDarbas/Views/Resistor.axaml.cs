using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Controls.Shapes;
using NamuDarbas.Utils.Formating;
using NamuDarbas.Utils.ResistorColours;

namespace NamuDarbas.Views
{
    public partial class Resistor : UserControl
    {
        private readonly ComboBox? _bandTypeComboBox;
        private readonly Canvas? _resistorCanvas;
        private readonly StackPanel? _colorSelectors;
        private readonly TextBlock? _resistanceText;
        private readonly TextBlock? _toleranceText;
        private readonly TextBlock? _minResistanceText;
        private readonly TextBlock? _maxResistanceText;
        private readonly TextBlock? _tempCoeffText;
        private readonly List<Rectangle> _colorBands;

        public Resistor()
        {
            InitializeComponent();
            // Duomenu struktura is System.Collections.Generic
            _colorBands = new List<Rectangle>();

            // Initialize control references
            _bandTypeComboBox = this.Find<ComboBox>("BandTypeComboBox");
            _resistorCanvas = this.Find<Canvas>("ResistorCanvas");
            _colorSelectors = this.Find<StackPanel>("ColorSelectors");
            _resistanceText = this.Find<TextBlock>("ResistanceText");
            _toleranceText = this.Find<TextBlock>("ToleranceText");
            _minResistanceText = this.Find<TextBlock>("MinResistanceText");
            _maxResistanceText = this.Find<TextBlock>("MaxResistanceText");
            _tempCoeffText = this.Find<TextBlock>("TempCoeffText");

            // Set up event handlers
            if (_bandTypeComboBox != null) _bandTypeComboBox.SelectionChanged += OnBandTypeChanged!;

            // Initial setup
            SetupUI(4);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnBandTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_bandTypeComboBox != null)
                //Lambda funkcija
                _bandTypeComboBox.SelectionChanged += (sender, e) =>
                {
                    var comboBox = sender as ComboBox;
                    int bandCount = comboBox?.SelectedIndex switch
                    {
                        0 => 4,
                        1 => 5,
                        2 => 6,
                        _ => 4
                    };
                    SetupUI(bandCount);
                };
        }

        private void SetupUI(int bandCount)
        {
            // Clear existing bands and selectors
            _colorBands.Clear();
            _resistorCanvas?.Children.Clear();
            _colorSelectors?.Children.Clear();

            // Add resistor body
            var body = new Rectangle
            {
                Width = 300,
                Height = 30,
                Fill = new SolidColorBrush(Colors.Beige)
            };
            Canvas.SetLeft(body, 50);
            Canvas.SetTop(body, 35);
            _resistorCanvas?.Children.Add(body);

            // Calculate band spacing
            double startX = 70; // Increased from 50 to give more space at the start
            double spacing = 220.0 / (bandCount - 1); // Distribute bands evenly across the resistor

            // Add color bands and selectors
            for (int i = 0; i < bandCount; i++)
            {
                var band = new Rectangle
                {
                    Width = 10,
                    Height = 30,
                    Fill = new SolidColorBrush(Colors.Black)
                };

                // Position the band
                double xPosition = startX + (i * spacing);
                Canvas.SetLeft(band, xPosition);
                Canvas.SetTop(band, 35);
                _resistorCanvas?.Children.Add(band);
                _colorBands.Add(band);

                var selector = new ComboBox
                {
                    Width = 100,
                    Tag = i
                };

                PopulateComboBox(selector, i, bandCount);

                selector.SelectedIndex = 0;
                selector.SelectionChanged += ColorSelector_SelectionChanged!;
                _colorSelectors?.Children.Add(selector);
            }

            UpdateColorBands();
        }

        private void PopulateComboBox(ComboBox selector, int bandPosition, int bandCount)
        {
            IEnumerable<string> colors = bandPosition switch
            {
                var n when n < bandCount - 1 => ResistorColours.ColorDigits.Keys,

                var n when n == bandCount - 2 => ResistorColours.Multipliers.Keys,

                var n when n == bandCount - 1 => ResistorColours.Tolerances.Keys,

                4 when bandCount == 6 => ResistorColours.TempCoefficients.Keys,

                _ => Enumerable.Empty<string>()
            };
            foreach (var color in colors)
            {
                selector.Items.Add(color);
            }
        }

        private void UpdateColorBands()
        {
            var selectors = _colorSelectors?.Children.OfType<ComboBox>().ToList();

            if (selectors != null)
                for (int i = 0; i < selectors.Count && i < _colorBands.Count; i++)
                {
                    string colorName = selectors[i].SelectedItem?.ToString() ?? "Black";
                    if (ResistorColours.ColorValues.TryGetValue(colorName, out Color color))
                    {
                        _colorBands[i].Fill = new SolidColorBrush(color);
                    }
                }
        }
        private void ColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateColorBands();
            CalculateResistance();
        }

        private void CalculateResistance()
        {
            var selectors = _colorSelectors?.Children.OfType<ComboBox>().ToList();
            if (selectors != null)
            {
                int bandCount = selectors.Count;

                try
                {
                    // Get values from selectors
                    List<string> selectedColors = selectors.Select(cb => cb.SelectedItem?.ToString() ?? "Black").ToList();

                    // Calculate resistance
                    double value = 0;

                    switch (bandCount)
                    {
                        case 4:
                            value = (ResistorColours.ColorDigits[selectedColors[0]] * 10 + 
                                     ResistorColours.ColorDigits[selectedColors[1]]) *
                                    ResistorColours.Multipliers[selectedColors[2]];
                            break;

                        case 5:
                        case 6:
                            value = (ResistorColours.ColorDigits[selectedColors[0]] * 100 +
                                     ResistorColours.ColorDigits[selectedColors[1]] * 10 +
                                     ResistorColours.ColorDigits[selectedColors[2]]) *
                                    ResistorColours.Multipliers[selectedColors[3]];
                            break;
                    }

                    double tolerance = ResistorColours.Tolerances[selectedColors[bandCount - 1]];
                    // Lambda Funkcija
                    Func<double, double, double> calculateRange = (val, tol) => val * (1 + tol / 100);
                    double minResistance = calculateRange(value, -tolerance);
                    double maxResistance = calculateRange(value, tolerance);

                    int tempCoeff = 0;
                    if (bandCount == 6 && selectedColors.Count > 4)
                    {
                        tempCoeff = ResistorColours.TempCoefficients[selectedColors[4]];
                    }

                    var resistance = new ResistanceFormat(value);
                    var minRes = new ResistanceFormat(minResistance);
                    var maxRes = new ResistanceFormat(maxResistance);
                    
                    //IFormattable
                    if (_resistanceText != null) _resistanceText.Text = resistance.ToString();
                    if (_toleranceText != null) _toleranceText.Text = $"±{tolerance}%";
                    if (_minResistanceText != null) _minResistanceText.Text = minRes.ToString();
                    if (_maxResistanceText != null) _maxResistanceText.Text = maxRes.ToString();
                    if (_tempCoeffText != null) _tempCoeffText.Text = bandCount == 6 ? $"{tempCoeff} ppm/°C" : "N/A";
                }
                catch
                {
                    if (_resistanceText != null) _resistanceText.Text = "Error";
                    if (_toleranceText != null) _toleranceText.Text = "Error";
                    if (_minResistanceText != null) _minResistanceText.Text = "Error";
                    if (_maxResistanceText != null) _maxResistanceText.Text = "Error";
                    if (_tempCoeffText != null) _tempCoeffText.Text = "Error";
                }
            }
        }
    }
}