using System;

namespace DripLab
{
    public sealed class TemporizadorCarregamento
    {
        public const double Duracao=3;
        double tempoDecorrido;
        public bool Ativo {get;private set;}
        public float Progresso {get{return (float)Math.Min(1,tempoDecorrido/Duracao);}}
        public void Start(){tempoDecorrido=0;Ativo=true;}
        public void Cancelar(){tempoDecorrido=0;Ativo=false;}
        public void Atualizar(double intervalo)
        {
            if(!Ativo || intervalo<=0 || double.IsNaN(intervalo) || double.IsInfinity(intervalo))return;
            tempoDecorrido=Math.Min(Duracao,tempoDecorrido+intervalo);
            if(tempoDecorrido>=Duracao)Ativo=false;
        }
    }
}
