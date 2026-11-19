using System;

namespace DripLab
{
    public static class GeometriaRoleta
    {
        static double Normalizar(double valor){return (valor%360+360)%360;}
        public static float AnguloParada(float inicio,int indice,int quantidade)
        {
            if(quantidade<2||indice<0||indice>=quantidade)throw new ArgumentOutOfRangeException();
            return (float)(inicio+1800+Normalizar(-indice*360.0/quantidade-inicio));
        }
        public static int ResultadoNoPonteiro(double angulo,int quantidade)
        {
            return (int)Math.Floor(Normalizar(-angulo)/(360.0/quantidade)+.5)%quantidade;
        }
    }
}
