using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        bool selecaoCaixasAberta;
        Texture2D caixasAtalhoArte,caixaSelecaoArte;
        static readonly Rect[] CaixaSelecaoAreasClique={new Rect(598,460,201,60),new Rect(1164,460,204,60),new Rect(598,755,201,61),new Rect(1164,755,204,61)};

        bool DesenharAtalhoCaixas()
        {
            if(caixasAtalhoArte==null)caixasAtalhoArte=Resources.Load<Texture2D>("UI/BoxesShortcut");
            Rect area=new Rect(536,731,600,200);
            if(caixasAtalhoArte==null)return Botao("CAIXAS",586,790,500,100,Amarelo,true,32);
            InterfaceCena.DesenharTextura(area,caixasAtalhoArte,ScaleMode.ScaleToFit);
            return InterfaceCena.Botao(new Rect(569,761,534,142),new GUIContent("","Escolher caixa para este pedido"),GUIStyle.none);
        }
        void AbrirSelecaoCaixas()
        {
            if(!dobraPecas.EstaDobrada(dobraPecas.IdAtivo))return;
            if(caixaSelecaoArte==null)caixaSelecaoArte=Resources.Load<Texture2D>("UI/BoxSelection");
            if(caixaSelecaoArte==null){AdicionarTecidoAviso("A tela de caixas não carregou.",false,"IMAGEM INDISPONÍVEL");return;}
            selecaoCaixasAberta=true;Toque();
        }
        void DesenharSelecaoCaixas(Pedido pedido)
        {
            InterfaceCena.DefinirTela("Embalagem/Caixas");
            InterfaceCena.DesenharTextura(new Rect(0,0,1672,941),caixaSelecaoArte,ScaleMode.StretchToFill);
            for(int i=0;i<CaixaSelecaoAreasClique.Length;i++)
            {
                if(InterfaceCena.Botao(CaixaSelecaoAreasClique[i],new GUIContent("","Selecionar caixa "+EconomiaJogo.Caixas[i]),GUIStyle.none))
                {
                    if(!PrepararCaixaPedido(pedido,i))break;
                    AdicionarTecidoAviso("Caixa "+EconomiaJogo.Caixas[i]+" selecionada. Pedido pronto para entregar.",true,"CAIXA SELECIONADA");
                    break;
                }
            }
            if(BotaoVoltarIlustrado(new Rect(23,855,366,65),new GUIContent("","Voltar à bancada"),GUIStyle.none,caixaSelecaoArte))
            {selecaoCaixasAberta=false;Toque();}
        }
    }
}
