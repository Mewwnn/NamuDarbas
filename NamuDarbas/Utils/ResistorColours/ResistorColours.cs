using System.Collections.Generic;
using Avalonia.Media;

namespace NamuDarbas.Utils.ResistorColours;

public static class ResistorColours
{
    public static readonly Dictionary<string, Color> ColorValues = new Dictionary<string, Color>
    {
        {"Black", Colors.Black},
        {"Brown", Colors.Brown},
        {"Red", Colors.Red},
        {"Orange", Colors.Orange},
        {"Yellow", Colors.Yellow},
        {"Green", Colors.Green},
        {"Blue", Colors.Blue},
        {"Violet", Colors.Violet},
        {"Gray", Colors.Gray},
        {"White", Colors.White},
        {"Gold", Colors.Gold},
        {"Silver", Colors.Silver}
    };

    public static readonly Dictionary<string, int> ColorDigits = new Dictionary<string, int>
    {
        {"Black", 0},
        {"Brown", 1},
        {"Red", 2},
        {"Orange", 3},
        {"Yellow", 4},
        {"Green", 5},
        {"Blue", 6},
        {"Violet", 7},
        {"Gray", 8},
        {"White", 9}
    };

    public static readonly Dictionary<string, double> Multipliers = new Dictionary<string, double>
    {
        {"Black", 1},
        {"Brown", 10},
        {"Red", 100},
        {"Orange", 1000},
        {"Yellow", 10000},
        {"Green", 100000},
        {"Blue", 1000000},
        {"Violet", 10000000},
        {"Gray", 100000000},
        {"White", 1000000000},
        {"Gold", 0.1},
        {"Silver", 0.01}
    };

    public static readonly Dictionary<string, double> Tolerances = new Dictionary<string, double>
    {
        {"Brown", 1},
        {"Red", 2},
        {"Green", 0.5},
        {"Blue", 0.25},
        {"Violet", 0.1},
        {"Gray", 0.05},
        {"Gold", 5},
        {"Silver", 10}
    };

    public static readonly Dictionary<string, int> TempCoefficients = new Dictionary<string, int>
    {
        {"Brown", 100},
        {"Red", 50},
        {"Orange", 15},
        {"Yellow", 25}
    };
}