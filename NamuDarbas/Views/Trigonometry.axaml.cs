using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OxyPlot;
using NP.Ava.Visuals;
using OxyPlot.Axes;
using OxyPlot.Avalonia;
using MathNet.Symbolics;
using System;
using System.Collections.Generic;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using OxyPlot.Annotations;
using ReactiveUI;
using Expr = MathNet.Symbolics.SymbolicExpression;
using LineAnnotation = OxyPlot.Avalonia.LineAnnotation;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using System;
using System.Collections.Generic; 

namespace NamuDarbas.Views;

public partial class Trigonometry : Window
{
    private TextBox? _inputTextBox;
    private PlotView? _plotView;

    public Trigonometry()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _inputTextBox = this.FindControl<TextBox>("InputTextBox");
        _plotView = this.FindControl<PlotView>("PlotView");
        var plotButton = this.FindControl<Button>("PlotButton");

        if (plotButton != null) plotButton.Click += PlotButton_Click;
    }

   private async void PlotButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_inputTextBox != null && _inputTextBox.Text != null)
            {
                string input = _inputTextBox.Text;
                if (!string.IsNullOrWhiteSpace(input))
                {
                    try
                    {
                        PlotFunction(input);
                    }
                    catch (Exception ex)
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Error", $"Invalid input: {ex.Message}");
                        var result = await box.ShowAsync();
                    }
                }
            }
        }

        private void PlotFunction(string functionInput)
        {
            var model = new PlotModel { Title = "Trigonometric Graph" };
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "X Axis",
                Minimum = -10,
                Maximum = 10,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Y Axis",
                Minimum = -10,
                Maximum = 10,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            Expr expression = Expr.Parse(functionInput);
            var series = new LineSeries { Title = functionInput, Color = OxyColors.Blue, StrokeThickness = 2 };

            for (double x = -10; x <= 10; x += 0.1)
            {
                try
                {
                    double y = expression.Evaluate(new Dictionary<string, FloatingPoint> { { "x", x } }).RealValue;
                    if (!double.IsNaN(y) && !double.IsInfinity(y))
                    {
                        series.Points.Add(new DataPoint(x, y));
                    }
                }
                catch
                {
                    // Skip points that cannot be evaluated
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }


public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindow(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

public class MainWindowViewModel : ViewModelBase
{
}

public class ViewModelBase : ReactiveObject
{
}

