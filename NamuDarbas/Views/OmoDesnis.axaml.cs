using System;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NamuDarbas.ViewModels;
using ReactiveUI;

namespace NamuDarbas.Views
{

    public partial class OmoDesnis : Window
    {
        private static int SkaiciavimoOperacijos;
        

        static OmoDesnis()
        {
            SkaiciavimoOperacijos = 0;
            System.Console.WriteLine("Statinis konstruktorius pakviestas");
        }
        public OmoDesnis()
        {
            InitializeComponent();
        }

        ~OmoDesnis()
        {
            SkaiciavimoOperacijos = 0;
        }

        private void PaspaustasSkaiciuoti(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            SkaiciavimoOperacijos++;
            
            string? R = Varza.Text;
            string? V = Itampa.Text;
            string? I = Srove.Text;

            double SkaiciusR = 0, SkaiciusV = 0, SkaiciusI = 0, SkaiciusP = 0;
            bool TikrinimasR = double.TryParse(R, out SkaiciusR);
            bool TikrinimasV = double.TryParse(V, out SkaiciusV);
            bool TikrinimasI = double.TryParse(I, out SkaiciusI);

            if (!TikrinimasR && TikrinimasV && TikrinimasI)
            {
                // Truktsa Varzos: Varzos formule: R = U\I
                if (SkaiciusI != 0)
                {
                    SkaiciusR = SkaiciusV / SkaiciusI;
                    ApskaiciuotasR.Text = $"{SkaiciusR} Ω";
                
                    // Galios formule: P = U*I 
                    SkaiciusP = SkaiciusV * SkaiciusI;
                    ApskaiciuotasP.Text = $"{SkaiciusP} W";
                    ApskaiciuotasI.Text = $"{SkaiciusI} A";
                    ApskaiciuotasV.Text = $"{SkaiciusV} V";
                }
                else
                {
                    ApskaiciuotasR.Text = $"Srove negali būti apskaičiuota";
                    ApskaiciuotasP.Text = $"0 W";
                    ApskaiciuotasI.Text = $"{SkaiciusI} A";
                    ApskaiciuotasV.Text = $"{SkaiciusV} V";
                }    
            }
                 
            else if (TikrinimasR && !TikrinimasV && TikrinimasI)
            {
                // Truksta Itampos: Itampos formule: U = I * R
                SkaiciusV = SkaiciusI * SkaiciusR;
                ApskaiciuotasV.Text = $"{SkaiciusV} V";
                // Galio formule: P = I^2 * R
                SkaiciusP = (SkaiciusI*SkaiciusI) * SkaiciusR;
                ApskaiciuotasP.Text = $"{SkaiciusP} W";
                ApskaiciuotasI.Text = $"{SkaiciusI} A";
                ApskaiciuotasR.Text = $"{SkaiciusR} Ω";
            }
            
            else if (TikrinimasR && TikrinimasV && !TikrinimasI)
            {
                // Truksta Sroves: Sroves formule: I = U\R
                // Galios formule: U^2\R
                if (SkaiciusR != 0)
                {
                    SkaiciusI = SkaiciusV / SkaiciusR;
                    SkaiciusP = (SkaiciusV * SkaiciusV) / SkaiciusR;
                    ApskaiciuotasI.Text = $"{SkaiciusI} A";
                    ApskaiciuotasP.Text = $"{SkaiciusP} W";
                    ApskaiciuotasR.Text = $"{SkaiciusR} Ω";
                    ApskaiciuotasV.Text = $"{SkaiciusV} V";
                }
                else
                {
                    ApskaiciuotasI.Text = $"Srovė negali būti apskaičiuota.";
                    ApskaiciuotasP.Text = $"Galia negali būti spakaičiuota.";
                    ApskaiciuotasR.Text = $"{SkaiciusR} Ω";
                    ApskaiciuotasV.Text = $"{SkaiciusV} V";
                }
            }
            System.Console.WriteLine($"Buvo atliktos: {SkaiciavimoOperacijos} skaičiavimo operacijos");
            
        }
    }
}

