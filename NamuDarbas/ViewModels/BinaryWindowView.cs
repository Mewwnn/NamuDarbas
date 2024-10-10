using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NamuDarbas.ViewModels;

public class BinaryWindowView : INotifyPropertyChanged
{
    private string _binVerte;
    private string _octVerte;
    private string _decVerte;
    private string _hexVerte;
    
    public string BinVerte
    {
        get => _binVerte;
        set
        {
            if (_binVerte != value)
            {
                _binVerte = value;
                OnPropertyChanged(nameof(BinVerte));
                KonvertuotiIsBin(value);
            }
        }
    }

    public string OctVerte
    {
        get => _octVerte;
        set
        {
            if (_octVerte != value)
            {
                _octVerte = value;
                OnPropertyChanged(nameof(OctVerte));
                KonvertuotiIsOct(value);
            }
        }
    }

    public string DecVerte
    {
        get => _decVerte;
        set
        {
            if (_decVerte != value)
            {
                _decVerte = value;
                OnPropertyChanged(nameof(DecVerte));
                KonvertuotiIsDec(value);
            }
        }
    }

    public string HexVerte
    {
        get => _hexVerte;
        set
        {
            if (_hexVerte != value)
            {
                _hexVerte = value;
                OnPropertyChanged(nameof(HexVerte));
                KonvertuotiIsHex(value);
            }
        }
    }
    private void KonvertuotiIsBin(string bin)
    {
            int dec = Convert.ToInt32(bin, 2);
            AtnaujintiVertes(dec);
    }

    private void KonvertuotiIsOct(string oct)
    {
            int dec = Convert.ToInt32(oct, 8);
            AtnaujintiVertes(dec);
    }

    private void KonvertuotiIsDec(string dec)
    {   // out argumentas
        if (int.TryParse(dec, out int decVerte))
        {
            AtnaujintiVertes(decVerte);
        }
    }

    private void KonvertuotiIsHex(string hex)
    {
        if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int dec))
        {
            AtnaujintiVertes(dec);
        }
    }

    private void AtnaujintiVertes(int dec)
    {
        _binVerte = Convert.ToString(dec, 2);
        _octVerte = Convert.ToString(dec, 8);
        _decVerte = dec.ToString();
        _hexVerte = dec.ToString("X");
        
        OnPropertyChanged(nameof(BinVerte));
        OnPropertyChanged(nameof(OctVerte));
        OnPropertyChanged(nameof(DecVerte));
        OnPropertyChanged(nameof(HexVerte));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}