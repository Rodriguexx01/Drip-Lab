using System;

namespace DripLab
{
    public sealed class RelogioDia
    {
        public const int MinutoInicial=12*60, MinutoFinal=22*60;
        public double Minutos {get;private set;}=MinutoInicial;
        public string Exibicao {get {int minuto=(int)Minutos;return (minuto/60).ToString("00")+":"+(minuto%60).ToString("00");}}
        public void Reiniciar(){Minutos=MinutoInicial;}
        public bool Avancar(double realSegundos,bool emAndamento,double segundosPorMinuto=1)
        {
            if(!emAndamento||realSegundos<=0||double.IsNaN(realSegundos)||double.IsInfinity(realSegundos)||segundosPorMinuto<=0||double.IsNaN(segundosPorMinuto)||double.IsInfinity(segundosPorMinuto))return false;
            Minutos+=realSegundos/segundosPorMinuto;
            if(Minutos<MinutoFinal)return false;
            Reiniciar();return true;
        }
    }
}
