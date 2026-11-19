using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        Texture2D tabletInicioArte;
        int paginaNoticias;
        void DesenharInicioTablet()
        {
            if(tabletInicioArte==null)tabletInicioArte=Resources.Load<Texture2D>("UI/TabletHome");
            Matrix4x4 anterior=IniciarReferenciaTela(tabletInicioArte);
            if(tabletInicioArte==null)
                Texto("O painel do tablet não carregou. Feche com Esc e tente novamente.",250,350,1172,100,32,Creme,true);
            else
            {
                // Hit areas are in the same reference coordinates as the supplied artwork.
                if(InterfaceCena.Botao(new Rect(430,353,826,150),new GUIContent("","Pedidos"),GUIStyle.none))
                {Toque();aba=3;paginaPedidos=0;produtoPedido=-1;pedidoProdutosPagina=0;}
                if(InterfaceCena.Botao(new Rect(430,553,826,150),new GUIContent("","Gestão"),GUIStyle.none))
                {Toque();aba=8;}
                if(InterfaceCena.Botao(new Rect(430,751,826,145),new GUIContent("","Notícias"),GUIStyle.none))
                {Toque();aba=7;paginaNoticias=0;diarioNoticias=false;}
                Texto(Estado.pedidos.Count.ToString(),1214,316,80,70,35,Color.white,true,TextAnchor.MiddleCenter);
            }
            if(BotaoFecharIlustrado(new Rect(1565,17,68,68),"Fechar tablet"))tabletAberto=false;
            InterfaceCena.matriz=anterior;
        }
        void TabletNoticiasPagina()
        {
            Cabecalho("NOTÍCIAS / DRIPLAB","O que está rolando na sua marca.","Acompanhe acontecimentos, entregas, campanhas e novidades do negócio.");
            int paginaQuantidade=Mathf.Max(1,(Estado.noticias.Count+4)/5);
            paginaNoticias=Mathf.Clamp(paginaNoticias,0,paginaQuantidade-1);
            if(Estado.noticias.Count==0)Pequeno("Nenhuma notícia ainda. As novidades aparecem durante o jogo.",698,430);
            for(int i=0;i<5;i++)
            {
                int indice=paginaNoticias*5+i;if(indice>=Estado.noticias.Count)break;
                Texto(Estado.noticias[indice],698,425+i*57,670,50,16,Color.white);
            }
            if(Botao("ANTERIOR",698,721,175,32,Cartao,paginaNoticias>0,13))paginaNoticias--;
            Texto((paginaNoticias+1)+" / "+paginaQuantidade,925,726,200,26,14,SemSom,false,TextAnchor.MiddleCenter);
            if(Botao("PRÓXIMA",1194,721,174,32,Cartao,paginaNoticias<paginaQuantidade-1,13))paginaNoticias++;
        }
    }
}
