using System;

namespace NamuDarbas.Utils.Formating;

public class ResistanceFormat : IFormattable
{
    private readonly double _value;

    public ResistanceFormat(double value)
    {
        _value = value;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format)) format = "G"; // Default formatas
        format = format.ToUpperInvariant(); // case-insensitive
        
        return format switch
        {
            "G" => FormatValue(_value),         
            "Ω" => $"{_value:F2}Ω",              
            "KΩ" => $"{_value / 1e3:F2}kΩ",      
            "MΩ" => $"{_value / 1e6:F2}MΩ",       
            "GΩ" => $"{_value / 1e9:F2}GΩ",      
            _ => _value.ToString(format, formatProvider) // Default formatas jei neatpazistas
        };
    }

    private string FormatValue(double value)
    {
        if (value >= 1e9) return $"{value / 1e9:F2}GΩ";
        if (value >= 1e6) return $"{value / 1e6:F2}MΩ";
        if (value >= 1e3) return $"{value / 1e3:F2}kΩ";
        return $"{value:F2}Ω";
    }
    
    public override string ToString()
    {
        return FormatValue(_value);
    }
}
