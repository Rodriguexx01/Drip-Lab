using System;

namespace DripLab
{
    // Pure timing/state model; independent from frames, audio and the renderer.
    public sealed class ReproducaoHistoria
    {
        public static readonly string[] Linhas={
            "Aí, moleque, esse dinheiro é pra te ajudar a começar. Não precisa devolver. Faz dar certo!",
            "Valeu, tio! Pode confiar, vai dar certo! Vou fazer essa marca acontecer."
        };
        public const double CaracteresPorSegundo=32;
        // Ambas as páginas aguardam uma ação explícita do jogador.
        public bool Ativo {get;private set;}
        public int Pagina {get;private set;}
        double tempoDecorrido;
        public int VisivelQuantidade {get{return Math.Min(Linhas[Pagina].Length,(int)Math.Floor(tempoDecorrido*CaracteresPorSegundo));}}
        public bool EstaDigitando {get{return Ativo&&VisivelQuantidade<Linhas[Pagina].Length;}}
        public void Start() {Ativo=true;Pagina=0;tempoDecorrido=0;}
        public void Atualizar(double intervalo)
        {
            if(!Ativo || intervalo<=0 || double.IsNaN(intervalo) || double.IsInfinity(intervalo))return;
            tempoDecorrido=Math.Min(tempoDecorrido+intervalo,Linhas[Pagina].Length/CaracteresPorSegundo);
            // Terminar a digitação nunca conclui a história sozinho.
        }
        public void Pular()
        {
            if(!Ativo)return;
            if(Pagina==0){Pagina=1;tempoDecorrido=0;}
            else Ativo=false;
        }
        public void Revelar()
        {
            if(Ativo&&EstaDigitando)tempoDecorrido=Linhas[Pagina].Length/CaracteresPorSegundo;
        }
    }
}
