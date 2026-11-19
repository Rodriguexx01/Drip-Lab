using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D vitoriaArte,derrotaArte,botoesFinalArte;
        static readonly Rect[] BotoesFinalUv={
            new Rect(.0406f,.5343f,.4488f,.2650f), // Jogar novamente laranja
            new Rect(.5110f,.5343f,.4480f,.2650f), // Jogar novamente azul
            new Rect(.0406f,.1627f,.4488f,.2640f), // Voltar ao menu azul
            new Rect(.5110f,.1627f,.4480f,.2640f)  // Voltar ao menu laranja
        };
        bool BotaoFinalIlustrado(Rect area,int arte,string descricao)
        {
            if(botoesFinalArte!=null)InterfaceCena.DesenharTexturaComCoordenadas(area,botoesFinalArte,BotoesFinalUv[arte]);
            else {UrbanoArredondado(area,18,arte==0||arte==3?Amarelo:Azul);Texto(descricao.ToUpperInvariant(),area.x,area.y,area.width,area.height,24,Creme,true,TextAnchor.MiddleCenter);}
            return InterfaceCena.Botao(area,new GUIContent("",descricao),GUIStyle.none);
        }
        void DesenharArteFinal()
        {
            bool vitoria=Estado.final=="win";
            InterfaceCena.DefinirTela(vitoria?"Final/Vitoria":"Final/Falencia");
            if(vitoriaArte==null)vitoriaArte=Resources.Load<Texture2D>("UI/Victory");
            if(derrotaArte==null)derrotaArte=Resources.Load<Texture2D>("UI/Defeat");
            if(botoesFinalArte==null)botoesFinalArte=Resources.Load<Texture2D>("UI/FinalButtons");
            Matrix4x4 anterior=IniciarReferenciaTela(vitoria?vitoriaArte:derrotaArte);
            if(vitoria)
            {
                string capitalTotal="R$ "+Estado.dinheiro.ToString("N2",System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                Texto(capitalTotal,604,606,650,88,capitalTotal.Length>14?40:52,Creme,true,TextAnchor.MiddleCenter);
                Texto("EM "+Estado.dia+" DIA"+(Estado.dia==1?"":"S")+" · "+Estado.entregues+" PEDIDOS ENTREGUES",450,762,772,38,21,Creme,true,TextAnchor.MiddleCenter);
            }
            else
            {
                Texto("DIA "+Estado.dia+" · CAPITAL: "+FormatarDinheiro(Estado.dinheiro)+" · "+Estado.entregues+" PEDIDOS ENTREGUES",400,750,872,40,21,Creme,true,TextAnchor.MiddleCenter);
            }
            if(BotaoFinalIlustrado(new Rect(388,818,410,92),vitoria?0:1,"Jogar novamente"))ReiniciarAposFinal(false);
            if(BotaoFinalIlustrado(new Rect(874,818,410,92),vitoria?2:3,"Voltar ao menu"))ReiniciarAposFinal(true);
            InterfaceCena.matriz=anterior;
        }
        void ReiniciarAposFinal(bool paraMenu)
        {
            NovaPartida();menuAberto=paraMenu;
            if(!paraMenu)IniciarHistoria();
            Toque();
        }
    }
}
