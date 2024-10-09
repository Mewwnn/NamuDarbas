using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Controls.Shapes;
using NamuDarbas.Utils.ResistorColours;

namespace NamuDarbas.Views
{
    public partial class Resistor : UserControl
    {
        private ComboBox bandTypeComboBox;
        private Canvas resistorCanvas;
        private StackPanel colorSelectors;
        private TextBlock resistanceText;
        private TextBlock toleranceText;
        private TextBlock minResistanceText;
        private TextBlock maxResistanceText;
        private TextBlock tempCoeffText;
        private List<Rectangle> colorBands;

        public Resistor()
        {
            InitializeComponent();
            colorBands = new List<Rectangle>();

            // Initialize control references
            bandTypeComboBox = this.Find<ComboBox>("BandTypeComboBox");
            resistorCanvas = this.Find<Canvas>("ResistorCanvas");
            colorSelectors = this.Find<StackPanel>("ColorSelectors");
            resistanceText = this.Find<TextBlock>("ResistanceText");
            toleranceText = this.Find<TextBlock>("ToleranceText");
            minResistanceText = this.Find<TextBlock>("MinResistanceText");
            maxResistanceText = this.Find<TextBlock>("MaxResistanceText");
            tempCoeffText = this.Find<TextBlock>("TempCoeffText");

            // Set up event handlers
            bandTypeComboBox.SelectionChanged += OnBandTypeChanged;

            // Initial setup
            SetupUI(4);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnBandTypeChanged(object sender, SelectionChangedEventArgs e)
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
        }

        private void SetupUI(int bandCount)
        {
            // Clear existing bands and selectors
            colorBands.Clear();
            resistorCanvas.Children.Clear();
            colorSelectors.Children.Clear();

            // Add resistor body
            var body = new Rectangle
            {
                Width = 300,
                Height = 30,
                Fill = new SolidColorBrush(Colors.Beige)
            };
            Canvas.SetLeft(body, 50);
            Canvas.SetTop(body, 35);
            resistorCanvas.Children.Add(body);

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
                resistorCanvas.Children.Add(band);
                colorBands.Add(band);

                var selector = new ComboBox
                {
                    Width = 100,
                    Tag = i
                };

                // Add appropriate colors based on band position
                if (i < bandCount - 2)
                {
                    foreach (var color in ResistorColours.ColorDigits.Keys)
                        selector.Items.Add(color);
                }
                else if (i == bandCount - 2)
                {
                    foreach (var color in ResistorColours.Multipliers.Keys)
                        selector.Items.Add(color);
                }
                else if (i == bandCount - 1)
                {
                    foreach (var color in ResistorColours.Tolerances.Keys)
                        selector.Items.Add(color);
                }

                selector.SelectedIndex = 0;
                selector.SelectionChanged += ColorSelector_SelectionChanged;
                colorSelectors.Children.Add(selector);
            }

            UpdateColorBands();
        }

        private void ColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateColorBands();
            CalculateResistance();
        }

        private void UpdateColorBands()
        {
            var selectors = colorSelectors.Children.OfType<ComboBox>().ToList();
            
            for (int i = 0; i < selectors.Count && i < colorBands.Count; i++)
            {
                string colorName = selectors[i].SelectedItem?.ToString() ?? "Black";
                if (ResistorColours.ColorValues.TryGetValue(colorName, out Color color))
                {
                    colorBands[i].Fill = new SolidColorBrush(color);
                }
            }
        }

        private void CalculateResistance()
        {
            var selectors = colorSelectors.Children.OfType<ComboBox>().ToList();
            int bandCount = selectors.Count;
            
            // Get values from selectors
            List<string> selectedColors = selectors.Select(cb => cb.SelectedItem?.ToString() ?? "Black").ToList();
            
            try
            {
                // Calculate resistance
                double value = 0;
                if (bandCount >= 4)
                {
                    if (bandCount == 4)
                    {
                        value = (ResistorColours.ColorDigits[selectedColors[0]] * 10 + 
                                ResistorColours.ColorDigits[selectedColors[1]]) *
                                ResistorColours.Multipliers[selectedColors[2]];
                    }
                    else if (bandCount == 5 || bandCount == 6)
                    {
                        value = (ResistorColours.ColorDigits[selectedColors[0]] * 100 +
                                ResistorColours.ColorDigits[selectedColors[1]] * 10 +
                                ResistorColours.ColorDigits[selectedColors[2]]) *
                                ResistorColours.Multipliers[selectedColors[3]];
                    }
                }
                
                // Get tolerance
                double tolerance = ResistorColours.Tolerances[selectedColors[bandCount - 1]];
                
                // Calculate min and max resistance
                double minResistance = value * (1 - tolerance / 100);
                double maxResistance = value * (1 + tolerance / 100);
                
                // Get temperature coefficient if 6-band
                int tempCoeff = 0;
                if (bandCount == 6 && selectedColors.Count > 4)
                {
                    tempCoeff = ResistorColours.TempCoefficients.GetValueOrDefault(selectedColors[4], 0);
                }
                
                // Update UI
                resistanceText.Text = $"{FormatValue(value)} Ω";
                toleranceText.Text = $"±{tolerance}%";
                minResistanceText.Text = $"{FormatValue(minResistance)} Ω";
                maxResistanceText.Text = $"{FormatValue(maxResistance)} Ω";
                tempCoeffText.Text = bandCount == 6 ? $"{tempCoeff} ppm/°C" : "N/A";
            }
            catch
            {
                // Handle any calculation errors
                resistanceText.Text = "Error";
                toleranceText.Text = "Error";
                minResistanceText.Text = "Error";
                maxResistanceText.Text = "Error";
                tempCoeffText.Text = "Error";
            }
        }

        private string FormatValue(double value)
        {
            if (value >= 1e9) return $"{value / 1e9:F2}G";
            if (value >= 1e6) return $"{value / 1e6:F2}M";
            if (value >= 1e3) return $"{value / 1e3:F2}k";
            return $"{value:F2}";
        }
    }
}